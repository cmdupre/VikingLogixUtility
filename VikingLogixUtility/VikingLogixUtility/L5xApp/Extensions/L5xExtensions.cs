using L5Sharp.Core;
using VikingLogixUtility.L5xApp.RegEx;

namespace VikingLogixUtility.L5xApp.Extensions
{
    internal static class L5xExtensions
    {
        /// <summary>
        /// Used to get addresses when I/O is mapped in rungs.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetMappingRungs(this L5X content) => content
            .Query<Rung>()
            .Select(r => r.Text.ToString())
            .Where(t => RegularExpressions.MappingFilter().IsMatch(t));
    }
}
