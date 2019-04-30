using Newtonsoft.Json;

namespace StatisticadlData.model.GetData
{
    public class BodyResponse
    {
        [JsonProperty("data")]
        public DataResponse[] Data { set; get; }
    }
}
