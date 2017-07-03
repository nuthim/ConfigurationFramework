using System.ComponentModel;
using System.Globalization;


namespace Libraries.Configuration.Converters
{
    public class LogicalBooleanConverter : BooleanConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null)
            {
                string strVal = value.ToString().ToLower();

                switch (strVal)
                {
                    case "1":
                    case "t":
                    case "y":
                    case "yes":
                        return true;

                    case "0":
                    case "f":
                    case "n":
                    case "no":
                        return false;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
