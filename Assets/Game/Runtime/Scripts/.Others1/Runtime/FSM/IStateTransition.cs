namespace Santelmo.Rinsurv
{
    public interface IStateTransition
    {
        public string FromStateName { get; }
        public string ToStateName { get; }
        public int Priority { get; set; }
        public StateMachine Fsm { get; internal set; }

        public bool OnEvaluate();
    }
}
