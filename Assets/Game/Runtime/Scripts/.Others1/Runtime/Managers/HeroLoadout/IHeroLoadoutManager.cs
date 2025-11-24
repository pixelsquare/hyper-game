using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public interface IHeroLoadoutManager : IGlobalBinding
    {
        public Hero[] Heroes { get; }
        public Hero ActiveHero { get; }

        public UniTask InitializeAsync();
        public void WriteLocalData();
        public void SetActiveHero(Hero hero);
    }
}
