using System.ComponentModel;
using System.Globalization;


namespace Libraries.Configuration.Converters
{
    public class ObjectConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null)
            {
                return value;
            }
            
            return base.ConvertFrom(context, culture, value);
        }
    }
}
