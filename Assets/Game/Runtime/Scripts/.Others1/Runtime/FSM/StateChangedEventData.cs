namespace Santelmo.Rinsurv
{
    public class StateChangedEventData
    {
        public string StateMachineId { get; internal set; }
        public string FromStateId { get; internal set; }
        public string ToStateId { get; internal set; }

        public override string ToString()
        {
            return $"{StateMachineId} | {FromStateId} -> {ToStateId}";
        }
    }
}
