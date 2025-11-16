using System;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [Serializable]
    public class FeedbackInputData : MonoBehaviour
    {
        private string email;
        private string wirelessHeadphones;
        private string similarApp;
        private string crashes;
        private string comments;
        private string enjoyUbeRating;
        private string graphicsRating;
        private string intuitiveRating;
        private string avatarDesignRating;
        private string joinRoomsRating;
        private string moveAvatarRating;
        private string emotesRating;
        private string voiceChatRating;
        private string roomDesignRating;
        private string phonePerformanceRating;
        private string ubeSatisfactionRating;
        private string nda;

        public string Email
        {
            get => email;
            set => email = value;
        }

        public string WirelessHeadphones
        {
            get => wirelessHeadphones;
            set => wirelessHeadphones = value;
        }
        
        public string SimilarApp
        {
            get => similarApp;
            set => similarApp = value;
        }

        public string Crashes
        {
            get => crashes;
            set => crashes = value;
        }

        public string Comments
        {
            get => comments;
            set => comments = value;
        }

        public string EnjoyUbeRating
        {
            get => enjoyUbeRating;
            set => enjoyUbeRating = value;
        }

        public string GraphicsRating
        {
            get => graphicsRating;
            set => graphicsRating = value;
        }

        public string IntuitiveRating
        {
            get => intuitiveRating;
            set => intuitiveRating = value;
        }

        public string AvatarDesignRating
        {
            get => avatarDesignRating;
            set => avatarDesignRating = value;
        }

        public string JoinRoomsRating
        {
            get => joinRoomsRating;
            set => joinRoomsRating = value;
        }

        public string MoveAvatarRating
        {
            get => moveAvatarRating;
            set => moveAvatarRating = value;
        }

        public string EmotesRating
        {
            get => emotesRating;
            set => emotesRating = value;
        }

        public string VoiceChatRating
        {
            get => voiceChatRating;
            set => voiceChatRating = value;
        }

        public string RoomDesignRating
        {
            get => roomDesignRating;
            set => roomDesignRating = value;
        }

        public string PhonePerformanceRating
        {
            get => phonePerformanceRating;
            set => phonePerformanceRating = value;
        }

        public string UbeSatisfactionRating
        {
            get => ubeSatisfactionRating;
            set => ubeSatisfactionRating = value;
        }

        public string Nda
        {
            get => nda;
            set => nda = value;
        }
    }
}
