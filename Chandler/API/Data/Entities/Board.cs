namespace Data.Entities
{
    public class Board : BaseEntity
    {
        protected Board() { }

        public Board(string tag, string title, string description, string bannerBase64, bool requireImages = false, bool isNSFW = false)
        {
            Tag = tag;
            Title = title;
            Description = description;
            BannerBase64 = bannerBase64;
            RequireImages = requireImages;
            IsNSFW = isNSFW;
        }

        public string Tag { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string BannerBase64 { get; private set; }

        public bool RequireImages { get; private set; }

        public bool IsNSFW { get; private set; }
    }
}
