using Kumu.Kulitan.Common;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Lobby
{
    public class HangoutFilterView : ListViewElement<HangoutFilter>
    {
        [SerializeField] private TextMeshProUGUI label;

        public string Id { get; private set; }

        public override void Refresh(HangoutFilter data)
        {
            label.text = data.label;
            Id = data.id;
        }
    }
}
