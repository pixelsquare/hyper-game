using System.ComponentModel;
using Kumu.Extensions;
using UnityEngine.AddressableAssets;
using UnityEngine.Scripting;

namespace Kumu.Kulitan.SROptions
{
    public class CacheSrOptions : UbeSROptions
    {
        [Preserve]
        [Category("Addressables")]
        public void ClearAddressablesCache()
        {
            Addressables.ClearDependencyCacheAsync("default");
            "AddressablesDepedencyCache cleared!".Log();
        }
    }
}
