using VikingLibPlcTagNet.Data;
using VikingLibPlcTagNet.Settings;
using VikingLibPlcTagNet.Tags;

namespace VikingLibPlcTagNet.Common
{
    internal static class DataTypesConverter
    {
        public static ITag? GetTagFor(TagInfo tagInfo) =>
            GetTagFor(tagInfo.Type, tagInfo.Path, tagInfo.Name);

        public static ITag? GetTagFor(DataTypes type, TagPath path, string name)
        {
            return type switch
            {
                DataTypes.BOOL => Tag<bool>.Create(path, name),
                DataTypes.SINT => Tag<sbyte>.Create(path, name),
                DataTypes.INT => Tag<short>.Create(path, name),
                DataTypes.DINT => Tag<int>.Create(path, name),
                DataTypes.LINT => Tag<long>.Create(path, name),
                DataTypes.USINT => Tag<byte>.Create(path, name),
                DataTypes.UINT => Tag<ushort>.Create(path, name),
                DataTypes.UDINT => Tag<uint>.Create(path, name),
                DataTypes.ULINT => Tag<ulong>.Create(path, name),
                DataTypes.REAL => Tag<float>.Create(path, name),
                DataTypes.LREAL => Tag<double>.Create(path, name),
                DataTypes.STRING => Tag<string>.Create(path, name),
                _ => null,
            };
        }
    }
}
