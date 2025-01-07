using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Templates
{
    public sealed class Template
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
        private readonly IList<Field> fields;

        private Template(TagPath path, string name, int type,
            int tagSize, ushort templateId, uint memberDescriptionSize, uint instanceSize, ushort numMembers, ushort handle,
            IList<Field> fields)
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
        }

        public static Template Create(TagPath path, string name, int type,
            int tagSize, ushort templateId, uint memberDescriptionSize, uint instanceSize, ushort numMembers, ushort handle,
            IList<Field> fields)
        {
            return new Template(path, name, type,
                tagSize, templateId, memberDescriptionSize, instanceSize, numMembers, handle,
                fields);
        }

        public TagPath Path => path;

        public string Name => name;

        public int Type => type;

        public int TagSize => tagSize;

        public ushort TemplateId => templateId;

        public uint MemberDescriptionSize => memberDescriptionSize;

        public uint InstanceSize => instanceSize;

        public ushort NumMembers => numMembers;

        public ushort Handle => handle;

        public IList<Field> Fields => fields;
    }
}
