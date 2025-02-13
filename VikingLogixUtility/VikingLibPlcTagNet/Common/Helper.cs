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
    }
}
