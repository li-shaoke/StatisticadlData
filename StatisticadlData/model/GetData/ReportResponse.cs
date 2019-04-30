using Newtonsoft.Json;

namespace StatisticadlData.model.GetData
{
	/// <summary>
	/// getData接口返回实体
	/// </summary>
    public class ReportResponse
    {
        [JsonProperty("header")]
        public HeaderResponse Header { get; set; }

		/// <summary>
		/// 反序列化，需要针对调用的method单独实现
		/// 有部分arry数据，不规则
		/// </summary>
        [JsonProperty("body")]
        public BodyResponse Body { get; set; }
    }
}
