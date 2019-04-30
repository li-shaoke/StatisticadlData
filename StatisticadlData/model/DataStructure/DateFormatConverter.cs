using Newtonsoft.Json.Converters;

namespace StatisticadlData.model.DataStructure
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
