using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public interface IUserAccountManager : IGlobalBinding
    {
        public UniTask InitializeAsync();
    }
}
