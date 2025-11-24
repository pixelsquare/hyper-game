using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class HeroLoadoutEquippedItem : MonoBehaviour
    {
        [SerializeField] private ItemButton[] _itemEquipButton;

        [Inject] private ItemDatabase _itemDatabase;
        [Inject] private IHudManager _hudManager;
        [Inject] private IHeroLoadoutManager _heroLoadoutManager;

        private void UpdateItemButtons(string[] itemEquippedIds)
        {
            var itemIds = itemEquippedIds.Select((itemId, idx) => (itemId, idx));

            foreach (var (itemId, idx) in itemIds)
            {
                var item = _itemDatabase.GetItem(itemId);
                var itemButton = _itemEquipButton[idx];
                itemButton.Setup(item, () => HandleButtonClicked(idx, itemButton));
            }
        }

        private async void HandleButtonClicked(int index, ItemButton button)
        {
            var item = await _hudManager.ShowHudAsync<ItemEquipHud>(HudType.ItemEquip)
                                        .Setup(button.Item)
                                        .Task;

            if (item != null)
            {
                button.Setup(item, () => HandleButtonClicked(index, button));

                var hero = _heroLoadoutManager.ActiveHero;
                hero.ItemEquipped[index] = item?.Id ?? "";
                _heroLoadoutManager.WriteLocalData();
            }

            await UniTask.NextFrame();
            button.SetButtonSelected(false);
        }

        private void HandleActiveHeroSelected(IMessage message)
        {
            if (message.Data is not Hero hero)
            {
                return;
            }

            UpdateItemButtons(hero.ItemEquipped);
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnActiveHeroSelected, HandleActiveHeroSelected);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(UIEvent.OnActiveHeroSelected, HandleActiveHeroSelected, true);
        }

        private void Start()
        {
            UpdateItemButtons(_heroLoadoutManager.ActiveHero.ItemEquipped);
        }
    }
}
