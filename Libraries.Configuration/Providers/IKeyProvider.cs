

namespace Libraries.Configuration.Providers
{
    public interface IKeyProvider
    {
        string GetKey(string raw);
    }
}
