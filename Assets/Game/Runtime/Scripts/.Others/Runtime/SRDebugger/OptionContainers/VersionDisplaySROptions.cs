using System.ComponentModel;
using Kumu.Kulitan.Common;
using UnityEngine.Scripting;

namespace Kumu.Kulitan.SROptions
{
    public class VersionDisplaySROptions : UbeSROptions, INotifyPropertyChanged
    {
        [Preserve]
        [Category("VersionInfo")]
        public bool IsVersionInfoVisible
        {
            get => VersionDisplay.IsVisible;
            set
            {
                VersionDisplay.IsVisible = value;
                OnPropertyChanged(nameof(IsVersionInfoVisible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
