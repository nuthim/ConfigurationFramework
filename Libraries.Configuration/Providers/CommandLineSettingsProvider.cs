using System;
using System.Collections.Generic;

namespace Libraries.Configuration.Providers
{
    /// <summary>
    /// Wrapper class for the executing application's configuration file settings section
    /// </summary>
    public class CommandLineSettingsProvider : ISettingsProvider
    {
        private char? _keyValueSeparator;

        public string Name { get; private set; }

        public char KeyValueSeparator
        {
            get { return _keyValueSeparator.GetValueOrDefault('='); }
            set { _keyValueSeparator = value; }
        }

        public CommandLineSettingsProvider(string name, char keyValueSeparator = '=')
        {
            Name = name;
            KeyValueSeparator = keyValueSeparator;
        }

        public IDictionary<string, string> GetSettings()
        {
            var dictionary = new Dictionary<string, string>();

            var settings = Environment.GetCommandLineArgs();
            if (settings.Length == 0)
                return dictionary;

            for (var index = 1; index < settings.Length; index++)
            {
                var pair = GetKeyValuePair(settings[index]);
                dictionary.Add(pair.Key, pair.Value ?? bool.TrueString);
            }
            return dictionary;
        }

        private KeyValuePair<string, string> GetKeyValuePair(string argument)
        {
            var separatorIndex = argument.IndexOf(KeyValueSeparator);
            if (separatorIndex == -1)
                throw new ArgumentException($"Commandline argument {argument} is not a key value pair");

            var key = argument.Substring(0, separatorIndex);
            var value = argument.Substring(separatorIndex + 1, argument.Length - key.Length - 1);
            return new KeyValuePair<string, string>(key, value);
        }
    }
}
