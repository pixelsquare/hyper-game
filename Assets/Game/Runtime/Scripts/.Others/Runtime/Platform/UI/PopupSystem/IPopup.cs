using System;

namespace Kumu.Kulitan.UI
{
    public interface IPopup : IOrderableTransform
    {
        int Priority { get; set; }
        Action OnOpened { get; set; }
        Action OnClosed { get; set; }
        void Open();
        void Close();
    }
}
