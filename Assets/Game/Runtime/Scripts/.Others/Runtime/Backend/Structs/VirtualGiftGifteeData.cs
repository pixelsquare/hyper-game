using System;

namespace Kumu.Kulitan.Gifting
{
    [Serializable]
    public struct VirtualGiftGifteeData
    {
        /// <summary>
        /// GIftee id.
        /// </summary>
        public string id;
        /// <summary>
        /// Giftee nickname.
        /// </summary>
        public string nickname;
        /// <summary>
        /// Giftee username.
        /// </summary>
        public string username;
    }
}
