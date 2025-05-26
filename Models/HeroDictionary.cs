namespace CreepStat.Models
{
    public class HeroDictionary
    {
        private Dictionary<int, string> _heroDictionary;

        public HeroDictionary(List<Hero> heroes)
        {
            _heroDictionary = heroes.ToDictionary(hero => hero.Id, hero => hero.LocalizedName);
        }

        public string GetHeroName(int heroId)
        {
            return _heroDictionary.TryGetValue(heroId, out var name) ? name : "Unknown Hero";
        }

        public Dictionary<int, string> GetDictionary()
        {
            return _heroDictionary;
        }
    }
}
