using libplctag.NativeImport;
using System.Text;
using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Interfaces;
using VikingLibPlcTagNet.Settings;
using VikingLibPlcTagNet.Tags;

namespace VikingLibPlcTagNet.Data
{
    /// <summary>
    /// Retrieve and process data from PLC using libplctag C library native calls
    /// and AB special tags.
    /// </summary>
    public sealed class PlcInfo : IDisposable
    {
        private readonly IList<TagInfo> tagInfos;
        private readonly IList<TemplateInfo> templateInfos;
        private readonly Dictionary<string, ITag> tagCache = [];

        public void Dispose()
        {
            foreach (var tag in tagCache.Values)
                tag.Dispose();
        }

        private PlcInfo(IList<TagInfo> tagInfos, IList<TemplateInfo> templateInfos)
        {
            this.tagInfos = tagInfos;
            this.templateInfos = templateInfos;
        }

        public static PlcInfo? Build(TagListing tagListing, ILoggable? logger = null, Func<bool>? cancellationToken = null)
        {
            List<TagInfo> tagInfos = [];
            List<ushort> templateIds = [];
            List<string> programNames = [];
            var offset = 0;

            while (offset < tagListing.PayloadSize)
            {
                if (cancellationToken is not null && cancellationToken())
                    return null;

                var id = plctag.plc_tag_get_uint32(tagListing.Id, offset);
                offset += 4;

                var type = plctag.plc_tag_get_uint16(tagListing.Id, offset);
                offset += 2;

                var elementLength = plctag.plc_tag_get_uint16(tagListing.Id, offset);
                offset += 2;

                var arrayDims = new uint[3];

                arrayDims[0] = plctag.plc_tag_get_uint32(tagListing.Id, offset);
                offset += 4;

                arrayDims[1] = plctag.plc_tag_get_uint32(tagListing.Id, offset);
                offset += 4;

                arrayDims[2] = plctag.plc_tag_get_uint32(tagListing.Id, offset);
                offset += 4;

                var nameLength = plctag.plc_tag_get_string_length(tagListing.Id, offset) + 1;

                var name = new StringBuilder(nameLength);

                var result = plctag.plc_tag_get_string(tagListing.Id, offset, name, nameLength);

                if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                {
                    name = new StringBuilder("[ERROR]");
                    Helper.LogError(logger, "Error reading tag name", [], result);
                }

                offset += plctag.plc_tag_get_string_total_length(tagListing.Id, offset);

                // Save program names for further processing.
                if (name.ToString().StartsWith("Program:"))
                    programNames.Add(name.ToString().Split(':')[1]);

                // Skip system tags.
                if ((type & BitMasks.SystemBit) > 0 ||
                    name.ToString().StartsWith("__") ||
                    name.ToString().Contains(':'))
                    continue;

                var tagInfo = TagInfo.Create(tagListing.Path, id, (DataTypes)type, elementLength, arrayDims, name);

                if (tagInfo.NumberDimensions > 1)
                {
                    Helper.LogError(logger, "Skipping array of more than one dimension", [tagInfo.NumberDimensions, tagInfo.Name]);
                    continue;
                }

                if (tagInfo.NumberDimensions == 1)
                {
                    var arrayTagId = plctag.plc_tag_create(tagListing.Path.GetAttributeString(tagInfo.Name), tagListing.Path.Timeout);

                    if (arrayTagId < 0)
                    {
                        Helper.LogError(logger, "Error reading array", [tagInfo.Name], arrayTagId);
                        continue;
                    }

                    var typeSize = plctag.plc_tag_get_int_attribute(arrayTagId, "raw_tag_type_bytes.length", -1);

                    if (typeSize != 2 && typeSize != 4)
                    {
                        Helper.LogError(logger, "Unexpected tag type size for array", [tagInfo.Name]);
                        continue;
                    }

                    var buffer = new byte[typeSize];

                    plctag.plc_tag_get_byte_array_attribute(arrayTagId, "raw_tag_type_bytes", buffer, buffer.Length);
                    plctag.plc_tag_destroy(arrayTagId);

                    // if 4, first two bytes don't appear to matter, getting 672 (0x2A0).
                    var arrayTagType = buffer.Length == 2
                        ? BitConverter.ToUInt16(buffer, 0)
                        : BitConverter.ToUInt16([buffer[2], buffer[3]], 0);

                    if (!Helper.TryGetDataType(arrayTagType, out var arrayTagDataType) ||
                        arrayTagDataType is null)
                    {
                        Helper.LogError(logger, "Skipping array of unknown element data type", [arrayTagType, tagInfo.Name]);
                        continue;
                    }

                    var elementCount = arrayDims[0];

                    // this is BOOL array, stored in PLC as bits of DWORD
                    if (arrayTagDataType == DataTypes.DWORD)
                    {
                        arrayTagDataType = DataTypes.BOOL;
                        elementCount = 32;
                    }

                    for (var i = 0; i < elementCount; i++)
                    {
                        var elementName = arrayTagDataType == DataTypes.BOOL
                            ? $"{tagInfo.Name}.{i}"
                            : $"{tagInfo.Name}[{i}]";

                        tagInfos.Add(
                            TagInfo.Create(
                                tagListing.Path, (uint)arrayTagId, (DataTypes)arrayTagDataType, 1, [], elementName));
                    }

                    continue;
                }

                tagInfos.Add(tagInfo);

                // Save template ids for further processing.
                if (tagInfo.IsUdt)
                    templateIds.Add(tagInfo.TemplateId);
            }

            // Process template ids.

            List<TemplateInfo> templateInfos = [];

            foreach (var id in templateIds.Distinct())
            {
                if (cancellationToken is not null && cancellationToken())
                    return null;

                using var templateInfo = TagFactory.GetTagFor(null, tagListing.Path, $"@udt/{id}", logger);

                if (templateInfo is null)
                    continue;

                var tagSize = plctag.plc_tag_get_size(templateInfo.Id);

                var templateId = plctag.plc_tag_get_uint16(templateInfo.Id, 0);

                var memberDescriptionSize = plctag.plc_tag_get_uint32(templateInfo.Id, 2);

                var instanceSize = plctag.plc_tag_get_uint32(templateInfo.Id, 6);

                var numMembers = plctag.plc_tag_get_uint16(templateInfo.Id, 10);

                var handle = plctag.plc_tag_get_uint16(templateInfo.Id, 12);

                // Sanity check.
                if (templateId != id)
                {
                    Helper.LogError(logger, "ID mismatch while reading UDT information.", [templateId, id]);
                    return null;
                }

                // Skip past this header.
                offset = 14;

                // Process fields.

                List<FieldInfo> fields = [];

                for (int fieldIndex = 0; fieldIndex < numMembers; fieldIndex++)
                {
                    if (cancellationToken is not null && cancellationToken())
                        return null;

                    var fieldMetadata = plctag.plc_tag_get_uint16(templateInfo.Id, offset);
                    offset += 2;

                    var fieldType = plctag.plc_tag_get_uint16(templateInfo.Id, offset);
                    offset += 2;

                    var fieldOffset = plctag.plc_tag_get_uint32(templateInfo.Id, offset);
                    offset += 4;

                    var field = FieldInfo.Create(fieldMetadata, fieldType, fieldOffset);

                    if (field.IsUdt)
                    {
                        // Can't do this since we are enumerating templateIds.Distinct() here.
                        // ICS is okay with skipping these.
                        // templateIds.Add(field.TemplateId);
                    }

                    fields.Add(field);
                }

                // Process template name.
                // Notes from C library example (https://github.com/libplctag/libplctag/blob/release/src/examples/list_tags_logix.c):
                //  then get the template/UDT name.   This is weird.
                //  Scan until we see a 0x3B, semicolon, byte.   That is the end of the
                //  template name.   Actually we should look for ";n" but the semicolon
                //  seems to be enough for now.

                var nameLength = plctag.plc_tag_get_string_length(templateInfo.Id, offset) + 1;

                var name = new StringBuilder(nameLength);

                var result = plctag.plc_tag_get_string(templateInfo.Id, offset, name, nameLength);

                if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                {
                    name = new StringBuilder("[ERROR];");
                    Helper.LogError(logger, "Error reading template name", [], result);
                }

                string udtName = name
                    .ToString()
                    .Split(';')
                    .FirstOrDefault() ?? string.Empty;

                offset += plctag.plc_tag_get_string_total_length(templateInfo.Id, offset);

                // Process field names.

                List<FieldInfo> fieldsWithNames = [];

                foreach (var field in fields)
                {
                    if (cancellationToken is not null && cancellationToken())
                        return null;

                    if (offset >= tagSize)
                    {
                        Helper.LogError(logger, "Refusing to read past tag size.", [offset, tagSize]);
                        break;
                    }

                    nameLength = plctag.plc_tag_get_string_length(templateInfo.Id, offset) + 1;

                    name = new StringBuilder(nameLength);

                    if (nameLength <= 0)
                    {
                        offset++;
                        continue;
                    }

                    result = plctag.plc_tag_get_string(templateInfo.Id, offset, name, nameLength + 1);

                    if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                    {
                        name = new StringBuilder("[ERROR]");
                        Helper.LogError(logger, "Error reading parameter name", [], result);
                    }

                    // ICS is okay with skipping fields (parameters) that are UDTs.
                    if (!field.IsUdt && !name.ToString().StartsWith("__"))
                        fieldsWithNames.Add(field.WithName(name));

                    offset += plctag.plc_tag_get_string_total_length(templateInfo.Id, offset);
                }

                // I think this is padding for alignment.
                // We don't seem to be missing any information, so if anything does appear to
                // be missing, this would be a good place to start looking.
                if (offset != tagSize)
                    Console.Error.WriteLine($"Processed {offset} bytes out of {tagSize} for {udtName}.");

                templateInfos.Add(
                    TemplateInfo.Create(
                        tagListing.Path, udtName, templateInfo.Id,
                        tagSize, templateId, memberDescriptionSize, instanceSize, numMembers, handle,
                        fieldsWithNames));
            }

            // Process programs.

            foreach (var programName in programNames)
            {
                if (cancellationToken is not null && cancellationToken())
                    return null;

                using var programTagListing = TagListing.CreateForProgram(tagListing.Path, programName, logger);

                if (programTagListing is null)
                    continue;

                var programInfo = Build(programTagListing, logger, cancellationToken);

                if (programInfo is null)
                    return null;

                tagInfos.AddRange(
                    programInfo.Tags.Select(
                        t => t.WithProgramName(programName)));

                templateInfos.AddRange(
                    programInfo.Templates.Select(
                        t => t.WithProgramName(programName)));
            }

            return new(tagInfos, templateInfos);
        }

