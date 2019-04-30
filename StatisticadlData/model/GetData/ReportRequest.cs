using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticadlData.model.GetData
{
	/// <summary>
	/// 报告请求实体
	/// getData
	/// </summary>
	public class ReportRequest
	{
		[JsonProperty("header")]
		public HeaderRequest Header { get; set; }

		[JsonProperty("body")]
		public BodyRequest Body { get; set; }

		public ReportRequest()
		{
			this.Header = new HeaderRequest();
			this.Body = null;
		}
	}
}
