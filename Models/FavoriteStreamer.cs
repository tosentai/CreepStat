using System.ComponentModel.DataAnnotations;

namespace CreepStat.Models
{
    public class FavoriteStreamer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string StreamerId { get; set; }

        [Required]
        public string StreamerName { get; set; }

        public string StreamerDisplayName { get; set; }

        public string ProfileImageUrl { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
