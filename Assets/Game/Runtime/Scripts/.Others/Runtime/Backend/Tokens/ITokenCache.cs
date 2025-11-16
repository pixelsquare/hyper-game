namespace Kumu.Kulitan.Backend
{
    public interface ITokenCache<T>
    {
        void SaveToken(T token);

        T LoadToken();

        bool IsValid();

        void Delete();
    }
}
