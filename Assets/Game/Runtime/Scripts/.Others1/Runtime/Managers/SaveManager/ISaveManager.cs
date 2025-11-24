namespace Santelmo.Rinsurv
{
    public interface ISaveManager : IGlobalBinding
    {
        public string Load(string key);

        public void Save(string key, string value);

        public void Delete(string key);

        public void ClearAll();
    }
}
