using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Lobby
{
    public class HangoutFilterInit : ListView<HangoutFilterView, HangoutFilter>
    {
        [SerializeField] private HangoutFilterConfig hangoutFilterConfig;
        [SerializeField] private bool initOnStart;

        protected override void OnCreate(HangoutFilterView element, HangoutFilter datum)
        {
            base.OnCreate(element, datum);
            element.name = $"{prefab.name}_{datum.id}";
        }

        private void Start()
        {
            if (initOnStart)
            {
                Display(hangoutFilterConfig.Filters);
            }
        }
    }
}
