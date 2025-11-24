namespace Santelmo.Rinsurv
{
    public interface IItemDataModule
    {
        IItem GetItemFromId(string id);
        Hero GetHeroFromId(string id);
        Weapon GetWeaponFromId(string id); 
        EmblemChange GetEmblemChange(string id); 
        EmblemDeparture GetEmblemDeparture(string id); 
        EmblemPursuit GetEmblemPursuit(string id); 
    }
}
