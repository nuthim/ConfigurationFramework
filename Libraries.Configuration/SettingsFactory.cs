using System;
using Libraries.Configuration.Readers;
using Libraries.Configuration.Extensions;

namespace Libraries.Configuration
{
    //Provides convenience methods to create discrete or composite readers 
    public static class SettingsReaderFactory
    {
        public static ISettingsReader GetSettingsReader(ReaderType type)
        {
            ISettingsReader reader = null;
            foreach (ReaderType item in Enum.GetValues(typeof(ReaderType)))
            {
                if (!type.HasFlag(item))
                    continue;

                var newReader = GetReaderForType(item);
                if (newReader == null)
                    continue;

                reader = reader == null ? newReader : reader.Combine(newReader);
            }

            return reader;
        }

        private static ISettingsReader GetReaderForType(ReaderType type)
        {
            switch (type)
            {
                case ReaderType.CommandLine:
                    return new DiscreteSettingsReader(ReaderConfiguration.CmdSettingsProvider);

                case ReaderType.AppSettings:
                    return new DiscreteSettingsReader(ReaderConfiguration.AppSettingsProvider);

                case ReaderType.Environment:
                    return new DiscreteSettingsReader(ReaderConfiguration.EnvSettingsProvider);

                default:
                    return null;
            }
        }
    }

    [Flags]
    public enum ReaderType
    {
        CommandLine = 1,
        AppSettings = 2,
        Environment = 4
    }
}
