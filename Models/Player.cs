namespace CreepStat.Models
{
    public class PlayerProfile
    {
    public int? solo_competitive_rank { get; set; }
    public int? competitive_rank { get; set; }
    public int? rank_tier { get; set; }
    public int? leaderboard_rank { get; set; }
    public Profile profile { get; set; }
    }

    public class Profile
    {
        public int account_id { get; set; }
        public string personaname { get; set; }
        public string name { get; set; }
        public bool plus { get; set; }
        public int cheese { get; set; }
        public string steamid { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public string profileurl { get; set; }
        public string last_login { get; set; }
        public string loccountrycode { get; set; }
        public bool is_contributor { get; set; }
        public bool is_subscriber { get; set; }
    }
}
