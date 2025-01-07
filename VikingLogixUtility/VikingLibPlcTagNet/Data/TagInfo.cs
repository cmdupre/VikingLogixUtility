using System.Text;
using VikingLibPlcTagNet.Common;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Data
{
    public sealed class TagInfo
    {
        private readonly TagPath path;
        private readonly uint id;
        private readonly DataTypes type;
        private readonly ushort elementLength;
        private readonly uint[] arrayDims;
        private readonly string name;
        private readonly string programName;

        private TagInfo(TagPath path, uint id, DataTypes type, ushort elementLength, uint[] arrayDims, string name, string programName = "")
        {
            this.path = path;
            this.id = id;
            this.type = type;
            this.elementLength = elementLength;
            this.arrayDims = arrayDims;
            this.name = name;
            this.programName = programName;
        }

        public static TagInfo Create(TagPath path, uint id, DataTypes type, ushort elementLength, uint[] arrayDims, string name)
        {
            return new TagInfo(path, id, type, elementLength, arrayDims, name);
        }

        public static TagInfo Create(TagPath path, uint id, DataTypes type, ushort elementLength, uint[] arrayDims, StringBuilder name)
        {
            return Create(path, id, type, elementLength, arrayDims, name.ToString());
        }

        public TagInfo WithProgramName(string programName) => 
            new(Path, Id, Type, ElementLength, ArrayDims, Name, programName);

        public TagPath Path => path;

        public uint Id => id;
        
        public DataTypes Type => type;

        public ushort NumberDimensions => 
            (ushort)(((ushort)Type & BitMasks.Dim) >> 13);

        /// <summary>
        /// Only valid if IsUdt is true.
        /// </summary>
        public ushort TemplateId => IsUdt 
            ? (ushort)((ushort)Type & BitMasks.UdtId) 
            : default;

        public bool IsUdt => ((ushort)type & BitMasks.StructBit) > 0;

        public ushort ElementLength => elementLength;
        
        public uint[] ArrayDims => arrayDims;
        
        public string Name => name;

        public string ProgramName => programName;

        public uint GetElementCount()
        {
            // TODO: Untested code from list tags example.

            uint count = 1;

            for (int i = 0; i < NumberDimensions; i++)
                count += arrayDims[i];

            return count;
        }
    }
}
