using libplctag.NativeImport;
using VikingLibPlcTagNet.Settings;

namespace VikingLibPlcTagNet.Tags
{
    public sealed class TagReadonly : IDisposable
    {
        private readonly TagPath path;
        private readonly int id;

        private TagReadonly(TagPath path, int id)
        {
            this.path = path;
            this.id = id;
        }

        public void Dispose() => plctag.plc_tag_destroy(Id);

        public static TagReadonly? Create(TagPath path, string name)
        {
            var id = plctag.plc_tag_create(path.WithName(name), path.Timeout);

            if (id < 0)
            {
                Console.Error.WriteLine(plctag.plc_tag_decode_error(id));
                return null;
            }

            var result = plctag.plc_tag_read(id, path.Timeout);

            if (result != (int)STATUS_CODES.PLCTAG_STATUS_OK)
            {
                Console.Error.WriteLine(plctag.plc_tag_decode_error(result));
                return null;
            }

            return new TagReadonly(path, id);
        }

        public TagPath Path => path;

        public int Id => id;
    }
}
