using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticadlData.model.SiteList
{
	/// <summary>
	/// geSiteList请求体
	/// </summary>
	public class RequestSiteListHeader
	{
		[JsonProperty("header")]
		public HeaderRequest header { get; set; }
	}
}
