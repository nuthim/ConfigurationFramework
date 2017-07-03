

namespace Libraries.Configuration.Providers
{
    public class TrimmedLowerKeyProvider : IKeyProvider
    {
        public string GetKey(string raw)
        {
            if (raw == null)
                return null;

            return raw.Trim().ToLower();
        }
    }
}
