namespace Santelmo.Rinsurv
{
    public interface IUnitAct
    {
        public bool IsFinished();
    }

    public interface IUnitActStart
    {
        public void OnUnitActStart();
    }

    public interface IUnitActUpdate
    {
        public void OnUnitActUpdate();
    }

    public interface IUnitActEnd
    {
        public void OnUnitActEnd();
    }

    public interface IUnitActDuration
    {
        public float UnitActDuration { get; }
    }
}
