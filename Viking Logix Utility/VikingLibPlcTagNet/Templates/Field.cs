using System.Text;
using VikingLibPlcTagNet.Common;

namespace VikingLibPlcTagNet.Templates
{
    public sealed class Field
    {
        private readonly string name;
        private readonly ushort metadata;
        private readonly DataTypes type;
        private readonly uint offset;

        private Field(string name, ushort metadata, DataTypes type, uint offset)
        {
            this.name = name;
            this.metadata = metadata;
            this.type = type;
            this.offset = offset;
        }

        public static Field Create(ushort metadata, ushort type, uint offset, string name = "")
        {
            if (!Enum.TryParse($"{type}", out DataTypes dataType))
                dataType = 0;

            return new(name, metadata, dataType, offset);
        }

        public Field WithName(StringBuilder name) => WithName(name.ToString());

        public Field WithName(string name) => Create(Metadata, (ushort)Type, Offset, name);

        public string Name => name;

        public ushort Metadata => metadata;

        public DataTypes Type => type;

        public uint Offset => offset;
    }
}
