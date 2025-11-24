namespace Santelmo.Rinsurv
{
    public interface IFSMProvider
    {
        public void Initialize(IStateMachine stateMachine);

        public void Cleanup();
    }
}
