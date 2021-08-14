using System;
using System.Globalization;

namespace Contoso.Text.Json
{
    public class TimeSpanConverter : DelegatedStringJsonConverter<TimeSpan?>
    {
        public TimeSpanConverter() : base(
            value => TimeSpan.Parse(value, CultureInfo.InvariantCulture),
            value => value?.ToString(null, CultureInfo.InvariantCulture))
        { }
    }
}