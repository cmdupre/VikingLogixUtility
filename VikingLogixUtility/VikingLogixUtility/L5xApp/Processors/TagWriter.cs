using L5Sharp.Core;
using System.Data;
using VikingLogixUtility.L5xApp.Interfaces;
using VikingLogixUtility.L5xApp.Models;
using VikingLogixUtility.L5xApp.RegEx;

namespace VikingLogixUtility.L5xApp.Processors
{
    internal static partial class TagWriter
    {
        public static void Geaux(
            ITagExportViewModel viewModel,
            ExportTable table,
            IEnumerable<string> mapping,
            string scope,
            IEnumerable<Tag> tags)
        {
            viewModel.Log($"Exporting {scope}...");

            foreach (var tag in tags)
            {
                var address = AddressParser.GetAddress(viewModel, mapping, tag);

                // If tag is alias for I/O point, move alias to address field.
                if (string.IsNullOrWhiteSpace(address) &&
                    (tag.TagType ?? TagType.Base) == TagType.Alias &&
                    !string.IsNullOrWhiteSpace(tag.AliasFor) &&
                    RegularExpressions.MappingFilter().IsMatch(tag.AliasFor))
                {
                    address = tag.AliasFor;
                    tag.AliasFor = "[I/O]";
                }

                var members = tag.Value.Members;

                if (!members.Any())
                {
                    table.AddRow(scope, tag.Name, tag.TagType, tag.AliasFor, tag.DataType, string.Empty,
                        tag.Description, tag.Value, address);

                    continue;
                }

                if (viewModel.ExcludeMemberTags)
                {
                    table.AddRow(scope, tag.Name, tag.TagType, tag.AliasFor, tag.DataType, string.Empty,
                        tag.Description, string.Empty, address);

                    continue;
                }

                foreach (var member in members)
                {
                    // Patch to cover exception when L5X has '$' in value field.
                    LogixData memberValue;
                    try
                    {
                        memberValue = member.Value;
                    }
                    catch (FormatException)
                    {
                        memberValue = "[ERROR]";
                    }

                    var comments = tag
                        .Comments?
                        .Where(c => c.Operand.Members.FirstOrDefault() == member.Name);

                    if (comments is null || !comments.Any() || comments.First().Operand.Contains("."))
                    {
                        table.AddRow(scope, $"{tag.Name}.{member.Name}", tag.TagType, tag.AliasFor, tag.DataType,
                            memberValue.Name, tag.Description, memberValue, address);

                        if (comments is null || !comments.Any())
                            continue;
                    }

                    foreach (var comment in comments)
                    {
                        table.AddRow(scope, $"{tag.Name}.{comment.Operand}", tag.TagType, tag.AliasFor, tag.DataType,
                            memberValue.Name, comment.Value, memberValue, address);
                    }
                }
            }
        }
    }
}
