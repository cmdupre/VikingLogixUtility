using VikingLibPlcTagNet.Interfaces;

namespace VikingLibPlcTagNet.Data
{
    public sealed class TagGroup : IDisposable
    {
        private readonly List<ITag> tags = [];

        public void Add(ITag? tag)
        {
            if (tag is null)
                return;

            tags.Add(tag);
        }

        public IEnumerable<ITag> Changed => tags.Where(t => t.Changed);

        public IEnumerable<ITag> NotChanged => tags.Where(t => !t.Changed);

        public bool AllChanged => !NotChanged.Any();

        public void ReadAllTags(ILoggable? logger = null)
        {
            foreach (var tag in tags)
                tag.Read(logger);
        }

        public void Dispose()
        {
            foreach (var tag in tags)
                tag.Dispose();
        }
    }
}
