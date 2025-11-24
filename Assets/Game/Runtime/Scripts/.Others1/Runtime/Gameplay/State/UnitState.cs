namespace Santelmo.Rinsurv
{
    public delegate void OnUnitStateChange(UnitState prev, UnitState next);

    public interface IUnitState
    {
        public event OnUnitStateChange OnUnitStateChange;
        
        public UnitState State { get; }
    }

    public enum UnitState 
    {
        None,
        Idle,
        Move,
        Knockback,
        Death,
        Spawn,
    }
}
