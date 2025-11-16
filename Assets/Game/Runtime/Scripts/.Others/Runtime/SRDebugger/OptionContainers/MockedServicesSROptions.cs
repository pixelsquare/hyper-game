using System.ComponentModel;
using Kumu.Kulitan.Backend;
using UnityEngine.Scripting;

namespace Kumu.Kulitan.SROptions
{
    public class MockedServicesSROptions : UbeSROptions
    {
        [Preserve]
        [Category("Mocked Services")]
        public void ClearUserProfile()
        {
            MockedServicesUtil.ClearMockedProfileInPrefs();
        }
    }
}
