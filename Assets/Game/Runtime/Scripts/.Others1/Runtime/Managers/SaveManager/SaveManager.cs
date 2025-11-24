using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class SaveManager : ISaveManager
    {
        public string Load(string key)
        {
            return PlayerPrefs.GetString(key, string.Empty);
        }

        public void Save(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void ClearAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
