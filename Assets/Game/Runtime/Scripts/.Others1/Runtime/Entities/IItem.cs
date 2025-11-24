namespace Santelmo.Rinsurv
{
    public interface IItem : IAsset
    {
        public string Name { get; }
        public string Description { get; }
        public string FlavorText { get; }
        public string IconSpriteName { get; }
        public string ItemTypeName { get; }
    }
}
