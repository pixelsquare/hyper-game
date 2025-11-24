using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Zenject;

namespace Santelmo.Rinsurv
{
    using SaveKey = GameConstants.SaveKeys;

    public class HeroLoadoutManager : IHeroLoadoutManager
    {
        private string ActiveHeroId
        {
            get => _saveManager.Load(SaveKey.ActiveHeroKey);
            set => _saveManager.Save(SaveKey.ActiveHeroKey, value);
        }

        private readonly HeroDatabase _heroDatabase;
        private readonly LoadoutModule _loadoutModule;
        private readonly ISaveManager _saveManager;

        [Inject]
        public HeroLoadoutManager(HeroDatabase heroDatabase, LoadoutModule loadoutModule, ISaveManager saveManager)
        {
            _heroDatabase = heroDatabase;
            _loadoutModule = loadoutModule;
            _saveManager = saveManager;
            ActiveHero = _heroDatabase.Heroes.First(x => x != null);
        }

        public Hero[] Heroes => _heroDatabase.Heroes.ToArray();

        public Hero ActiveHero { get; private set; }

        public async UniTask InitializeAsync()
        {
            ReadLocalData();

            if (GameUtil.ShouldSyncUp(_saveManager))
            {
                FillDefaultItems();
                await SyncUpAsync();
                return;
            }

            await SyncDownAsync();
            SetActiveHero(_heroDatabase.GetHero(ActiveHeroId) ?? _heroDatabase.Heroes.First());
        }

        public void WriteLocalData()
        {
            var heroItemEquippedMap = _heroDatabase.Heroes.Select(x => x).ToDictionary(x => x.Id, y => y.ItemEquipped);
            _saveManager.Save(SaveKey.HeroItemEquippedKey, JsonConvert.SerializeObject(heroItemEquippedMap));
        }

        public void SetActiveHero(Hero hero)
        {
            ActiveHero = hero;
            ActiveHeroId = ActiveHero.Id;
        }

        private async UniTask SyncUpAsync()
        {
            // FIX: [TONY] Should be placed and initialized on backend ideally.
            await _loadoutModule.WriteItemCountsAsync();
        }

        private async UniTask SyncDownAsync()
        {
            await _loadoutModule.ReadFromCloudAsync();
        }

        private void FillDefaultItems()
        {
            // TODO: Default items should be placed here.
            var heroLoadouts = from hero in _heroDatabase.Heroes
                               let loadout = new Loadout { Hero = hero }
                               select loadout.ToLoadoutData();

            _loadoutModule.Initialize(heroLoadouts);
        }

        private void ReadLocalData()
        {
            var itemEquipped = _saveManager.Load(SaveKey.HeroItemEquippedKey);

            if (!string.IsNullOrEmpty(itemEquipped))
            {
                var heroItemEquippedMap = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(itemEquipped);

                foreach (var heroItem in heroItemEquippedMap)
                {
                    var hero = _heroDatabase.GetHero(heroItem.Key);
                    hero.ItemEquipped = heroItem.Value;
                }
            }
        }
    }
}
