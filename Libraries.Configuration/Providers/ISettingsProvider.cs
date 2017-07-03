using System.Collections.Generic;

namespace Libraries.Configuration.Providers
{
    public interface ISettingsProvider
    {
        string Name { get; }
        IDictionary<string, string> GetSettings();
    }

}
