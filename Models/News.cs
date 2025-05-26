using System;
using System.ComponentModel.DataAnnotations;

namespace CreepStat.Models
{
    public class News
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}
