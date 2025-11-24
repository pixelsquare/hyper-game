namespace Santelmo.Rinsurv
{
    public interface IHudManager : IGlobalBinding
    {
        public bool IsInitialized { get; }

        public T ShowHudAsync<T>(HudType hudType) where T : BaseHud;
        public void ShowHud(HudType hudType);
        public void HideHud(HudType hudType);
        public bool HudExist(HudType hudType);
        public void Cleanup();
    }
}
