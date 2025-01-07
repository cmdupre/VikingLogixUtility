using libplctag.NativeImport;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Tags
{
    public sealed class TagReadonly : ITag
    {
        private readonly TagPath path;
        private readonly int id;
        private readonly string name;

        private TagReadonly(TagPath path, int id, string name)
        {
            this.path = path;
            this.id = id;
            this.name = name;
        }

        public void Dispose() => plctag.plc_tag_destroy(Id);

        internal static TagReadonly? Create(TagPath path, string name)
        {
            var id = plctag.plc_tag_create(path.WithFqn(name), path.Timeout);

            if (id < 0)
                return null;

            var result = plctag.plc_tag_read(id, path.Timeout);

            if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
                return null;

            return new TagReadonly(path, id, name);
        }

        public void Write(string value)
        {
            // NOP.
        }

        public void Read()
        {
            // NOP.
        }

        public void Toggle()
        {
            // NOP.
        }

        public TagPath Path => path;

        public int Id => id;

        public string FQN => name;

        public string? Value => null;

        public string? PreviousValue => null;

        public bool Changed => false;
    }
}
