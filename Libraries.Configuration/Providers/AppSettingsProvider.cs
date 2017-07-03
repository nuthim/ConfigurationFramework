using System.Collections.Generic;
using System.Configuration;


namespace Libraries.Configuration.Providers
{
    /// <summary>
    /// Wrapper class for the executing application's configuration file settings section
    /// </summary>
    public class AppSettingsProvider : ISettingsProvider
    {
        public string Name { get; private set; }

        public AppSettingsProvider(string name)
        {
            Name = name;
        }

        public IDictionary<string, string> GetSettings()
        {
            var dictionary = new Dictionary<string, string>();

            var settings = ConfigurationManager.AppSettings;
            if (!settings.HasKeys())
                return dictionary;

            foreach (var key in settings.AllKeys)
                dictionary.Add(key, settings[key]);

            return dictionary;
        }
    }
}
