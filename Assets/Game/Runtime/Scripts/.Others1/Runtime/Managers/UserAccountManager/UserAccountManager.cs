using Cysharp.Threading.Tasks;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class UserAccountManager : IUserAccountManager
    {
        private readonly IHeroLoadoutManager _heroLoadoutManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly ISaveManager _saveManager;

        [Inject]
        public UserAccountManager(IHeroLoadoutManager heroLoadoutManager, IInventoryManager inventoryManager, ISaveManager saveManager)
        {
            _heroLoadoutManager = heroLoadoutManager;
            _inventoryManager = inventoryManager;
            _saveManager = saveManager;
        }

        public async UniTask InitializeAsync()
        {
            await _heroLoadoutManager.InitializeAsync();
            await _inventoryManager.InitializeAsync();
            GameUtil.RecordFirstInstall(_saveManager);
        }
    }
}
