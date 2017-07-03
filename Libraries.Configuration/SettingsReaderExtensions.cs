using Libraries.Configuration.Readers;


namespace Libraries.Configuration
{
    public static class SettingsReaderExtensions
    {
        public static ISettingsReader Combine(this ISettingsReader readerA, ISettingsReader readerB)
        {
            return CompositeSettingsReader.Create(readerA, readerB);
        }
    }
}
