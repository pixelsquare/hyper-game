using System.ComponentModel;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

namespace Kumu.Kulitan.SROptions
{
    public class ServicesSROptions : UbeSROptions
    {
        [Preserve]
        [Category("Backend Service")]
        public async void UnlinkKumuAccount()
        {
            var result = await Services.AuthService.UnlinkUserAsync(new UnlinkUserRequest());

            if (result.HasError)
            {
                "Failed to unlink account!".LogError();
                $"[{result.Error.Code}] | {result.Error.Message}".LogError();
                return;
            }

            Application.Quit();

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
        }
    }
}
