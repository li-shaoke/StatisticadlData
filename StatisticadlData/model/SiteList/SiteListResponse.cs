using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticadlData.model.SiteList
{
	/// <summary>
	/// getSiteList返回体
	/// </summary>
	public class SiteListResponse
	{
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("header")]
		public HeaderResponse Header { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("body")]
		public BodyResponse Body { get; set; }
	}
}
