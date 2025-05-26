namespace CreepStat.Models
{
    public class Stream
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public int ViewerCount { get; set; }
        public string StreamerId { get; set; }
        public string Language { get; set; }

        public string StreamerName { get; set; }
    }
}
