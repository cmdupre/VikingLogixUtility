namespace VikingLibPlcTagNet.Settings
{
    /// <summary>
    /// Communications Path settings to connect to PLC.
    /// Supported:
    ///     1. AB Control/Compact Logix
    /// </summary>
    /// <param name="gateway"></param>
    /// <param name="timeout"></param>
    public sealed class TagPath(string gateway, int slot, int timeout = 9999)
    {
        public static string Protocol => "ab-eip";

        public static string PLC => "controllogix";

        public int Timeout => timeout;

        public int Slot => slot;

        public string Path => $"1,{Slot}";

        public string Gateway => gateway;

        /// <summary>
        /// Get attribute/connection string for a fully qualified name (tag).
        /// </summary>
        /// <param name="fqn"></param>
        /// <returns></returns>
        public string GetAttributeString(string fqn) =>
            $"protocol={Protocol}&gateway={Gateway}&path={Path}&plc={PLC}&name={fqn}";
    }
}
