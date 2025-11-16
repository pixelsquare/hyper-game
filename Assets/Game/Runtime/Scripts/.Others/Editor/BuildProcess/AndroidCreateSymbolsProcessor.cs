using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Kumu.Kulitan.Common
{
    public class AndroidCreateSymbolsProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Debugging;
        }
    }
}
