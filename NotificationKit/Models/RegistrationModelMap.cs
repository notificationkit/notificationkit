using System;
using System.Collections.Generic;

using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace NotificationKit.Models
{
    public class RegistrationModelMap : CsvClassMap<RegistrationModel>
    {
        public RegistrationModelMap()
        {
            Map(p => p.RegistrationId).Index(0);
            Map(p => p.Platform).Index(1);
            Map(p => p.Handle).Index(2);
            Map(p => p.Tags).Index(3).TypeConverter<SetConverter>();
            Map(p => p.RegistrationTime).Index(4).TypeConverter<DateTimeOffsetConverter>();
            Map(p => p.ExpirationTime).Index(5).TypeConverter<DateTimeOffsetConverter>();
        }
    }

    public class SetConverter : DefaultTypeConverter
    {
        public override bool CanConvertFrom(Type type)
        {
            return typeof(string) == type;
        }

        public override bool CanConvertTo(Type type)
        {
            return typeof(string) == type;
        }

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            return new HashSet<string>(text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            return string.Join(";", (ISet<string>)value);
        }
    }

    public class DateTimeOffsetConverter : DefaultTypeConverter
    {
        public override bool CanConvertFrom(Type type)
        {
            return typeof(string) == type;
        }

        public override bool CanConvertTo(Type type)
        {
            return typeof(string) == type;
        }

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            return DateTimeOffset.Parse(text);
        }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            return ((DateTimeOffset)value).ToString();
        }
    }
}