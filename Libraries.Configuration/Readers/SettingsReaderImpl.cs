using System;
using System.Linq;
using System.Collections.Generic;
using Libraries.Configuration.Providers;
using Libraries.Configuration.Converters;

namespace Libraries.Configuration.Readers
{
    public abstract class SettingsReaderImpl : ISettingsReader
    {
        #region Fields
        private readonly Dictionary<string, string> _settings = new Dictionary<string, string>();
        private readonly StringConverterImpl _converter = new StringConverterImpl();
        #endregion

        #region Constructor

        protected SettingsReaderImpl(ISettingsProvider provider)
        {
            ProviderName = provider != null ? provider.Name : GetType().Name;
            Initialize(provider);
        }

        #endregion

        #region ISettingsReader Implementation

        public string ProviderName { get; }

        public IEnumerable<KeyValuePair<string, string>> List
        {
            get
            {
                return _settings.ToList();
            }
        }

        public bool Contains(string key)
        {
            if (key == null)
                return false;

            return _settings.ContainsKey(ReaderConfiguration.KeyProvider.GetKey(key));
        }

        public T Read<T>(string key)
        {
            if (Read(key, out T result))
                return result;

            return OnKeyMissing<T>(key);
        }

        public T Read<T>(string key, T defaultValue)
        {
            if (Read(key, out T result))
                return result;

            return OnKeyMissing(key, defaultValue, _converter.ConvertFrom<T>(defaultValue));
        }


        #endregion

        #region Protected Methods

        protected virtual T OnKeyMissing<T>(string key)
        {
            throw new KeyNotFoundException($"Key: {key} not found");
        }

        protected virtual T OnKeyMissing<T>(string key, T defaultValue, string formattedValue)
        {
            return defaultValue;
        }

        protected void Set(string param, string value)
        {
            string key = GetKey(param);

            if (_settings.ContainsKey(key))
                _settings[key] = value;
            else
                _settings.Add(key, value);
        }

        #endregion

        #region Helper Methods

        private void Initialize(ISettingsProvider provider)
        {
            if (provider == null)
                return;

            var dictionary = provider.GetSettings();
            if (dictionary == null)
                return;

            foreach (var item in dictionary)
                Set(item.Key, item.Value);
        }

        private string GetKey(string param)
        {
            if (param == null)
                throw new ArgumentNullException(param);

            return ReaderConfiguration.KeyProvider.GetKey(param);
        }

        private bool Read<T>(string key, out T result)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            result = default(T);

            if (GetValue(GetKey(key), out string value))
            {
                result = _converter.ConvertTo<T>(value);
                return true;
            }

            return false;
        }

        private bool GetValue(string param, out string value)
        {
            if (_settings.ContainsKey(param))
            {
                value = _settings[param];
                return true;
            }

            value = null;
            return false;
        }

        #endregion
    }
   
}
