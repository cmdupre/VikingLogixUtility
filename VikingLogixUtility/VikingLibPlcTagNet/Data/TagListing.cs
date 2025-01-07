using libplctag.NativeImport;
using VikingLibPlcTagNet.Settings;
using VikingLibPlcTagNet.Tags;

namespace VikingLibPlcTagNet.Data
{
    public sealed class TagListing : IDisposable
    {
        private readonly ITag tag;
        private readonly int payloadSize;

        private TagListing(ITag tag, int payloadSize)
        {
            this.tag = tag;
            this.payloadSize = payloadSize;
        }

        public void Dispose() => tag.Dispose();

        public static TagListing? Create(TagPath path, string name = "@tags")
        {
            var tag = TagFactory.GetTagFor(null, path, name);

            if (tag is null)
                return null;

            var payloadSize = plctag.plc_tag_get_size(tag.Id);

            if (payloadSize < 4)
            {
                tag.Dispose();
                return null;
            }

            return new TagListing(tag, payloadSize);
        }

        public static TagListing? CreateForProgram(TagPath path, string name) => 
            Create(path, $"Program:{name}.@tags");

        public int Id => tag.Id;

        public TagPath Path => tag.Path;

        public int PayloadSize => payloadSize;
    }
}
