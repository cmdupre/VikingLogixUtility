using System.IO;

namespace VikingLogixUtility.Extensions
{
    internal static class StringExtensions
    {
        public static string WithXlsxExtension(this string s)
        {
            var pathname = Path.GetDirectoryName(s);

            var filename = Path
                .GetFileName(s)
                .Replace(".", "_")
                + ".xlsx";

            return string.IsNullOrEmpty(pathname)
                ? filename
                : pathname + Path.DirectorySeparatorChar + filename;
        }
    }
}
