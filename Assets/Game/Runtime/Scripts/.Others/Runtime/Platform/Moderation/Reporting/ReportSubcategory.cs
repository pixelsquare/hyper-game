using System.Collections.Generic;
using Kumu.Kulitan.Moderation;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class ReportSubcategory : MonoBehaviour
    {
        [SerializeField] private ReportSubcategoryButton subcategoryButtonRef;
        [SerializeField] private Transform contentParent;

        [SerializeField] private TMP_Text headerText;

        private readonly List<ReportSubcategoryButton> subcategoryButtonPool = new();

        /// <summary>
        ///     Used by FSM.
        /// </summary>
        public void Initialize(ReportCategoryData reportCategoryData)
        {
            headerText.text = reportCategoryData.categoryName;
            PopulateSubcategories(reportCategoryData.subcategories);
        }

        /// <summary>
        ///     Used by FSM.
        /// </summary>
        public void ResetSubcategoryButtons()
        {
            subcategoryButtonPool.ForEach(a => a.gameObject.SetActive(false));
        }

        private void PopulateSubcategories(string[] subcategories)
        {
            foreach (var subcategoryName in subcategories)
            {
                var subcategoryButton = GetSubcategoryButton();
                subcategoryButton.Initialize(subcategoryName);
                subcategoryButton.gameObject.SetActive(true);
            }
        }

        private ReportSubcategoryButton GetSubcategoryButton()
        {
            foreach (var subcategoryButton in subcategoryButtonPool)
            {
                if (!subcategoryButton.IsInitialized)
                {
                    return subcategoryButton;
                }
            }

            var newSubcategoryButton = Instantiate(subcategoryButtonRef, contentParent);
            subcategoryButtonPool.Add(newSubcategoryButton);
            return newSubcategoryButton;
        }
    }
}
