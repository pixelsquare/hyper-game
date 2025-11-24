using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public interface IGameplaySystem
    {
        public int GameplaySystemInitPriority { get; }
        public UniTask GameplaySystemInitAsync();
    }
}
