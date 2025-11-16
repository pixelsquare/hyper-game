using System;

namespace Kumu.Kulitan.Common
{
    public static class ShopData
    {
        private static bool isInitialized;
        
        public static event Action OnValuesUpdated;

        public static bool IsInitialized
        {
            get => isInitialized;
            set // called by Visual Scripting
            {
                isInitialized = value;
                OnValuesUpdated?.Invoke();
            }
        }
    }
}
