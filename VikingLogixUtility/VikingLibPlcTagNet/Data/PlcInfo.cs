using libplctag.NativeImport;
using System.Text;
using VikingLibPlcTagNet.Common;
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

        public static PlcInfo Build(TagListing tagListing)
        {
            List<TagInfo> tagInfos = [];
            List<ushort> templateIds = [];
            List<string> programNames = [];
            var offset = 0;

            while (offset < tagListing.PayloadSize)
            {
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
                    name = new StringBuilder(plctag.plc_tag_decode_error(result));

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
                
                tagInfos.Add(tagInfo);

                // Save template ids for further processing.
                if (tagInfo.IsUdt)
                    templateIds.Add(tagInfo.TemplateId);
            }

            // Process template ids.

            var templateInfos = new List<TemplateInfo>();

            foreach (var id in templateIds.Distinct())
            {
                using var templateInfo = TagFactory.GetTagFor(null, tagListing.Path, $"@udt/{id}");

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
                    throw new InvalidDataException("ID mismatch while reading UDT information.");

                // Skip past this header.
                offset = 14;

                // Process fields.

                List<FieldInfo> fields = [];

                for (int fieldIndex = 0; fieldIndex < numMembers; fieldIndex++)
                {
                    var fieldMetadata = plctag.plc_tag_get_uint16(templateInfo.Id, offset);
                    offset += 2;

                    var fieldType = plctag.plc_tag_get_uint16(templateInfo.Id, offset);
                    offset += 2;

                    var fieldOffset = plctag.plc_tag_get_uint32(templateInfo.Id, offset);
                    offset += 4;

                    var field = FieldInfo.Create(fieldMetadata, fieldType, fieldOffset);

                    // TODO: Can't do this since we are enumerating templateIds.Distinct() here.
                    //if (field.IsUdt)
                    //    templateIds.Add(field.TemplateId);

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
                    name = new StringBuilder(plctag.plc_tag_decode_error(result));

                string udtName = name
                    .ToString()
                    .Split(';')
                    .FirstOrDefault() ?? string.Empty;

                offset += plctag.plc_tag_get_string_total_length(templateInfo.Id, offset);

                // Process field names.

                List<FieldInfo> fieldsWithNames = [];

                foreach (var field in fields)
                {
                    if (offset >= tagSize)
                        throw new InvalidDataException("Refusing to read past tag size.");

                    nameLength = plctag.plc_tag_get_string_length(templateInfo.Id, offset) + 1;

                    name = new StringBuilder(nameLength);

                    if (nameLength <= 0)
                    {
                        offset++;
                        continue;
                    }

                    result = plctag.plc_tag_get_string(templateInfo.Id, offset, name, nameLength + 1);
                    
                    if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                        name = new StringBuilder(plctag.plc_tag_decode_error(result));

                    if (!name.ToString().StartsWith("__"))
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
                using var programTagListing = TagListing.CreateForProgram(tagListing.Path, programName);

                if (programTagListing is null)
                    continue;

                var programInfo = Build(programTagListing);

                tagInfos.AddRange(
                    programInfo.Tags.Select(
                        t => t.WithProgramName(programName)));

                templateInfos.AddRange(
                    programInfo.Templates.Select(
                        t => t.WithProgramName(programName)));
            }

            return new PlcInfo(tagInfos, templateInfos);
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

        public IEnumerable<string> GetTagNamesFor(string programName, string templateName)
        {
            var templateId = Templates
                .Where(t => t.ProgramName == programName)
                .Where(t => t.Name == templateName)
                .Select(t => t.TemplateId)
                .SingleOrDefault();

            var tags = Tags
                .Where(t => t.IsUdt)
                .Where(t => t.ProgramName == programName)
                .Where(t => t.TemplateId == templateId);

            foreach (var tag in tags)
                yield return tag.Name;
        }

        public IEnumerable<string> GetTagNamesFor(string programName)
        {
            var tags = Tags
                .Where(t => !t.IsUdt)
                .Where(t => t.ProgramName == programName);

            foreach (var tag in tags)
                yield return tag.Name;
        }

        public string? GetTagValue(string programName, string tagName, string? templateName = null, string? parameterName = null)
        {
            var tag = GetTag(programName, tagName, templateName, parameterName);

            tag?.Read();

            return tag?.Value;
        }

        public void WriteTagValue(string programName, string tagName, string value, string? templateName = null, string? parameterName = null)
        {
            var tag = GetTag(programName, tagName, templateName, parameterName);

            tag?.Write(value);
        }

        private ITag? GetTag(string programName, string tagName, string? templateName = null, string? parameterName = null)
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

            return GetCachedOrNewTag(dataType, tagInfo.Path, fqn);
        }

        private ITag? GetCachedOrNewTag(DataTypes type, TagPath path, string fqn)
        {
            if (tagCache.TryGetValue(fqn, out var tag))
                return tag;

            var newTag = TagFactory.GetTagFor(type, path, fqn);

            if (newTag is null)
                return null;

            tagCache.Add(fqn, newTag);

            return newTag;
        }
    }
}
