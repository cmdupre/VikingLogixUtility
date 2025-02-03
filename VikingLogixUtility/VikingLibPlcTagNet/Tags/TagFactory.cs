using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Data;
using VikingLibPlcTagNet.Interfaces;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Tags
{
    internal static class TagFactory
    {
        public static ITag? GetTagFor(ILoggable logger, TagInfo tagInfo) =>
            GetTagFor(logger, tagInfo.Type, tagInfo.Path, tagInfo.Name);

        public static ITag? GetTagFor(ILoggable logger, DataTypes? type, TagPath path, string fqn)
        {
            return type switch
            {
                DataTypes.BOOL => Tag<bool>.Create(logger, path, fqn),
                DataTypes.SINT => Tag<sbyte>.Create(logger, path, fqn),
                DataTypes.INT => Tag<short>.Create(logger, path, fqn),
                DataTypes.DINT => Tag<int>.Create(logger, path, fqn),
                DataTypes.LINT => Tag<long>.Create(logger, path, fqn),
                DataTypes.USINT => Tag<byte>.Create(logger, path, fqn),
                DataTypes.UINT => Tag<ushort>.Create(logger, path, fqn),
                DataTypes.UDINT => Tag<uint>.Create(logger, path, fqn),
                DataTypes.ULINT => Tag<ulong>.Create(logger, path, fqn),
                DataTypes.REAL => Tag<float>.Create(logger, path, fqn),
                DataTypes.LREAL => Tag<double>.Create(logger, path, fqn),
                DataTypes.TIME => Tag<DateTime>.Create(logger, path, fqn),
                // TODO: allow strings.
                //DataTypes.STRING => Tag<string>.Create(logger, path, fqn),
                _ => TagReadonly.Create(logger, path, fqn),
            };
        }
    }
}
