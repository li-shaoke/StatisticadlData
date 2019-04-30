using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace StatisticadlData.method
{
	
	public static class MethodName
	{
		#region 网站概况
		/// <summary>
		/// 趋势数据
		/// </summary>
		[Description("趋势数据")]
		public static readonly string TrendDataName = "overview/getTimeTrendRpt";

		/// <summary>
		/// 地域分布
		/// </summary>
		[Description("地域分布")]
		public static readonly string RegionalDistribution = "overview/getDistrictRpt";

		/// <summary>
		/// 来源网站、搜索词、
		/// 入口页面、受访页面
		/// </summary>
		[Description("来源网站、搜索词、入口页面、受访页面")]
		public static readonly string FromSourceAndWordHintPage = "overview/getCommonTrackRpt";

		#endregion

		/// <summary>
		/// 推广方式
		/// </summary>
		[Description("推广方式")]
		public static readonly string PromotionWay = "pro/product/a";

		/// <summary>
		/// 趋势分析
		/// </summary>
		[Description("趋势分析")]
		public static readonly string TrendAnalysis = "trend/time/a";

		/// <summary>
		/// 百度推广趋势
		/// </summary>
		[Description("百度推广趋势")]
		public static readonly string PromotedTrends = "pro/hour/a";

		/// <summary>
		/// 全部来源
		/// </summary>
		[Description("全部来源")]
		public static readonly string AllSource = "source/all/a";

		/// <summary>
		/// 搜索引擎
		/// </summary>
		[Description("搜索引擎")]
		public static readonly string SearchEngine = "source/engine/a";

		/// <summary>
		/// 搜索词
		/// </summary>
		[Description("搜索词")]
		public static readonly string SearchKeyWord = "source/searchword/a";

		/// <summary>
		/// 外部链接
		/// </summary>
		[Description("外部链接")]
		public static readonly string ExternalLinks = "source/link/a";

		/// <summary>
		/// 受访页面
		/// </summary>
		[Description("受访页面")]
		public static readonly string VisitedPage = "visit/toppage/a";

		/// <summary>
		/// 入口页面
		/// </summary>
		[Description("入口页面")]
		public static readonly string LandingPage = "visit/landingpage/a";

		/// <summary>
		/// 受访域名
		/// </summary>
		[Description("受访域名")]
		public static readonly string VisitedDomain = "visit/topdomain/a";

		/// <summary>
		/// 地区分布
		/// 按省
		/// </summary>
		[Description("地区分布_按省")]
		public static readonly string DistrictProvince = "visit/district/a";

		/// <summary>
		/// 地区分布
		/// 按国
		/// </summary>
		[Description("地区分布_按国")]
		public static readonly string DistrictState = "visit/world/a";
	}

	
}
