using Kumu.Extensions;
using Kumu.Kulitan.Gifting;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    [UnitCategory("Kumu")]
    public class UnitIsVirtualGiftDatabaseSynced : Unit
    {
        [DoNotSerialize] private ValueOutput isSynced;

        private bool IsSynced => VirtualGiftDatabase.Current.IsSynced;
        
        protected override void Definition()
        {
            isSynced = ValueOutput<bool>(nameof(isSynced), flow =>
            {
                $"[UnitIsVirtualGiftDatabaseSynced] IsSynced: {IsSynced}".Log();
                return IsSynced;
            });
        }
    }
}
