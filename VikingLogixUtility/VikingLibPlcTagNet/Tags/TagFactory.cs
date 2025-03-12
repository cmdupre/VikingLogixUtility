using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Interfaces;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Tags
{
    internal static class TagFactory
    {
        public static ITag? GetTagFor(DataTypes? type, TagPath path, string fqn, ILoggable? logger = null)
        {
            if (type is null)
                return TagReadonly.Create(path, fqn, logger);

            var tag = GetTag(type, path, fqn, logger);

            if (tag is TagReadonly)
            {
                Helper.LogError(logger, "Unknown data type for tag", [type, fqn]);
                return null;
            }

            return tag;
        }

        private static ITag? GetTag(DataTypes? type, TagPath path, string fqn, ILoggable? logger = null)
        {
            return type switch
            {
                DataTypes.BOOL => Tag<bool>.Create(path, fqn, logger),
                DataTypes.SINT => Tag<sbyte>.Create(path, fqn, logger),
                DataTypes.INT => Tag<short>.Create(path, fqn, logger),
                DataTypes.DINT => Tag<int>.Create(path, fqn, logger),
                DataTypes.LINT => Tag<long>.Create(path, fqn, logger),
                DataTypes.USINT => Tag<byte>.Create(path, fqn, logger),
                DataTypes.UINT => Tag<ushort>.Create(path, fqn, logger),
                DataTypes.UDINT => Tag<uint>.Create(path, fqn, logger),
                DataTypes.ULINT => Tag<ulong>.Create(path, fqn, logger),
                DataTypes.REAL => Tag<float>.Create(path, fqn, logger),
                DataTypes.LREAL => Tag<double>.Create(path, fqn, logger),
                DataTypes.TIME => Tag<DateTime>.Create(path, fqn, logger),
                DataTypes.STRING => Tag<string>.Create(path, fqn, logger),
                DataTypes.STRINGE => Tag<string>.Create(path, fqn, logger),
                DataTypes.CHAR => Tag<char>.Create(path, fqn, logger),
                _ => TagReadonly.Create(path, fqn, logger)
            };
        }
    }
}
