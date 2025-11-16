using Kumu.Extensions;
using SRDebugger.Editor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class SRDebuggerDisabler : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        
        public void OnPreprocessBuild(BuildReport report)
        {
            var identifier = Application.identifier;
            
            var enable = identifier.EndsWith("dev") || identifier.EndsWith("stg");
            SRDebugEditor.SetEnabled(enable);
                
            $"SRDebugger enabled: {enable.ToString()}".Log();
        }
    }
}
