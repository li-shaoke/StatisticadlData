using Newtonsoft.Json;

namespace StatisticadlData.model.GetData
{
    public class DataResponse
    {
        [JsonProperty("result")]
        public ReportData Result { get; set; }
    }
}
