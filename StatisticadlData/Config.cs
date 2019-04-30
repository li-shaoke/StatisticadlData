using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticadlData
{
	public class Config
	{
		/// <summary>
		/// preLogin,doLogin URL 
		/// </summary>
		public static readonly String LOGIN_URL = "https://api.baidu.com/sem/common/HolmesLoginService";

		/// <summary>
		/// Tongji API URL
		/// /getSiteList 获取当前用户下的站点和子目录列表，不包括权限站点和汇总网站。
		/// /getData 根据站点 ID 获取站点报告数据。
		/// </summary>
		public static readonly String API_URL = "https://api.baidu.com/json/tongji/v1/ReportService";

		public static readonly String USERNAME = "********";

		public static readonly String PASSWORD = "*************";

		public static readonly String TOKEN = "*********************************";

		/// <summary>
		/// UUID, used to identify your device, for instance: MAC address
		/// </summary>
		public static readonly String UUID=Guid.NewGuid().ToString();

		/// <summary>
		/// ZhanZhang:1,FengChao:2,Union:3,Columbus:4
		/// </summary>
		public static readonly string ACCOUNT_TYPE = "1";


		/// <summary>
		/// mongodb地址和端口号
		/// 多个地址的话，端口号后面接ip+端口
		/// </summary>
		public static readonly string MONGODB_CONNECTION = "mongodb://127.0.0.1:27017";

		public static readonly string DBName = "baidu";

	}
}
