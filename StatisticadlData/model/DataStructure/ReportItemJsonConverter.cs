﻿using StatisticadlData.model.GetData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace StatisticadlData.model.DataStructure
{
    public class ReportItemJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            ReportItem item = new ReportItem();
			if (((Newtonsoft.Json.Linq.JContainer)token).Count<=0)
			{
				return item;
			}
            // 指标数据，有4部分组成
            // 0：维度数据
            int count1 = ((Newtonsoft.Json.Linq.JContainer)token[0]).Count;
            item.Dimension = new string[count1];
            for (int i = 0; i < count1; i++)
            {
                item.Dimension[i] = ((Newtonsoft.Json.Linq.JContainer)token[0][i]).First.ToString();
            }
			int count2 = 0;
			int count3 = 0;
			if (((Newtonsoft.Json.Linq.JContainer)token).Count > 1)
			{
				/// 1：指标数据
				count2 = ((Newtonsoft.Json.Linq.JContainer)token[1]).Count;
				if (count2 > 0)
				{
					count3 = ((Newtonsoft.Json.Linq.JContainer)token[1][0]).Count;
					item.Metric = new string[count2, count3];
					for (int i = 0; i < count2; i++)
					{
						for (int j = 0; j < count3; j++)
						{
							item.Metric[i, j] = token[1][i][j].ToString();
						}
					}
				}
			}
			

			if (((Newtonsoft.Json.Linq.JContainer)token).Count > 2)
			{
				// 2：对比时间数据
				int count4 = ((Newtonsoft.Json.Linq.JContainer)token[2]).Count;
				if (count4 > 0)
				{
					int count5 = ((Newtonsoft.Json.Linq.JContainer)token[2][0]).Count;
					item.ComparisonMetric = new string[count2, count3];
					for (int i = 0; i < count4; i++)
					{
						for (int j = 0; j < count5; j++)
						{
							item.ComparisonMetric[i, j] = token[2][i][j].ToString();
						}
					}
				}
			}
			

			if (((Newtonsoft.Json.Linq.JContainer)token).Count > 3)
			{
				// 3：变化率数据
				if (((Newtonsoft.Json.Linq.JContainer)token[3]).Count > 0)
				{
					int count6 = ((Newtonsoft.Json.Linq.JContainer)token[3]).Count;
					if (count6 > 0)
					{
						int count7 = ((Newtonsoft.Json.Linq.JContainer)token[3][0]).Count;
						item.ChangeRate = new string[count2, count3];
						for (int i = 0; i < count6; i++)
						{
							for (int j = 0; j < count7; j++)
							{
								item.ChangeRate[i, j] = token[3][i][j].ToString();
							}
						}
					}
				}
			}
			

            return item;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
			writer.WriteValue(string.Join(",", value));
		}
    }
}
