namespace Santelmo.Rinsurv
{
    public interface IMoveCondition
    {
        public bool CanMove { get; }
    }

    public delegate void OnMoveStop();
    public delegate void OnMoveStart();

    public interface IMoveEvent
    {
        public event OnMoveStart OnMoveStart;
        public event OnMoveStop OnMoveStop;
    }
}
