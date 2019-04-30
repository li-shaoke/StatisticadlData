using MongoDB.Bson;
using StatisticadlData.model.GetData;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticadlData.model
{
	public class ReportDataEntity: ReportData
	{
		public ObjectId _id { get; set; }
	}
}
