using Kumu.Kulitan.Events;

public class FPSCounterShowHideEvent : Event<string>
{
    public const string EVENT_NAME = "FPSCounterShowHideEvent";

    public FPSCounterShowHideEvent(bool toShow) :  base(EVENT_NAME)
    {
        ToShow = toShow;
    }

    public bool ToShow { get; private set; }
}
