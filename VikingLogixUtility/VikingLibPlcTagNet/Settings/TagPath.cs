namespace VikingLibPlcTagNet.Settings
{
    /// <summary>
    /// Communications Path settings to connect to PLC.
    /// Supported:
    ///     1. AB Control/Compact Logix
    /// </summary>
    /// <param name="gateway"></param>
    /// <param name="timeout"></param>
    public sealed class TagPath(string gateway, int timeout = 9999)
    {
        public int Timeout => timeout;

        public string Gateway => gateway;

        public static string Protocol => "ab-eip";

        public static string Path => "1,0";

        public static string PLC => "controllogix";

        /// <summary>
        /// Fully Qualified Name
        /// </summary>
        /// <param name="fqn"></param>
        /// <returns></returns>
        public string WithFqn(string fqn) =>
            $"protocol={Protocol}&gateway={Gateway}&path={Path}&plc={PLC}&name={fqn}";
    }
}
