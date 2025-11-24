namespace Santelmo.Rinsurv
{
    public interface ICurrencyModule
    {
        int GetCurrency(string key);
        int AddCurrency(string key, int amount);
        int ReduceCurrency(string key, int amount);
    }
}
