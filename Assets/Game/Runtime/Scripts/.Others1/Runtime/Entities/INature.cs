namespace Santelmo.Rinsurv
{
    public interface INature
    {
        public Nature Nature { get; set; }
    }

    public enum Nature
    {
        Victory,
        Departure,
        Pursuit,
        Change,
    }
}
