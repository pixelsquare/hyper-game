namespace Santelmo.Rinsurv
{
    public interface IMaxHealth
    {
        public uint MaxHealth { get; }
    }

    public interface IMoveSpeed
    {
        public float MoveSpeed { get; }
    }

    public interface IKnockback
    {
        public float KnockbackDistance { get; }
        public float KnockbackDuration { get; }
    }
}
