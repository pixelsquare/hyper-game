using System.Text;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [AddComponentMenu("Utilities/FramesPerSecondCounter")]
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private Rect startRect = new Rect(10, 10, 140, 80);
        [SerializeField] private bool updateColor = true;
        [SerializeField] private bool allowDrag = true;
        [SerializeField] private int decimalCount = 1;

        private float fps = 0f; // Current fps
        private float updateSlugginessFactor = 0.99f;
        private Color color = Color.white;
        private GUIStyle style;

        private StringBuilder stringBuilder;
        
        public static bool IsVisible { get; set; } = true;

        private static bool EnableOnAwake => Application.identifier.EndsWith("stg") || Application.identifier.EndsWith("dev");

        private void UpdateWindow(int windowID)
        {
            GUI.Label(new Rect(0, 0, startRect.width, startRect.height), stringBuilder.ToString(), style);
            if (allowDrag)
            {
                GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
            }
        }
        
        private void OnGUI()
        {
            if (!IsVisible)
            {
                return;
            }
            
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = Color.white }, 
                    alignment = TextAnchor.MiddleCenter, 
                    fontSize = 30
                };
            }

            GUI.color = updateColor ? color : Color.white;
            startRect = GUI.Window(0, startRect, UpdateWindow, "");
        }

        private void Awake()
        {
            stringBuilder = new StringBuilder();
            this.enabled = EnableOnAwake;
        }

        private void Start()
        {
            fps = Time.timeScale / Time.deltaTime;
        }
        
        private void Update()
        {
            var currentFps = Time.timeScale / Time.deltaTime;
            var newFps = fps * updateSlugginessFactor + currentFps * (1.0f - updateSlugginessFactor);

            fps = newFps;

            stringBuilder.Clear();
            stringBuilder.Append(fps.ToString("0.0"));
            stringBuilder.Append(" FPS");
            
            //Update the color
            color = (fps >= 30) ? Color.green : (fps > 10) ? Color.red : Color.yellow;
        }
    }
}
