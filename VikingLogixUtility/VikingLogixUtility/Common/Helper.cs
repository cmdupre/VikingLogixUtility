using L5Sharp.Core;
using VikingLogixUtility.L5xApp.RegEx;
namespace VikingLogixUtility.Common
{
    internal static class Helper
    {
        /// <summary>
        /// Used to get addresses when I/O is mapped in rungs.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetMappingRungs(L5X content) => content
            .Query<Rung>()
            .Select(r => r.Text.ToString())
            .Where(t => RegularExpressions.MappingFilter().IsMatch(t));
    }
}
