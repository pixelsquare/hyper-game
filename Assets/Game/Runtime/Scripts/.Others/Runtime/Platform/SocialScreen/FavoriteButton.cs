using System;
using Kumu.Kulitan.Backend;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class FavoriteButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImg;

        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite activeSprite;
        
        [SerializeField] private Transform favoriteLoadingIcon;

        private bool isFavorite;
        private string accountId;
        private Action callback;

        public void Initialize(string accountId, Action callback)
        {
            this.accountId = accountId;
            this.callback = callback;
        }

        public void SetIsFavorite(bool isFavorite)
        {
            this.isFavorite = isFavorite;
            buttonImg.sprite = isFavorite ? activeSprite : normalSprite;
        }

        private async void HandleButtonClick()
        {
            if (isFavorite)
            {
                var request = new RemoveFavoriteRequest()
                {
                    userId = accountId,
                };
                
                favoriteLoadingIcon.gameObject.SetActive(true);
                var result = await Services.SocialService.RemoveFavoriteAsync(request);
                favoriteLoadingIcon.gameObject.SetActive(false);

                if (!result.HasError)
                {
                    SetIsFavorite(false);
                    callback?.Invoke();
                }
            }
            else
            {
                var request = new SetFavoriteRequest()
                {
                    userId = accountId,
                };
                
                favoriteLoadingIcon.gameObject.SetActive(true);
                var result = await Services.SocialService.SetFavoriteAsync(request);
                favoriteLoadingIcon.gameObject.SetActive(false);

                if (!result.HasError)
                {
                    SetIsFavorite(true);
                    callback?.Invoke();
                }
            }
        }

        private void OnEnable()
        {
            button.onClick.AddListener(HandleButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(HandleButtonClick);
        }
    }
}
