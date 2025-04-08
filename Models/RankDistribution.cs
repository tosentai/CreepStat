using System.Text.Json.Serialization;

namespace CreepStat.Models
{
    public class RankDistribution
    {
        [JsonPropertyName("ranks")]
        public Ranks Ranks { get; set; }

        [JsonPropertyName("sum")]
        public Sum Sum { get; set; }
    }

    public class Ranks
    {
        [JsonPropertyName("rows")]
        public List<RankBin> Rows { get; set; }
    }

    public class RankBin
    {
        [JsonPropertyName("bin")]
        public int Bin { get; set; }

        [JsonPropertyName("bin_name")]
        public int BinName { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("cumulative_sum")]
        public int CumulativeSum { get; set; }
    }

    public class Sum
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
