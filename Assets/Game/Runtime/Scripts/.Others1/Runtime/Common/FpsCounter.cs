using System.Text;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [AddComponentMenu("Utilities/FpsCounter")]
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private Rect _startRect = new(10, 10, 140, 80);
        [SerializeField] private bool _updateColor = true;
        [SerializeField] private bool _allowDrag = true;

        private float _fps = 0f; // Current fps
        private float _updateSlugginessFactor = 0.99f;
        private Color _color = Color.white;
        private GUIStyle _style;

        private StringBuilder _stringBuilder;
        
        public static bool IsVisible { get; set; } = true;

        private static bool EnableOnAwake => Application.identifier.EndsWith("stg") || Application.identifier.EndsWith("dev");

        private void UpdateWindow(int windowID)
        {
            GUI.Label(new Rect(0, 0, _startRect.width, _startRect.height), _stringBuilder.ToString(), _style);
            if (_allowDrag)
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
            
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = Color.white }, 
                    alignment = TextAnchor.MiddleCenter, 
                    fontSize = 30
                };
            }

            GUI.color = _updateColor ? _color : Color.white;
            _startRect = GUI.Window(0, _startRect, UpdateWindow, "");
        }

        private void Awake()
        {
            _stringBuilder = new StringBuilder();
            this.enabled = EnableOnAwake;
        }

        private void Start()
        {
            _fps = Time.timeScale / Time.deltaTime;
        }
        
        private void Update()
        {
            if (Time.timeScale == 0f || Time.deltaTime == 0f)
            {
                return;
            }
            
            var currentFps = Time.timeScale / Time.deltaTime;

            var newFps = _fps * _updateSlugginessFactor + currentFps * (1.0f - _updateSlugginessFactor);
            _fps = newFps;

            _stringBuilder.Clear();
            _stringBuilder.Append(_fps.ToString("0.0"));
            _stringBuilder.Append(" FPS");
            
            _color = _fps > 20 ?
                Color.green : _fps > 10 ?
                Color.yellow : Color.red;
        }
    }
}
