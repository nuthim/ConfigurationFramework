using System;
using System.Collections.Generic;

namespace Libraries.Configuration.Providers
{
    /// <summary>
    /// Wrapper class for the environment variables
    /// </summary>
    public class EnvironmentSettingsProvider : ISettingsProvider
    {
        public string Name { get; private set; }

        public EnvironmentSettingsProvider(string name)
        {
            Name = name;
        }

        public IDictionary<string, string> GetSettings()
        {
            var dictionary = new Dictionary<string, string>();

            var settings = Environment.GetEnvironmentVariables();
            if (settings.Count == 0)
                return dictionary;

            foreach (var item in settings.Keys)
                dictionary.Add(item.ToString(), settings[item].ToString());

            return dictionary;
        }
    }
}
