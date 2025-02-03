namespace VikingLibPlcTagNet.Common
{
    public static class BitMasks
    {
        public static ushort StructBit => 0x8000;
        public static ushort SystemBit => 0x1000;
        public static ushort UdtId => 0xFFF;
        public static ushort Dim => 0x6000;
    }
}
