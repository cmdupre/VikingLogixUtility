using System.Text;
using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Data
{
    public sealed class TagInfo
    {
        private readonly TagPath path;
        private readonly ushort id;
        private readonly DataTypes type;
        private readonly ushort elementLength;
        private readonly uint[] arrayDims;
        private readonly string name;
        private readonly string programName;

        private TagInfo(TagPath path, ushort id, DataTypes type, ushort elementLength, uint[] arrayDims, string name, string programName = "")
        {
            this.path = path;
            this.id = id;
            this.type = type;
            this.elementLength = elementLength;
            this.arrayDims = arrayDims;
            this.name = name;
            this.programName = programName;
        }

        public static TagInfo Create(TagPath path, ushort id, DataTypes type, ushort elementLength, uint[] arrayDims, string name)
        {
            return new TagInfo(path, id, type, elementLength, arrayDims, name);
        }

        public static TagInfo Create(TagPath path, ushort id, DataTypes type, ushort elementLength, uint[] arrayDims, StringBuilder name)
        {
            return Create(path, id, type, elementLength, arrayDims, name.ToString());
        }

        public TagInfo WithProgramName(string programName) => 
            new(Path, Id, Type, ElementLength, ArrayDims, Name, programName);

        public TagPath Path => path;

        public ushort Id => id;
        
        public DataTypes Type => type;

        /// <summary>
        /// Only valid if IsUdt is true.
        /// </summary>
        public ushort TemplateId => IsUdt 
            ? (ushort)((ushort)Type & BitMasks.UdtId) 
            : (ushort)0;

        public bool IsUdt => ((ushort)type & BitMasks.StructBit) > 0;

        public ushort ElementLength => elementLength;
        
        public uint[] ArrayDims => arrayDims;
        
        public string Name => name;

        public string ProgramName => programName;
    }
}
