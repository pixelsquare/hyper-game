namespace Santelmo.Rinsurv
{
    public interface ILevelingEquipmentModule
    {
        /// <returns>
        /// The potential resulting equipment and cost from leveling up
        /// </returns>
        (IEquipment equipment, int cost) CanLevelUpEquipment(IEquipment baseEquipment);
        
        /// <returns>
        /// The resulting equipment from the leveling up
        /// </returns>
        IEquipment LevelUpEquipment(IEquipment baseEquipment);
    }
}
