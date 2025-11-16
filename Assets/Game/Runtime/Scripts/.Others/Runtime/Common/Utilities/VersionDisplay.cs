using System;
using Kumu.Kulitan.Hangout;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class VersionDisplay : MonoBehaviour
    {
        public static bool IsVisible { get; set; } = true;
        public const string VERSION_FILE_NAME = "ube.version";

        [SerializeField] private GUIStyle style;
        [SerializeField] private Color bgColor = new(0f, 0f, 0f, .5f);
        [SerializeField] private Vector2 offset;

        private Texture2D bgTexture;
        private Rect rect;
        private string text;
        private GUIContent content;
        private VersionObject versionObject;

        private static bool EnableOnAwake => Application.identifier.EndsWith("stg") || Application.identifier.EndsWith("dev");

        private void OnGUI()
        {
            if (!IsVisible)
            {
                return;
            }

            DrawVersionInfoBox();
        }

        private void DrawVersionInfoBox()
        {
            GUI.DrawTexture(rect, bgTexture, ScaleMode.StretchToFill);
            GUI.Box(rect, content, style);
        }

        private Rect CalcRect(GUIStyle style, GUIContent content)
        {
            var size = style.CalcSize(content);
            var rect = new Rect
            {
                x = offset.x,
                y = Screen.height - size.y - offset.y,
                width = size.x,
                height = size.y
            };

            return rect;
        }

        private Texture2D CreateTexture(int width, int height)
        {
            var tex = new Texture2D(width, height);
            var pixels = tex.GetPixels();
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = bgColor;
            }

            tex.SetPixels(pixels);
            tex.Apply();

            return tex;
        }

        private string GetBuildGuid()
        {
            var text = "1234567890";
#if !UNITY_EDITOR
            text = Application.buildGUID.Substring(0, 7);
#endif
            return text.Substring(0, 7);
        }

        private void Awake()
        {
            this.enabled = EnableOnAwake;
            versionObject = VersionObject.Fetch();
        }

        private void OnEnable()
        {
            var buildType = "";

            if (Application.identifier.Contains("dev"))
            {
                buildType = "-DEV";
            }
            else if (Application.identifier.Contains("stg"))
            {
                buildType = "-STG";
            }
            
            text = $"[{Application.platform.ToString().ToLower()}] Ube{buildType} v{GetDisplayVersion()}";
            content = new GUIContent(text);
            bgTexture = CreateTexture(2, 2);
            rect = CalcRect(style, content);
        }

        private void OnDestroy()
        {
            if (bgTexture)
            {
                Destroy(bgTexture);
            }
        }

        private string GetDisplayVersion()
        {
            if (versionObject == null)
            {
                Debug.LogError("[VersionDisplay] Version object is missing.");
                return "";
            }

            return $"{versionObject.major}.{versionObject.minor}.{versionObject.patch}b{versionObject.build}";
        }
    }

    [Serializable]
    public class VersionData
    {
        public int major;
        public int minor;
        public int patch;
        public int build;
    }
}
