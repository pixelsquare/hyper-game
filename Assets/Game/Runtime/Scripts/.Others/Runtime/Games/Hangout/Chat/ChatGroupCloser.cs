using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChatGroupCloser : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float dragThreshold = -2;
    [SerializeField] private ChatGroupManager manager;
    
    private float totalYDrag;
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        totalYDrag = 0;
    }

    public void OnDrag(PointerEventData data)
    {
        totalYDrag += data.delta.y;
        
        if (totalYDrag <= dragThreshold)
        {
            manager.ToggleChatGroup(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        totalYDrag = 0;
    }
}
