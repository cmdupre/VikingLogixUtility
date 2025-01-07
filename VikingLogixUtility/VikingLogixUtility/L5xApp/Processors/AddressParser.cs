using L5Sharp.Core;
using VikingLogixUtility.Interfaces;
using VikingLogixUtility.L5xApp.RegEx;

namespace VikingLogixUtility.L5xApp.Processors
{
    internal sealed class AddressParser
    {
        public static string GetAddress(ILoggable viewModel, IEnumerable<string> mapping, Tag tag)
        {
            List<string> tagMappings = [];

            foreach (var item in mapping)
            {
                var cleanedItem = item
                    .Replace(" ", "")
                    .Replace(";", "")
                    .Replace("[", "")
                    .Replace("]", "");

                var tagNameMatch = cleanedItem
                    .Split('(', ')', ',')
                    .Where(x => x.Equals(tag.Name, StringComparison.InvariantCultureIgnoreCase));

                if (tagNameMatch.Any())
                    tagMappings.Add(cleanedItem);
            }

            if (tagMappings.Count > 1)
                viewModel.Log($" - Warning: multiple mapping rungs found for tag {tag.Name}, using the first instance.");

            var tagMapping = tagMappings.FirstOrDefault();

            if (tagMapping != null)
            {
                var replacedMapping = tagMapping
                    .Replace("XIC", "\0", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("XIO", "\0", StringComparison.InvariantCultureIgnoreCase);

                var possibleParts = replacedMapping.Split('\0');

                foreach (var part in possibleParts)
                {
                    var parts = part.Split('(', ')');

                    if (parts.Length < 4)
                        continue;
                    
                    if (parts[1].Equals(tag.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        RegularExpressions.MappingFilter().IsMatch(parts[3]))
                            return parts[3];

                    if (parts[3].Equals(tag.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        RegularExpressions.MappingFilter().IsMatch(parts[1]))
                            return parts[1];
                }

                replacedMapping = tagMapping
                    .Replace("AnalogProcessing", "\0", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("Analog_AOI", "\0", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("AI_Alm", "\0", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("MOVE", "\0", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("MOV", "\0", StringComparison.InvariantCultureIgnoreCase);

                possibleParts = replacedMapping.Split('\0');

                foreach (var part in possibleParts)
                {
                    var parts = part
                        .Replace("(", "")
                        .Replace(")", "")
                        .Split(',');

                    if (parts.Length < 2)
                        continue;
                    
                    if (parts[0].Equals(tag.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        RegularExpressions.MappingFilter().IsMatch(parts[1]))
                            return parts[1];

                    if (parts[1].Equals(tag.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        RegularExpressions.MappingFilter().IsMatch(parts[0]))
                            return parts[0];
                }

                viewModel.Log($" - Warning: unable to parse address for tag {tag.Name}.");
            }

            return string.Empty;
        }
    }
}
