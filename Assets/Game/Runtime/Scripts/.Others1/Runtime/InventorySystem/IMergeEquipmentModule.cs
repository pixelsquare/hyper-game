namespace Santelmo.Rinsurv
{
    public interface IMergeEquipmentModule
    {
        /// <returns>
        /// The potential resulting equipment from the merge
        /// </returns>
        IEquipment CanMergeEquipment(IEquipment baseEquipment, IEquipment addOnEquipment);
        
        /// <summary>
        /// Processes the inputted equipment instances (if applicable) and handles the resulting merged equipment
        /// </summary>
        /// <param name="baseEquipment"></param>
        /// <param name="addOnEquipment"></param>
        /// <returns>
        /// The resulting equipment from the merge
        /// </returns>
        IEquipment MergeEquipment(IEquipment baseEquipment, IEquipment addOnEquipment);
    }
}
