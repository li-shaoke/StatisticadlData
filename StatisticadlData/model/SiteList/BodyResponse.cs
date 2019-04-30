using Newtonsoft.Json;

namespace StatisticadlData.model.SiteList
{
	/// <summary>
	/// getSiteList接口返回实体
	/// </summary>
    public class BodyResponse
    {
        [JsonProperty("data")]
        public DataResponse[] Data { set; get; }
    }
}
