using System.IO;
using System.Text;
using VikingLogixUtility.Common;

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

        /// <summary>
        /// Removes artifacts from a grid column name.
        /// For example, removes " (W)" and double underscores from writeable column name.
        /// </summary>
        /// <param name="gridColumnName"></param>
        /// <returns></returns>
        public static string Raw(this string s) => s
            .Replace("__", "_")
            .Replace(Constants.Read, "")
            .Replace(Constants.Write, "")
            .TrimEnd();

        public static string SanitizeFilterText(this string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
                if ((c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    (c >= '0' && c <= '9') ||
                    (c == '-') ||
                    (c == '_'))
                    sb.Append(c);

            return sb.ToString();
        }
    }
}
