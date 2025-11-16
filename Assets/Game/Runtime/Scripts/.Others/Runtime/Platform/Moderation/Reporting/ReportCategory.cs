using System;
using Kumu.Kulitan.Moderation;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class ReportCategory : MonoBehaviour
    {
        [SerializeField] private ReportCategoryButton categoryButtonRef;
        [SerializeField] private Transform contentParent;

        [SerializeField] private ReportCategoryData[] reportCategories;

        /// <summary>
        ///     Used by FSM.
        /// </summary>
        public void PopulateReportCategories()
        {
            foreach (var category in reportCategories)
            {
                var categoryButton = Instantiate(categoryButtonRef, contentParent);
                var showIndicator = category.subcategories.Length > 0;
                categoryButton.Initialize(category.categoryName, showIndicator);
            }
        }

        /// <summary>
        ///     Used by FSM.
        /// </summary>
        public ReportCategoryData GetReportCategory(string category)
        {
            return Array.Find(reportCategories, reportCategory => string.Compare(reportCategory.categoryName, category, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}
