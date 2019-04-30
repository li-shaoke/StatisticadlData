using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using StatisticadlData.DbOpration;
using StatisticadlData.method;
using StatisticadlData.model;
using StatisticadlData.model.GetData;
using StatisticadlData.model.SiteList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StatisticadlData
{
	class Program
	{
		static void Main(string[] args)
		{
			var siteList = GetSiteList();
			int siteId = siteList.Body.Data[0].List[0].SiteId;//siteId是固定的，调用一次就可以了

			var reqeust = new BodyRequest
			{
				StartDate = "20181223",
				EndDate = System.DateTime.Now.AddDays(1).ToString("yyyyMMdd"),

				MaxResults = 20,//0-全部
				StartIndex = 0,

				SiteId = siteId,

				//Method = "visit/toppage/a" //要调用的报告的名称，根据文档写
			};

			Console.WriteLine("正在进行 数据查询 和 落库 中 。。。");
			Console.WriteLine("请稍等 。。。");
			CallApiAndSave(reqeust);
			Console.ReadKey();
		}


		private static void CallApiAndSave(BodyRequest request)
		{
			var dic = GetDicOfAllName();
			BodyRequest para = null;
			var clientDb = new MongoDbHelper(Config.MONGODB_CONNECTION, Config.DBName, true, true);
			Parallel.ForEach(dic, item =>
			{
				para = (BodyRequest)request.Clone();
				para.Method = item.Value;
				var data = GetData(para);

				//json处理，取值到result节点
				JObject jsonObj = JObject.Parse(data);
				var resultJson = jsonObj["body"].First.First.First.First.First.ToString();

				//json直接转化成BsonDocument
				BsonDocument document = BsonDocument.Parse(resultJson);
				clientDb.Insert(item.Key, document);
			});
			Console.WriteLine("数据落入mongo完毕");
		}

		/// <summary>
		/// 获取全部 methodName
		/// </summary>
		/// <returns></returns>
		private static Dictionary<string, string> GetDicOfAllName()
		{
			var dic = new Dictionary<string, string>();
			var fields = typeof(MethodName).GetFields();
			foreach (var item in fields)
			{
				dic.Add(item.Name, item.GetValue(string.Empty).ToString());//保存到mongoDB时，doc的名称
				//var desc = ((DescriptionAttribute)item.GetCustomAttributes().FirstOrDefault()).Description;
			}
			return dic;
		}

		/// <summary>
		/// 获取站点ID
		/// </summary>
		/// <returns></returns>
		private static SiteListResponse GetSiteList()
		{
			string url = Config.API_URL + "/getSiteList";
			var request = new RequestSiteListHeader()
			{
				header = new HeaderRequest
				{
					AccountType = Config.ACCOUNT_TYPE,
					Password = Config.PASSWORD,
					Token = Config.TOKEN,
					UserName = Config.USERNAME
				}
			};
			var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
			var message = HttpHelper.PostHttps(url, requestJson);
			return Newtonsoft.Json.JsonConvert.DeserializeObject<SiteListResponse>(message);
		}

		/// <summary>
		/// 获取报告
		/// </summary>
		/// <param name="body"></param>
		/// <returns></returns>
		private static string GetData(BodyRequest body)
		{
			string url= Config.API_URL + "/getData";
			var request = new ReportRequest
			{
				Header = new HeaderRequest
				{
					AccountType = Config.ACCOUNT_TYPE,
					Password = Config.PASSWORD,
					Token = Config.TOKEN,
					UserName = Config.USERNAME
				},
				Body = body
			};
			var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
			var message = HttpHelper.PostHttps(url, requestJson);

			//json中数据有很多空数组，反序列化需要特殊处理
			//目前ReportResponse反序列化失败,需要针对methodName单独写实体类
			//var reault = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportResponse>(message);
			return message;
		}
	}
}
