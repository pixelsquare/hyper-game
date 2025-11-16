using Kumu.Kulitan.Hangout;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitCheckRoomExit : Unit
    {
        [DoNotSerialize] public ControlInput onEnter;
        [DoNotSerialize] public ControlOutput onLeftNormal;
        [DoNotSerialize] public ControlOutput onLeftHost;
        
        protected override void Definition()
        {
            onEnter = ControlInput(nameof(onEnter), OnCheck);
            onLeftNormal = ControlOutput(nameof(onLeftNormal));
            onLeftHost = ControlOutput(nameof(onLeftHost));
            
            Succession(onEnter, onLeftNormal);
            Succession(onEnter, onLeftHost);
        }

        private ControlOutput OnCheck(Flow flow)
        {
            if (RoomConnectionDetails.Instance.roomExitMode == RoomExitMode.OnHostLeft)
            {
                return onLeftHost;
            }

            return onLeftNormal;
        }
    }
}
