using UnityEngine;

namespace Santelmo.Rinsurv
{
    public struct ItemStats
    {
        public Sprite icon;
        public string name;
        public double value;
        public double change;

        public ItemStats(Sprite icon, string name, double value, double change)
        {
            this.icon = icon;
            this.name = name;
            this.value = value;
            this.change = change;
        }
    }
}
