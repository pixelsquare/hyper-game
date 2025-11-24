using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ItemDataModule : IItemDataModule
    {
        private readonly IReadOnlyDictionary<string, IItem> itemsMap;
        private readonly IReadOnlyDictionary<string, Hero> heroesMap;
        private readonly IReadOnlyDictionary<string, Weapon> weaponsMap;
        private readonly IReadOnlyDictionary<string, EmblemChange> emblemsChangeMap;
        private readonly IReadOnlyDictionary<string, EmblemDeparture> emblemsDepartureMap;
        private readonly IReadOnlyDictionary<string, EmblemPursuit> emblemsPursuitMap;
        private readonly IReadOnlyDictionary<string, Relic> relicsMap;

        [Inject]
        public ItemDataModule(HeroDatabase heroDatabase, ItemDatabase itemDatabase)
        {
            itemsMap = itemDatabase.AllItems.ToDictionary(x => x.Id, y => y);
            heroesMap = heroDatabase.Heroes.ToDictionary(x => x.Id, y => y);
            weaponsMap = itemDatabase.GetItems<Weapon>().ToDictionary(x => x.Id, y => y);
            emblemsChangeMap = itemDatabase.GetItems<EmblemChange>().ToDictionary(x => x.Id, y => y);
            emblemsDepartureMap = itemDatabase.GetItems<EmblemDeparture>().ToDictionary(x => x.Id, y => y);
            emblemsPursuitMap = itemDatabase.GetItems<EmblemPursuit>().ToDictionary(x => x.Id, y => y);
            relicsMap = itemDatabase.GetItems<Relic>().ToDictionary(x => x.Id, y => y);
        }

        public IItem GetItemFromId(string id) => itemsMap.TryGetValue(id, out var item) ? item : null;
        public Hero GetHeroFromId(string id) => heroesMap.TryGetValue(id, out var item) ? item : null;
        public Weapon GetWeaponFromId(string id) => weaponsMap.TryGetValue(id, out var item) ? item : null;
        public EmblemChange GetEmblemChange(string id) => emblemsChangeMap.TryGetValue(id, out var item) ? item : null;
        public EmblemDeparture GetEmblemDeparture(string id) => emblemsDepartureMap.TryGetValue(id, out var item) ? item : null;
        public EmblemPursuit GetEmblemPursuit(string id) => emblemsPursuitMap.TryGetValue(id, out var item) ? item : null;
    }
}