        public IList<TagInfo> Tags => tagInfos;

        public IList<TemplateInfo> Templates => templateInfos;

        public IEnumerable<string> ProgramNames => Tags
            .Where(t => t.ProgramName != string.Empty)
            .Select(t => t.ProgramName)
            .Distinct();

        public IEnumerable<string> GetTemplateNamesFor(string programName) => Templates
            .Where(t => t.ProgramName == programName)
            .Select(t => t.Name);

        public IEnumerable<string> GetFieldNamesFor(string programName, string templateName)
        {
            var template = Templates
                .Where(t => t.ProgramName == programName)
                .Where(t => t.Name == templateName)
                .SingleOrDefault();

            if (template is null)
                yield break;

            foreach (var field in template.Fields)
                yield return field.Name;
        }

        public IEnumerable<string> GetTagNamesFor(string programName, string templateName, string nameFilter)
        {
            var templateId = Templates
                .Where(t => t.ProgramName == programName)
                .Where(t => t.Name == templateName)
                .Select(t => t.TemplateId)
                .SingleOrDefault();

            return Tags
                .Where(t => t.IsUdt)
                .Where(t => t.ProgramName == programName)
                .Where(t => t.TemplateId == templateId)
                .Select(t => t.Name)
                .Where(n => n.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<string> GetTagNamesFor(string programName, string nameFilter)
        {
            return Tags
                .Where(t => !t.IsUdt)
                .Where(t => t.ProgramName == programName)
                .Select(t => t.Name)
                .Where(n => n.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase));
        }

        public ITag? GetTag(string programName, string tagName, string? templateName = null, string? parameterName = null, ILoggable? logger = null)
        {
            var udtRequired = templateName is not null && parameterName is not null;

            var tagInfo = Tags
                .Where(t => t.IsUdt == udtRequired)
                .Where(t => t.ProgramName == programName)
                .Where(t => t.Name == tagName)
                .SingleOrDefault();

            if (tagInfo is null)
                return null;

            string fqn = string.Empty;

            if (tagInfo.ProgramName != string.Empty)
                fqn = $"Program:{tagInfo.ProgramName}.";

            fqn = $"{fqn}{tagName}";

            var dataType = tagInfo.Type;

            if (udtRequired)
            {
                var template = Templates
                    .Where(t => t.ProgramName == tagInfo.ProgramName)
                    .Where(t => t.TemplateId == tagInfo.TemplateId)
                    .SingleOrDefault();

                if (template is null)
                    return null;

                // Sanity check.
                if (templateName != template.Name)
                    return null;

                var fieldInfo = template
                    .Fields
                    .Where(f => f.Name == parameterName)
                    .SingleOrDefault();

                if (fieldInfo is null)
                    return null;

                dataType = fieldInfo.Type;

                fqn = $"{fqn}.{parameterName}";
            }

            return GetCachedOrNewTag(dataType, tagInfo.Path, fqn, logger);
        }

        private ITag? GetCachedOrNewTag(DataTypes type, TagPath path, string fqn, ILoggable? logger = null)
        {
            if (tagCache.TryGetValue(fqn, out var tag))
            {
                tag.Read(logger);
                return tag;
            }

            var newTag = TagFactory.GetTagFor(type, path, fqn, logger);

            if (newTag is null)
                return null;

            tagCache.Add(fqn, newTag);

            return newTag;
        }
    }
}
