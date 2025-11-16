using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(Button))]
    public class ReportCategoryButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;
        [SerializeField] private TMP_Text buttonText;

        private string categoryName;
        private Variables variables;
        private StateMachine stateMachine;

        public void Initialize(string category, bool showIndicator)
        {
            categoryName = category;
            buttonText.text = categoryName;
            buttonImage.enabled = showIndicator;
        }

        private void HandleButtonClicked()
        {
            variables.declarations.Set("SelectedCategory", categoryName);
            stateMachine.TriggerUnityEvent("OnSelectedCategory");
        }

        private void Awake()
        {
            variables = GetComponentInParent<Variables>();
            stateMachine = GetComponentInParent<StateMachine>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(HandleButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(HandleButtonClicked);
        }
    }
}
