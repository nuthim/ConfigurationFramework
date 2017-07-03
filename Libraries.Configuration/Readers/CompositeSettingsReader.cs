using System;
using System.Linq;
using System.Collections.Generic;
using Libraries.Configuration.Converters;
using System.Threading.Tasks;

namespace Libraries.Configuration.Readers
{
    /// <summary>
    /// Composition of multiple readers. This provisions a way to aggregate multiple readers as if the source were one.
    /// Lookup for key is done through the first reader to the last. As a result, duplicate key's from subsquent readers will get ignored.
    /// </summary>
    public class CompositeSettingsReader : ISettingsReader
    {
        #region Fields
        private readonly List<ISettingsReader> _readers = new List<ISettingsReader>();
        private readonly StringConverterImpl _converter = new StringConverterImpl();
        #endregion

        #region ISettingsReader Implementation

        public string ProviderName { get; private set; }

        public IEnumerable<KeyValuePair<string, string>> List
        {
            get
            {
                var list = new List<KeyValuePair<string, string>>();
                _readers.ForEach(r => list.AddRange(r.List));
                return list;
            }
        }

        public bool Contains(string key)
        {
            if (key == null)
                return false;

            return _readers.Exists(x => x.Contains(ReaderConfiguration.KeyProvider.GetKey(key)));
        }

        public T Read<T>(string key)
        {
            if (TryReadKey(key, out T value))
                return value;

            return OnKeyMissing<T>(key);
        }

        public T Read<T>(string key, T defaultValue)
        {
            if (TryReadKey(key, out T value))
                return value;

            //Give all readers an opportunity to handle the default value
            Parallel.ForEach(_readers, r =>
            {
                try
                {
                    value = r.Read<T>(key, defaultValue);
                }
                catch
                {
                    //Do nothing 
                }
            });
            
            return OnKeyMissing(key, defaultValue, _converter.ConvertFrom<T>(defaultValue));
        }

        #endregion

        #region Constructor

        private CompositeSettingsReader(IEnumerable<ISettingsReader> readers)
        {
            _readers = new List<ISettingsReader>(readers);
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

        #endregion

        #region Helper Methods

        private bool TryReadKey<T>(string key, out T value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var reader = _readers.FirstOrDefault(x => x.Contains(ReaderConfiguration.KeyProvider.GetKey(key)));
            if (reader != null)
            {
                value = reader.Read<T>(key);
                return true;
            }

            value = default(T);
            return false;
        }

        #endregion

        public static ISettingsReader Create(params ISettingsReader[] readers)
        {
            if (readers == null)
                throw new ArgumentNullException(nameof(readers));

            return new CompositeSettingsReader(readers.Where(r => r != null))
            {
                ProviderName = readers.Select(r => r.ProviderName).Aggregate(string.Empty, (current, next) => current + "," + next).TrimStart(',')
            };
        }
    }

}
