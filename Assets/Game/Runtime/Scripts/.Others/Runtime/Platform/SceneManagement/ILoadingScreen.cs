using Kumu.Kulitan.UI;

namespace Kumu.Kulitan.Common
{
    public interface ILoadingScreen : IOrderableTransform
    {
        void Show();
        void Hide();
        void UpdateLoadingProgress(float progress);
    }
}
