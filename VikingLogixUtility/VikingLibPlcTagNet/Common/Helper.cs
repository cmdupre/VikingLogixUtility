using libplctag.NativeImport;
using VikingLibPlcTagNet.Interfaces;

namespace VikingLibPlcTagNet.Common
{
    public static class Helper
    {
        public static bool TryGetDataType(int value, out DataTypes? dataType)
        {
            if (Enum.IsDefined(typeof(DataTypes), value))
            {
                dataType = (DataTypes)value;
                return true;
            }

            dataType = null;
            return false;
        }

        public static void LogError(ILoggable? logger, string msg, object[] parms, int err = 0) =>
            logger?.Log($"{msg} ({string.Join(',', [.. parms])}) [{plctag.plc_tag_decode_error(err)}]");
    }
}
