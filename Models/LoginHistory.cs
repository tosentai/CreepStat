using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CreepStat.Models
{
    public class LoginHistory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string SteamID { get; set; }

        public DateTime LoginTime { get; set; }

        public User User { get; set; }
    }

}
