using System.Text.Json.Serialization;

namespace CreepStat.Models
{
    public class Hero
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("localized_name")]
        public string LocalizedName { get; set; }

        [JsonPropertyName("primary_attr")]
        public string PrimaryAttr { get; set; }

        [JsonPropertyName("attack_type")]
        public string AttackType { get; set; }

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }
    }
}
