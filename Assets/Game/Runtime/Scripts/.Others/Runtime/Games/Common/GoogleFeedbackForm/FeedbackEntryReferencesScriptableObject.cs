using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Feedback Form Entry References")]

    public class FeedbackEntryReferencesScriptableObject : ScriptableObject
    {
        [Header("Short Answer Entry IDs")]
        [SerializeField] private string formBaseURL;
        [SerializeField] private string kumuEmailEntryId;
        [SerializeField] private string wirelessHeadphonesEntryId;
        [SerializeField] private string similarAppEntryId;
        [SerializeField] private string crashesEntryId;
        [SerializeField] private string commentsEntryId;
        [Header("Linear Scale Entry IDs")]
        [SerializeField] private string enjoyPlayingUbeEntryId;
        [SerializeField] private string likeGraphicsUbeEntryId;
        [SerializeField] private string intuitiveEntryId;
        [SerializeField] private string avatarDesignEntryId;
        [SerializeField] private string joinRoomsEntryId;
        [SerializeField] private string moveAvatarEntryId;
        [SerializeField] private string emotesEntryId;
        [SerializeField] private string voiceChatEntryId;
        [SerializeField] private string roomDesignEntryId;
        [SerializeField] private string phonePerformanceEntryId;
        [SerializeField] private string ubeSatisfactionEntryId;
        [Header("Device Info Entry IDs")]
        [SerializeField] private string deviceModelEntryId;
        [SerializeField] private string operatingSystemEntryId;
        [SerializeField] private string systemMemorySizeEntryId;
        [SerializeField] private string graphicsDeviceNameEntryId;
        [SerializeField] private string graphicsMemorySizeEntryId;
        [SerializeField] private string graphicsShaderLevelEntryId;
        [Header("NDA entry ID")]
        [SerializeField] private string nda;

        // Questions
        public string FormBaseURL => formBaseURL;
        public string KumuEmailEntryId => kumuEmailEntryId;
        public string WirelessHeadphonesEntryId => wirelessHeadphonesEntryId;
        public string SimilarAppEntryId => similarAppEntryId;
        public string CrashesEntryId => crashesEntryId;
        public string CommentsEntryId => commentsEntryId;
        // Ratings
        public string EnjoyPlayingUbeEntryId => enjoyPlayingUbeEntryId;
        public string LikeGraphicsUbeEntryId => likeGraphicsUbeEntryId;
        public string IntuitiveEntryId => intuitiveEntryId;
        public string AvatarDesignEntryId => avatarDesignEntryId;
        public string JoinRoomsEntryId => joinRoomsEntryId;
        public string MoveAvatarEntryId => moveAvatarEntryId;
        public string EmotesEntryId => emotesEntryId;
        public string VoiceChatEntryId => voiceChatEntryId;
        public string RoomDesignEntryId => roomDesignEntryId;
        public string PhonePerformanceEntryId => phonePerformanceEntryId;
        public string UbeSatisfactionEntryId => ubeSatisfactionEntryId;
        // Device Data
        public string DeviceModelEntryId => deviceModelEntryId;
        public string OperatingSystemEntryId => operatingSystemEntryId;
        public string SystemMemorySizeEntryId => systemMemorySizeEntryId;
        public string GraphicsDeviceNameEntryId => graphicsDeviceNameEntryId;
        public string GraphicsMemorySizeEntryId => graphicsMemorySizeEntryId;
        public string GraphicsShaderLevelEntryId => graphicsShaderLevelEntryId;
        //NDA
        public string Nda => nda;
    }
}
