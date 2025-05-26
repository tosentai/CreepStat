using System.ComponentModel.DataAnnotations;

namespace CreepStat.Models
{
    public class User
    {
        [Key]
        public string SteamID { get; set; }

        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Role { get; set; } 

        public List<LoginHistory> LoginHistories { get; set; } = new();
    }

}
