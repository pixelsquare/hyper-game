using System.ComponentModel;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.Scripting;

namespace Kumu.Kulitan.SROptions
{
    public class PlayerPrefsSROptions : UbeSROptions
    {
        [Preserve]
        [Category("PlayerPrefs")]
        public void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [Preserve]
        [Category("PlayerPrefs")]
        public void CheckFirebaseIds()
        {
            LogPrefString("gmp_app_id");
            LogPrefString("admob_app_id");
        }
        
        [Preserve]
        [Category("Tutorials")]
        public void ResetTutorialsViewed()
        {
            Tutorial.ResetViewed();
        }

        private void LogPrefString(string id)
        {
            var value = PlayerPrefs.GetString(id, "empty");
            $"<color=#ffff00>{id}: {value}</color>".Log();
        }
    }
}
