namespace Santelmo.Rinsurv
{
    public interface ILegacyLayerEffect
    {
        public LegacyLayer LegacyEffectLayer { get; }
    }

    public interface ILegacyLayerApply
    {
        public void ApplyLegacyLayer(LegacyLayer legacyLayer);
    }

    public enum LegacyLayer
    {
        [StringValue("")] None = 0,
        [StringValue("Idalmunon")] Idalmunon = 1,
        [StringValue("Kahangian")] Kahangian = 2,
        [StringValue("Tubignon")] Tubignon = 3,
        [StringValue("Ibabaw-non")] IbabawNon = 4,
        [StringValue("Lupan-on")] LupanOn = 5,
        [StringValue("Langit-non")] LangitNon = 6,
    }
}
