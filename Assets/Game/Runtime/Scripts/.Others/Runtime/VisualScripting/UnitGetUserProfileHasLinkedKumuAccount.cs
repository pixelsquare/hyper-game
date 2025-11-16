using Unity.VisualScripting;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.VisualScripting
{
    [UnitCategory("Kumu")]
    public class UnitGetUserProfileHasLinkedKumuAccount : Unit
    {
        [DoNotSerialize] public ValueOutput output;
        
        protected override void Definition()
        {
            output = ValueOutput<bool>(nameof(output), flow => UserProfileLocalDataManager.Instance.GetLocalUserProfile().hasLinkedKumuAccount);
        }
    }
}
