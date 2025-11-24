namespace Santelmo.Rinsurv
{
    public delegate void OnMomentumStop(bool isAtFullSpeed);

    public interface IMomentumStop
    {
        public event OnMomentumStop OnMomentumStop;
    }

    public interface IMomentumStopEffect
    {
        public void OnMomentumStopEffect(bool isAtFullSpeed);
    }
}
