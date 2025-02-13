namespace VikingLibPlcTagNet.Common
{
    public enum DataTypes
    {
        BOOL = 0xC1,
        SINT = 0xC2,
        INT = 0xC3,
        DINT = 0xC4,
        LINT = 0xC5,
        USINT = 0xC6,
        UINT = 0xC7,
        UDINT = 0xC8,
        ULINT = 0xC9,
        REAL = 0xCA,
        LREAL = 0xCB,
        STRING = 0x8FCE,
        TIME = 0xD7,
        //LTIME = 0xDF,
        //STIME = 0xCC,
        //DATE = 0xCD,
        //TIME_OF_DAY = 0xCE,
        //DATE_AND_TIME = 0xCF,
        //BYTE = 0xD1,
        //WORD = 0xD2,
        DWORD = 0xD3,
        DWORD_ARRAY = 0x20D3,
        //LWORD = 0xD4,
        //STRING2 = 0xD5,
        //FTIME = 0xD6,
        //ITIME = 0xD8,
        //STRINGN = 0xD9,
        //SHORT_STRING = 0xDA,
        //EPATH = 0xDC,
        //ENGUNIT = 0xDD,
        //STRINGI = 0xDE
    }
}
