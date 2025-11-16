namespace Kumu.Kulitan.Gifting
{
    public interface IVirtualGiftShareComputable
    {
        public int ComputeOwnShare();
    }
    
    public interface IVirtualGiftIdQuantified
    {
        public string GetId();
        public int GetQuantity();
    }
}
