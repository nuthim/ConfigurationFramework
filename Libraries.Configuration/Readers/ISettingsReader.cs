using System.Collections.Generic;


namespace Libraries.Configuration.Readers
{
    public interface ISettingsReader
    {
        string ProviderName { get; }

        IEnumerable<KeyValuePair<string, string>> List { get; }

        bool Contains(string key);

        T Read<T>(string key);

        T Read<T>(string key, T defaultValue);
    }
}
