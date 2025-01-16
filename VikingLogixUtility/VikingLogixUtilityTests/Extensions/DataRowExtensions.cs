using System.Data;

namespace VikingLogixUtilityTests.Extensions
{
    internal static class DataRowExtensions
    {
        public static string AsString(this DataRow row)
        {
            return $"{row["Scope"]},{row["Name"]},{row["TagType"]}," +
                $"{row["AliasFor"]},{row["DataType"]},{row["MemberDataType"]}," +
                $"{row["Description"]},{row["Value"]},{row["Address"]}";
        }
    }
}
