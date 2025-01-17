using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Data;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Tags
{
    internal static class TagFactory
    {
        public static ITag? GetTagFor(TagInfo tagInfo) =>
            GetTagFor(tagInfo.Type, tagInfo.Path, tagInfo.Name);

        public static ITag? GetTagFor(DataTypes? type, TagPath path, string fqn)
        {
            return type switch
            {
                DataTypes.BOOL => Tag<bool>.Create(path, fqn),
                DataTypes.SINT => Tag<sbyte>.Create(path, fqn),
                DataTypes.INT => Tag<short>.Create(path, fqn),
                DataTypes.DINT => Tag<int>.Create(path, fqn),
                DataTypes.LINT => Tag<long>.Create(path, fqn),
                DataTypes.USINT => Tag<byte>.Create(path, fqn),
                DataTypes.UINT => Tag<ushort>.Create(path, fqn),
                DataTypes.UDINT => Tag<uint>.Create(path, fqn),
                DataTypes.ULINT => Tag<ulong>.Create(path, fqn),
                DataTypes.REAL => Tag<float>.Create(path, fqn),
                DataTypes.LREAL => Tag<double>.Create(path, fqn),
                DataTypes.TIME => Tag<DateTime>.Create(path,fqn),
                DataTypes.STRING => Tag<string>.Create(path, fqn),
                _ => TagReadonly.Create(path, fqn),
            };
        }
    }
}
