using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Common.Converters;

public class EnumDescriptionConverter<T> : EnumConverter
{
    public EnumDescriptionConverter() : base(typeof(T))
    {
        
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        if (fi == null) return null;

        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes.Length < 1)
        {
            return null;
        }
        else
        {
            string description = attributes[0].Description;
            return description;
        }
    }
}
