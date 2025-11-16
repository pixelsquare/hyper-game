using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(Button))]
    public class ReportSubcategoryButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;

        public bool IsInitialized { get; private set; }

        private Variables variables;
        private StateMachine stateMachine;

        private string subcategoryName;

        public void Initialize(string subcategory)
        {
            IsInitialized = true;
            subcategoryName = subcategory;
            buttonText.text = subcategoryName;
        }

        private void HandleButtonClicked()
        {
            variables.declarations.Set("SelectedSubcategory", subcategoryName);
            stateMachine.TriggerUnityEvent("OnSelectedSubcategory");
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
            IsInitialized = false;
            button.onClick.RemoveListener(HandleButtonClicked);
        }
    }
}
