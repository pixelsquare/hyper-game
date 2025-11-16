using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kumu.Kulitan.Common
{
    [AddComponentMenu("Utilities/BatchCounter")]
    public class BatchCounter : MonoBehaviour
    {
        [SerializeField] private Rect startRect = new Rect(10, 10, 140, 120);
        [SerializeField] private bool updateColor = true;
        [SerializeField] private bool allowDrag = true;
        [SerializeField] private int decimalCount = 1;

        private int staticBatches = 0;
        private int dynamicBatches = 0;
        private int instancedBatches = 0;
        private int totalBatches = 0;
        private Color color = Color.white;
        private string cachedString = "";
        private GUIStyle style;

        private void UpdateWindow(int windowID)
        {
            GUI.Label(new Rect(0, 0, startRect.width, startRect.height), cachedString, style);
            if (allowDrag)
            {
                GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
            }
        }

        private void OnGUI()
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = Color.white },
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 24
                };
            }

            GUI.color = updateColor ? color : Color.white;
            startRect = GUI.Window(1, startRect, UpdateWindow, "");
        }

        private void UpdateValues()
        {
#if UNITY_EDITOR
            staticBatches = UnityStats.staticBatches;
            dynamicBatches = UnityStats.dynamicBatches;
            instancedBatches = UnityStats.instancedBatches;
            totalBatches = UnityStats.batches;
#endif
        }


        private void Start()
        {
            UpdateValues();
        }

        private void Update()
        {
            UpdateValues();

#if UNITY_EDITOR
            cachedString = $"+{staticBatches} s\n+{instancedBatches} i\n+{dynamicBatches} d\n/{totalBatches} t";
            color = (totalBatches < 30) ? Color.green : (totalBatches > 60) ? Color.red : Color.yellow;
#else
            cachedString = "can only display batches in editor";
            color = Color.red;
#endif
        }
    }
}

