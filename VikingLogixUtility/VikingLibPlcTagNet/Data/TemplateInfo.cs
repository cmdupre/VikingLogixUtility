using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Data
{
    public sealed class TemplateInfo
    {
        private readonly TagPath path;
        private readonly string name;
        private readonly int type;
        private readonly int tagSize;
        private readonly ushort templateId;
        private readonly uint memberDescriptionSize;
        private readonly uint instanceSize;
        private readonly ushort numMembers;
        private readonly ushort handle;
        private readonly IList<FieldInfo> fields;
        private readonly string programName;

        private TemplateInfo(TagPath path, string name, int type,
            int tagSize, ushort templateId, uint memberDescriptionSize, uint instanceSize, ushort numMembers, ushort handle,
            IList<FieldInfo> fields,
            string programName = "")
        {
            this.path = path;
            this.name = name;
            this.type = type;
            this.tagSize = tagSize;
            this.templateId = templateId;
            this.memberDescriptionSize = memberDescriptionSize;
            this.instanceSize = instanceSize;
            this.numMembers = numMembers;
            this.handle = handle;
            this.fields = fields;
            this.programName = programName;
        }

        public static TemplateInfo Create(TagPath path, string name, int type,
            int tagSize, ushort templateId, uint memberDescriptionSize, uint instanceSize, ushort numMembers, ushort handle,
            IList<FieldInfo> fields)
        {
            return new TemplateInfo(path, name, type,
                tagSize, templateId, memberDescriptionSize, instanceSize, numMembers, handle,
                fields);
        }

        public TemplateInfo WithProgramName(string programName) =>
            new(Path, Name, Type, TagSize, TemplateId, MemberDescriptionSize, InstanceSize, NumMembers, Handle, Fields, programName);

        public TagPath Path => path;

        public string Name => name;

        public int Type => type;

        public int TagSize => tagSize;

        public ushort TemplateId => templateId;

        public uint MemberDescriptionSize => memberDescriptionSize;

        public uint InstanceSize => instanceSize;

        public ushort NumMembers => numMembers;

        public ushort Handle => handle;

        public IList<FieldInfo> Fields => fields;

        public string ProgramName => programName;
    }
}
