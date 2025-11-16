using Kumu.Kulitan.Hangout;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    /// Sets a TMP_Text component's text value to the version string.
    /// </summary>
    public class VersionLabel : MonoBehaviour
    {
        private void Start()
        {
            var text = GetComponent<TMP_Text>();
            text.text = $"v{VersionObject.FetchVersionString()}";
        }
    }
}
