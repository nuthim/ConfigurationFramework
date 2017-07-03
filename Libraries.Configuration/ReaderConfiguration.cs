using System;
using System.ComponentModel;
using Libraries.Configuration.Providers;
using System.Collections.Concurrent;

namespace Libraries.Configuration
{
    public static class ReaderConfiguration
    {
        private static ISettingsProvider _appSettingsProvider;
        private static ISettingsProvider _cmdSettingsProvider;
        private static ISettingsProvider _envSettingsProvider;
        private static IKeyProvider _keyProvider;
        private static readonly ConcurrentDictionary<Type, TypeConverter> _converters = new ConcurrentDictionary<Type, TypeConverter>();

        public static ISettingsProvider AppSettingsProvider
        {
            get { return _appSettingsProvider != null ? _appSettingsProvider : new AppSettingsProvider("ApplicationSettings"); }
            set { _appSettingsProvider = value; }
        }

        public static ISettingsProvider CmdSettingsProvider
        {
            get { return _cmdSettingsProvider != null ? _cmdSettingsProvider : new CommandLineSettingsProvider("CommandLineSettings"); }
            set { _cmdSettingsProvider = value; }
        }

        public static ISettingsProvider EnvSettingsProvider
        {
            get { return _envSettingsProvider != null ? _envSettingsProvider : new EnvironmentSettingsProvider("EnvironmentSettings"); }
            set { _envSettingsProvider = value; }
        }

        public static IKeyProvider KeyProvider
        {
            get { return _keyProvider != null ? _keyProvider : new TrimmedLowerKeyProvider(); }
            set { _keyProvider = value; }
        }

        public static void RegisterTypeConverter(Type type, TypeConverter converter)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            _converters.AddOrUpdate(type, converter, (x, y) => converter);
        }

        public static TypeConverter GetTypeConverter(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            _converters.TryGetValue(type, out TypeConverter converter);
            return converter;
        }
    }


}
