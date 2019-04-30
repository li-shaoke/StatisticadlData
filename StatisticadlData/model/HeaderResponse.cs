using Newtonsoft.Json;

namespace StatisticadlData.model
{
	/// <summary>
	/// api接口返回 header
	/// </summary>
    public class HeaderResponse
    {
        [JsonProperty("desc")]
        public string desc { get; set; }

        [JsonProperty("failures")]
        public object[] failures { get; set; }

        [JsonProperty("oprs")]
        public int oprs { get; set; }

        [JsonProperty("succ")]
        public int succ { get; set; }

        [JsonProperty("oprtime")]
        public int oprtime { get; set; }

        [JsonProperty("quota")]
        public int quota { get; set; }

        [JsonProperty("rquota")]
        public int rquota { get; set; }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty("status")]
        public int status { get; set; }
    }
}
