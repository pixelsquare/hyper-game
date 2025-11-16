using System;
using System.Linq;
using System.Reflection;
using SRDebugger.Services;
using UnityEngine;

namespace Kumu.Kulitan.SROptions
{
    public class SROptionsContainerAdder : MonoBehaviour
    {
        private VersionDisplaySROptions versionDisplaySrOptions;
        private PlayerPrefsSROptions playerPrefsSrOptions;

        private UbeSROptions[] optionContainers;

        private IDebugService debugService => SRDebug.Instance;

        private void Start()
        {
            optionContainers = Assembly.GetExecutingAssembly()
                                       .GetTypes()
                                       .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(UbeSROptions)))
                                       .Select(t => (UbeSROptions)Activator.CreateInstance(t))
                                       .ToArray();

            foreach (var o in optionContainers)
            {
                debugService.AddOptionContainer(o);
            }
        }

        private void OnDestroy()
        {
            if (debugService == null)
            {
                return;
            }

            foreach (var o in optionContainers)
            {
                debugService.RemoveOptionContainer(o);
            }
        }
    }
}
