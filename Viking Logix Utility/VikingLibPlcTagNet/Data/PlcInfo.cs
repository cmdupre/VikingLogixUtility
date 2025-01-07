using libplctag.NativeImport;
using System.Text;
using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Tags;
using VikingLibPlcTagNet.Templates;

namespace VikingLibPlcTagNet.Data
{
    public sealed class PlcInfo
    {
        private readonly IList<TagInfo> tags;
        private readonly IList<Template> templates;

        private PlcInfo(IList<TagInfo> tags, IList<Template> templates)
        {
            this.tags = tags;
            this.templates = templates;
        }

        public static PlcInfo Build(TagListing tagListing)
        {
            List<TagInfo> tags = [];
            List<ushort> templateIds = [];
            List<string> programNames = [];
            var offset = 0;

            while (offset < tagListing.PayloadSize)
            {
                var id = (ushort)plctag.plc_tag_get_uint32(tagListing.Id, offset);
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

                var nameLength = plctag.plc_tag_get_string_length(tagListing.Id, offset);

                var name = new StringBuilder(nameLength + 1);

                var result = plctag.plc_tag_get_string(tagListing.Id, offset, name, nameLength + 1);
                
                if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                    Console.Error.WriteLine($"Unable to get name string, error {plctag.plc_tag_decode_error(result)}");

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
                
                tags.Add(tagInfo);

                // Save template ids for further processing.
                if (tagInfo.IsUdt)
                    templateIds.Add(tagInfo.TemplateId);
            }

            // Process UDT ids.

            var templates = new List<Template>();

            foreach (var id in templateIds.Distinct())
            {
                using var templateInfo = TagReadonly.Create(tagListing.Path, $"@udt/{id}");

                if (templateInfo is null)
                    continue;

                var tagSize = plctag.plc_tag_get_size(templateInfo.Id);

                var templateId = plctag.plc_tag_get_uint16(templateInfo.Id, 0);

                var memberDescriptionSize = plctag.plc_tag_get_uint32(templateInfo.Id, 2);

                var instanceSize = plctag.plc_tag_get_uint32(templateInfo.Id, 6);

                var numMembers = plctag.plc_tag_get_uint16(templateInfo.Id, 10);

                var handle = plctag.plc_tag_get_uint16(templateInfo.Id, 12);

                if (templateId != id)
                    Console.Error.WriteLine("ID mismatch while reading UDT information.");

                // Skip past this header.
                offset = 14;

                // Get fields.

                List<Field> fields = [];

                for (int fieldIndex = 0; fieldIndex < numMembers; fieldIndex++)
                {
                    var fieldMetadata = plctag.plc_tag_get_uint16(templateInfo.Id, offset);
                    offset += 2;

                    var fieldType = plctag.plc_tag_get_uint16(templateInfo.Id, offset);
                    offset += 2;

                    var fieldOffset = plctag.plc_tag_get_uint32(templateInfo.Id, offset);
                    offset += 4;

                    // TODO: check to see if member is UDT and save to process later, or process now?
                    // see line 780 https://github.com/libplctag/libplctag/blob/release/src/examples/list_tags_logix.c

                    fields.Add(Field.Create(fieldMetadata, fieldType, fieldOffset));
                }

                // Get UDT name.
                // Notes from C library example (https://github.com/libplctag/libplctag/blob/release/src/examples/list_tags_logix.c):
                //  then get the template/UDT name.   This is weird.
                //  Scan until we see a 0x3B, semicolon, byte.   That is the end of the
                //  template name.   Actually we should look for ";n" but the semicolon
                //  seems to be enough for now.

                var nameLength = plctag.plc_tag_get_string_length(templateInfo.Id, offset);
                if (nameLength <= 0 || nameLength >= 250)
                    Console.Error.WriteLine($"Unexpected raw UDT name length: {nameLength}.");

                var name = new StringBuilder(nameLength + 1);

                var result = plctag.plc_tag_get_string(templateInfo.Id, offset, name, nameLength + 1);
                if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                    Console.Error.WriteLine($"Error {plctag.plc_tag_decode_error(result)} retrieving UDT name string from the tag.");

                string udtName = name
                    .ToString()
                    .Split(';')
                    .FirstOrDefault() ?? string.Empty;

                offset += plctag.plc_tag_get_string_total_length(templateInfo.Id, offset);

                // Get field names.

                List<Field> fieldsWithNames = [];

                foreach (var field in fields)
                {
                    if (offset >= tagSize)
                    {
                        Console.Error.WriteLine("Refusing to read past tag size.");
                        break;
                    }

                    nameLength = plctag.plc_tag_get_string_length(templateInfo.Id, offset);

                    name = new StringBuilder(nameLength + 1);

                    if (nameLength < 0 || nameLength >= 256)
                        Console.Error.WriteLine($"Unexpected raw field name length: {nameLength}.");

                    if (nameLength == 0)
                    {
                        offset++;
                        continue;
                    }

                    // TODO: may need other filtering.
                    if (!name.ToString().StartsWith("__"))
                    {
                        result = plctag.plc_tag_get_string(templateInfo.Id, offset, name, nameLength + 1);
                        if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                            Console.Error.WriteLine($"Error {plctag.plc_tag_decode_error(result)} retrieving field name from the tag.");

                        fieldsWithNames.Add(field.WithName(name));
                    }

                    offset += plctag.plc_tag_get_string_total_length(templateInfo.Id, offset);
                }

                if (offset != tagSize)
                    Console.Error.WriteLine($"Did not finish reading UDT tag {udtName}.");

                if (fieldsWithNames.Count != numMembers)
                    Console.Error.WriteLine($"Did not finish reading UDT tag {udtName} members.");

                templates.Add(
                    Template.Create(
                        tagListing.Path, udtName, templateInfo.Id,
                        tagSize, templateId, memberDescriptionSize, instanceSize, numMembers, handle,
                        fieldsWithNames));
            }

            // Process programs.

            foreach (var programName in programNames)
            {
                using var programTagListing = TagListing.CreateForProgram(tagListing.Path, programName);

                if (programTagListing is null)
                {
                    Console.Error.WriteLine($"Unable to get tag listing for program {programName}.");
                    continue;
                }

                var programInfo = Build(programTagListing);

                tags.AddRange(
                    programInfo.Tags.Select(
                        t => t.WithProgramName(programName)));

                templates.AddRange(programInfo.Templates);
            }

            return new PlcInfo(tags, templates);
        }

        public IList<TagInfo> Tags => tags;

        public IList<Template> Templates => templates;
    }
}
