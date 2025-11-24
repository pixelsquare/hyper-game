using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public class UnloadGameHud : BaseHud
    {
        public UnloadGameHud Setup(float duration = 1f)
        {
            Task = UniTask.WaitForSeconds(duration);
            return this;
        }
    }
}
