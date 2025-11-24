using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public interface IRollbackState
    {
        public UniTask<bool> OnRollbackAsync();
    }
}
