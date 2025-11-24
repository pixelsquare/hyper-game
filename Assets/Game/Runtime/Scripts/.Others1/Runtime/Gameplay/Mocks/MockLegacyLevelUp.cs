using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class MockLegacyLevelUp : MonoBehaviour
    {
        public string _legacyToUpgrade;
        
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.L) && !string.IsNullOrEmpty(_legacyToUpgrade))
            {
                var childLegacyTrans = transform.Find(_legacyToUpgrade);

                if (childLegacyTrans != null)
                {
                    var legacy = childLegacyTrans.GetComponent<ILegacy>();

                    if (legacy.CurrentLevel == legacy.MaxLevel)
                    {
                        return;
                    }
                    
                    legacy.LevelUp();
                }
            }
        }
    }
}
