using Kumu.Kulitan.Backend;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class UserDetailsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameLabel;
        [SerializeField] private TMP_Text playerUserNameLabel;

        public void Initialize(UserProfile userProfile, SocialState socialState)
        {
            playerNameLabel.text = userProfile.nickName;
            playerUserNameLabel.text = userProfile.userName;
        }
    }
}
