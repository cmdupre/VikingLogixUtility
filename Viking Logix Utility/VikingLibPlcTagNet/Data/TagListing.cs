using libplctag.NativeImport;
using VikingLibPlcTagNet.Settings;
using VikingLibPlcTagNet.Tags;

namespace VikingLibPlcTagNet.Data
{
    public sealed class TagListing : IDisposable
    {
        private readonly TagReadonly tag;
        private readonly int payloadSize;

        private TagListing(TagReadonly tag, int payloadSize)
        {
            this.tag = tag;
            this.payloadSize = payloadSize;
        }

        private static void Destroy(int id) => plctag.plc_tag_destroy(id);

        public void Dispose() => Destroy(tag.Id);

        public static TagListing? Create(TagPath path, string name = "@tags")
        {
            var tag = TagReadonly.Create(path, name);

            if (tag is null)
                return null;

            var payloadSize = plctag.plc_tag_get_size(tag.Id);

            if (payloadSize < 4)
            {
                Destroy(tag.Id);
                Console.Error.WriteLine($"Unexpectedly small payload size: {payloadSize}.");
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
