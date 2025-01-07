using System.Text;
using VikingLibPlcTagNet.Common;

namespace VikingLibPlcTagNet.Data
{
    public sealed class FieldInfo
    {
        private readonly string name;
        private readonly ushort metadata;
        private readonly DataTypes type;
        private readonly uint offset;

        private FieldInfo(string name, ushort metadata, DataTypes type, uint offset)
        {
            this.name = name;
            this.metadata = metadata;
            this.type = type;
            this.offset = offset;
        }

        public static FieldInfo Create(ushort metadata, ushort type, uint offset, string name = "")
        {
            if (!Enum.TryParse($"{type}", out DataTypes dataType))
                dataType = 0;

            return new(name, metadata, dataType, offset);
        }

        public FieldInfo WithName(StringBuilder name) => WithName(name.ToString());

        public FieldInfo WithName(string name) => Create(Metadata, (ushort)Type, Offset, name);

        public string Name => name;

        public ushort Metadata => metadata;

        public DataTypes Type => type;

        public uint Offset => offset;

        public bool IsUdt => ((ushort)type & BitMasks.StructBit) > 0;

        /// <summary>
        /// Only valid if IsUdt is true.
        /// </summary>
        public ushort TemplateId => IsUdt
            ? (ushort)((ushort)Type & BitMasks.UdtId)
            : default;
    }
}
