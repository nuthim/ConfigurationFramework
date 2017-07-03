

namespace Libraries.Configuration.Readers
{
    public class CachedSettingsReader : DiscreteSettingsReader
    {
        public CachedSettingsReader() : base(null)
        {
            
        }

        protected override T OnKeyMissing<T>(string key, T defaultValue, string formattedValue)
        {
            Set(key, formattedValue);
            return defaultValue;
        }
    }
}
