using Libraries.Configuration.Providers;


namespace Libraries.Configuration.Readers
{
    public class DiscreteSettingsReader : SettingsReaderImpl
    {
        public DiscreteSettingsReader(ISettingsProvider provider) : base(provider)
        {
            
        }
    }
}
