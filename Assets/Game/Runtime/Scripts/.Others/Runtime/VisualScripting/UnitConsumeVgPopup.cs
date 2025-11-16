using Kumu.Kulitan.Gifting;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitConsumeVgPopup : Unit
    {
        [DoNotSerialize] public ControlInput onEnter;
        [DoNotSerialize] public ControlOutput onHasRewards;
        [DoNotSerialize] public ControlOutput onNoRewards;

        [DoNotSerialize] public ValueOutput message;
        
        protected override void Definition()
        {
            onEnter = ControlInput(nameof(onEnter), OnCheck);
            onHasRewards = ControlOutput(nameof(onHasRewards));
            onNoRewards = ControlOutput(nameof(onNoRewards));
            message = ValueOutput<string>(nameof(message));
            
            Succession(onEnter, onHasRewards);
            Succession(onEnter, onNoRewards);
        }

        private ControlOutput OnCheck(Flow flow)
        {
            var totalRewards = VGRewardsTracker.Instance.TotalRewards;

            VGRewardsTracker.Instance.ResetTotalRewards();

            if (totalRewards > 0)
            {
                var messageValue = $"You received <sprite=0>{totalRewards.ToString()} from gifts.";
                flow.SetValue(message, messageValue);
                return onHasRewards;
            }

            return onNoRewards;
        }
    }
}
