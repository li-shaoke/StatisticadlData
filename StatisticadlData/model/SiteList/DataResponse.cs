using Newtonsoft.Json;

namespace StatisticadlData.model.SiteList
{
    public class DataResponse
    {
        [JsonProperty("list")]
        public SiteInfo[] List { get; set; }
    }
}
