using System;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveController : MonoBehaviour
    {
        [SerializeField] private InteractiveObject[] interactiveObjects = Array.Empty<InteractiveObject>();

        public InteractiveObject[] InteractiveObjects => interactiveObjects;

        public void StopAllInteracts()
        {
            foreach (var interactive in interactiveObjects)
            {
                interactive.Stop();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (var interactiveObj in interactiveObjects)
            {
                if (interactiveObj == null)
                {
                    continue;
                }
                
                var oldColor = Gizmos.color;
                Gizmos.color = Color.red;
            
                var interactiveObjPos = interactiveObj.transform.position;
                Gizmos.DrawLine(transform.position, interactiveObjPos);
                Gizmos.DrawIcon(interactiveObjPos, "sv_icon_dot6_pix16_gizmo");
                Gizmos.color = oldColor;
            }
        }
#endif
    }
}
