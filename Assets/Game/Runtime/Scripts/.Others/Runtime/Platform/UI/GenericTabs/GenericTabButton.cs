using Kumu.Kulitan.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GenericTabButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onButtonActivated;
    [SerializeField] private UnityEvent onButtonDeactivated;
    [SerializeField] private Button button;

    [Header("Color Settings")] 
    [SerializeField] private bool autoChangeButtonColor;
    [SerializeField] private Color activatedColor;
    [SerializeField] private Color deactivatedColor;
    
    private GenericTabController tabController;

    public void Initialize(GenericTabController controller)
    {
        tabController = controller;
    }

    public void SetActive(bool isActive)
    {
        ChangeButtonColor(isActive);
        if (isActive)
        {
            onButtonActivated?.Invoke();
            return;
        }
        onButtonDeactivated?.Invoke();
    }

    public void ChangeButtonColor(bool isActive)
    {
        if (!autoChangeButtonColor)
        {
            return;
        }

        button.image.color = isActive ? activatedColor : deactivatedColor;
    }

    private void OnButtonClicked()
    {
        tabController.UpdateActiveTab(this);
    }
    
    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);   
    }
}
