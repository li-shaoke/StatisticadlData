using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace StatisticadlData.DbOpration
{
	public class MongoDbHelper
	{
		private readonly string connectionString = null;
		private readonly string databaseName = null;
		private IMongoDatabase database = null;
		private readonly bool autoCreateDb = false;
		private readonly bool autoCreateCollection = false;

		static MongoDbHelper()
		{
			BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
		}

		public MongoDbHelper(string _connectionString, string databasesName, bool autoCreateDb = false, bool autoCreateCollection = false)
		{
			this.connectionString = _connectionString;
			this.databaseName = databasesName;
			this.autoCreateDb = autoCreateDb;
			this.autoCreateCollection = autoCreateCollection;
		}

		#region 私有方法

		/// <summary>
		/// 获取 mongoClient连接
		/// </summary>
		/// <returns></returns>
		private MongoClient CreateMongoClient()
		{
			return new MongoClient(connectionString);
		}

		/// <summary>
		/// 获取数据库
		/// </summary>
		/// <returns></returns>
		private IMongoDatabase GetMongoDatabase()
		{
			if (database == null)
			{
				var client = CreateMongoClient();
				if (!DatabaseExists(client, databaseName) && !autoCreateDb)
				{
					throw new KeyNotFoundException("此MongoDB名称不存在：" + databaseName);
				}
				database = CreateMongoClient().GetDatabase(databaseName);
			}
			return database;
		}

		/// <summary>
		/// 检查数据库是否存在
		/// </summary>
		/// <param name="mongoClient"></param>
		/// <param name="databaseName"></param>
		/// <returns></returns>
		private bool DatabaseExists(MongoClient mongoClient, string databaseName)
		{
			try
			{
				var dbNames = mongoClient.ListDatabases()
					.ToList()
					.Select(db => db.GetValue("name").AsString);
				return dbNames.Contains(databaseName);
			}
			catch //如果连接的账号不能枚举出所有DB会报错，则默认为true
			{
				return true;
			}

		}
		/// <summary>
		/// 检查表是否存在
		/// </summary>
		/// <param name="mongoDatabase"></param>
		/// <param name="collectionName"></param>
		/// <returns></returns>
		private bool CollectionExists(IMongoDatabase mongoDatabase, string collectionName)
		{
			var options = new ListCollectionsOptions
			{
				Filter = Builders<BsonDocument>.Filter.Eq("name", collectionName)
			};
			return mongoDatabase.ListCollections(options).ToEnumerable().Any();
		}

		/// <summary>
		/// 获取表
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		private IMongoCollection<TDoc> GetMongoCollection<TDoc>(string collectionName, MongoCollectionSettings settings = null)
		{
			var mongoDatabase = GetMongoDatabase();

			if (!CollectionExists(mongoDatabase, collectionName) && !autoCreateCollection)
			{
				CreateCollection<TDoc>(collectionName);
			}
			return mongoDatabase.GetCollection<TDoc>(collectionName, settings);
		}

		/// <summary>
		/// 更新数据
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="doc"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		private List<UpdateDefinition<TDoc>> BuildUpdateDefinition<TDoc>(object doc, string parent)
		{
			var updateList = new List<UpdateDefinition<TDoc>>();
			foreach (var property in typeof(TDoc).GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				var key = parent == null ? property.Name : string.Format("{0}.{1}", parent, property.Name);
				//非空的复杂类型
				if ((property.PropertyType.IsClass || property.PropertyType.IsInterface) && property.PropertyType != typeof(string) && property.GetValue(doc) != null)
				{
					if (typeof(IList).IsAssignableFrom(property.PropertyType))
					{
						#region 集合类型
						int i = 0;
						var subObj = property.GetValue(doc);
						foreach (var item in subObj as IList)
						{
							if (item.GetType().IsClass || item.GetType().IsInterface)
							{
								updateList.AddRange(BuildUpdateDefinition<TDoc>(doc, string.Format("{0}.{1}", key, i)));
							}
							else
							{
								updateList.Add(Builders<TDoc>.Update.Set(string.Format("{0}.{1}", key, i), item));
							}
							i++;
						}
						#endregion
					}
					else
					{
						#region 实体类型
						//复杂类型，导航属性，类对象和集合对象
						var subObj = property.GetValue(doc);
						foreach (var sub in property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
						{
							updateList.Add(Builders<TDoc>.Update.Set(string.Format("{0}.{1}", key, sub.Name), sub.GetValue(subObj)));
						}
						#endregion
					}
				}
				else //简单类型
				{
					updateList.Add(Builders<TDoc>.Update.Set(key, property.GetValue(doc)));
				}
			}

			return updateList;
		}

		/// <summary>
		/// 创建索引
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="mongoCollection"></param>
		/// <param name="indexFields"></param>
		/// <param name="options"></param>
		private void CreateIndex<TDoc>(IMongoCollection<TDoc> mongoCollection, string[] indexFields, CreateIndexOptions options = null)
		{
			if (indexFields == null)
			{
				return;
			}
			var indexKeys = Builders<TDoc>.IndexKeys;
			IndexKeysDefinition<TDoc> keys = null;
			if (indexFields.Length > 0)
			{
				keys = indexKeys.Descending(indexFields[0]);
			}
			for (var i = 1; i < indexFields.Length; i++)
			{
				var strIndex = indexFields[i];
				keys = keys.Descending(strIndex);
			}

			if (keys != null)
			{
				mongoCollection.Indexes.CreateOne(keys, options);
			}

		}


		private void CreateIndex<TDoc>(IMongoCollection<TDoc> mongoCollection, string[] indexFields, CreateIndexModel<TDoc> model, CreateOneIndexOptions options = null)
		{
			if (indexFields == null)
			{
				return;
			}
			var indexKeys = Builders<TDoc>.IndexKeys;
			IndexKeysDefinition<TDoc> keys = null;
			if (indexFields.Length > 0)
			{
				keys = indexKeys.Descending(indexFields[0]);
			}
			for (var i = 1; i < indexFields.Length; i++)
			{
				var strIndex = indexFields[i];
				keys = keys.Descending(strIndex);
			}

			if (keys != null)
			{
				mongoCollection.Indexes.CreateOne(model, options);
			}

		}




		#endregion
		/// <summary>
		/// 检查表是否存在
		/// </summary>
		/// <param name="collectionName">表名称</param>
		/// <returns></returns>
		public bool CollectionExists(string collectionName)
		{
			var mongoDatabase = GetMongoDatabase();
			var options = new ListCollectionsOptions
			{
				Filter = Builders<BsonDocument>.Filter.Eq("name", collectionName)
			};
			return mongoDatabase.ListCollections(options).ToEnumerable().Any();
		}
		/// <summary>
		/// 创建表索引
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="indexFields"></param>
		/// <param name="options"></param>
		public void CreateCollectionIndex<TDoc>(string collectionName, string[] indexFields, CreateIndexOptions options = null)
		{
			CreateIndex(GetMongoCollection<TDoc>(collectionName), indexFields, options);
		}

		/// <summary>
		/// 创建表结构
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="indexFields"></param>
		/// <param name="options"></param>
		public void CreateCollection<TDoc>(string[] indexFields = null, CreateIndexOptions options = null)
		{
			string collectionName = typeof(TDoc).Name;
			CreateCollection<TDoc>(collectionName, indexFields, options);
		}

		/// <summary>
		/// 创建表索引
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="indexFields"></param>
		/// <param name="options"></param>
		public void CreateCollection<TDoc>(string collectionName, string[] indexFields = null, CreateIndexOptions options = null)
		{
			var mongoDatabase = GetMongoDatabase();
			mongoDatabase.CreateCollection(collectionName);
			CreateIndex(GetMongoCollection<TDoc>(collectionName), indexFields, options);
		}

		/// <summary>
		/// 查找
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="filter"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public List<TDoc> Find<TDoc>(Expression<Func<TDoc, bool>> filter, FindOptions options = null)
		{
			string collectionName = typeof(TDoc).Name;
			return Find<TDoc>(collectionName, filter, options);
		}

		public List<TDoc> Find<TDoc>(string collectionName, Expression<Func<TDoc, bool>> filter, FindOptions options = null)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			return colleciton.Find(filter, options).ToList();
		}

		/// <summary>
		/// 分页查找
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="filter"></param>
		/// <param name="keySelector"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="rsCount"></param>
		/// <returns></returns>
		public List<TDoc> FindByPage<TDoc, TResult>(Expression<Func<TDoc, bool>> filter, Expression<Func<TDoc, TResult>> keySelector, int pageIndex, int pageSize, out int rsCount)
		{
			string collectionName = typeof(TDoc).Name;
			return FindByPage<TDoc, TResult>(collectionName, filter, keySelector, pageIndex, pageSize, out rsCount);
		}

		/// <summary>
		/// 分页查找
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="filter"></param>
		/// <param name="keySelector"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="rsCount"></param>
		/// <returns></returns>
		public List<TDoc> FindByPage<TDoc, TResult>(string collectionName, Expression<Func<TDoc, bool>> filter, Expression<Func<TDoc, TResult>> keySelector, int pageIndex, int pageSize, out int rsCount)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			rsCount = colleciton.AsQueryable().Where(filter).Count();

			int pageCount = rsCount / pageSize + ((rsCount % pageSize) > 0 ? 1 : 0);
			if (pageIndex > pageCount) pageIndex = pageCount;
			if (pageIndex <= 0) pageIndex = 1;

			return colleciton.AsQueryable(new AggregateOptions { AllowDiskUse = true }).Where(filter).OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
		}

		/// <summary>
		/// 分页查找 较大数据
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="filter"></param>
		/// <param name="keySelector"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="rsCount"></param>
		/// <returns></returns>
		public List<TDoc> FindBigDataByPage<TDoc, TResult>(string collectionName, Expression<Func<TDoc, bool>> filter, Expression<Func<TDoc, TResult>> keySelector, int pageIndex, int pageSize, out int rsCount)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			rsCount = 500;

			int pageCount = rsCount / pageSize + ((rsCount % pageSize) > 0 ? 1 : 0);
			if (pageIndex > pageCount) pageIndex = pageCount;
			if (pageIndex <= 0) pageIndex = 1;
			return colleciton.AsQueryable(new AggregateOptions { AllowDiskUse = true }).Where(filter).OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
		}

		/// <summary>
		/// 添加
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="doc"></param>
		/// <param name="options"></param>
		public void Insert<TDoc>(TDoc doc, InsertOneOptions options = null)
		{
			string collectionName = typeof(TDoc).Name;
			Insert<TDoc>(collectionName, doc, options);
		}

		/// <summary>
		/// 添加
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="doc"></param>
		/// <param name="options"></param>
		public void Insert<TDoc>(string collectionName, TDoc doc, InsertOneOptions options = null)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			colleciton.InsertOne(doc, options);
		}

		/// <summary>
		/// 批量添加
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="docs"></param>
		/// <param name="options"></param>
		public void InsertMany<TDoc>(IEnumerable<TDoc> docs, InsertManyOptions options = null)
		{
			string collectionName = typeof(TDoc).Name;
			InsertMany<TDoc>(collectionName, docs, options);
		}

		/// <summary>
		/// 批量添加
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="docs"></param>
		/// <param name="options"></param>
		public void InsertMany<TDoc>(string collectionName, IEnumerable<TDoc> docs, InsertManyOptions options = null)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			colleciton.InsertMany(docs, options);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="doc"></param>
		/// <param name="filter"></param>
		/// <param name="options"></param>
		public void Update<TDoc>(TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateOptions options = null)
		{
			string collectionName = typeof(TDoc).Name;
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			List<UpdateDefinition<TDoc>> updateList = BuildUpdateDefinition<TDoc>(doc, null);
			colleciton.UpdateOne(filter, Builders<TDoc>.Update.Combine(updateList), options);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="doc"></param>
		/// <param name="filter"></param>
		/// <param name="options"></param>
		public void Update<TDoc>(string collectionName, TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateOptions options = null)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			List<UpdateDefinition<TDoc>> updateList = BuildUpdateDefinition<TDoc>(doc, null);
			colleciton.UpdateOne(filter, Builders<TDoc>.Update.Combine(updateList), options);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="doc"></param>
		/// <param name="filter"></param>
		/// <param name="updateFields"></param>
		/// <param name="options"></param>
		public void Update<TDoc>(TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateDefinition<TDoc> updateFields, UpdateOptions options = null)
		{
			string collectionName = typeof(TDoc).Name;
			Update<TDoc>(collectionName, doc, filter, updateFields, options);
		}
		/// <summary>
		/// 更新
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="doc"></param>
		/// <param name="filter"></param>
		/// <param name="updateFields"></param>
		/// <param name="options"></param>
		public void Update<TDoc>(string collectionName, TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateDefinition<TDoc> updateFields, UpdateOptions options = null)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			colleciton.UpdateOne(filter, updateFields, options);
		}

		/// <summary>
		/// 批量更新
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="doc"></param>
		/// <param name="filter"></param>
		/// <param name="options"></param>
		public void UpdateMany<TDoc>(TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateOptions options = null)
		{
			string collectionName = typeof(TDoc).Name;
			UpdateMany<TDoc>(collectionName, doc, filter, options);
		}

		/// <summary>
		/// 批量更新
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="doc"></param>
		/// <param name="filter"></param>
		/// <param name="options"></param>
		public void UpdateMany<TDoc>(string collectionName, TDoc doc, Expression<Func<TDoc, bool>> filter, UpdateOptions options = null)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			List<UpdateDefinition<TDoc>> updateList = BuildUpdateDefinition<TDoc>(doc, null);
			colleciton.UpdateMany(filter, Builders<TDoc>.Update.Combine(updateList), options);
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="filter"></param>
		/// <param name="options"></param>
		public void Delete<TDoc>(Expression<Func<TDoc, bool>> filter, DeleteOptions options = null)
		{
			string collectionName = typeof(TDoc).Name;
			Delete<TDoc>(collectionName, filter, options);
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="filter"></param>
		/// <param name="options"></param>
		public void Delete<TDoc>(string collectionName, Expression<Func<TDoc, bool>> filter, DeleteOptions options = null)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			colleciton.DeleteOne(filter, options);
		}

		/// <summary>
		/// 批量删除
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="filter"></param>
		/// <param name="options"></param>
		public void DeleteMany<TDoc>(Expression<Func<TDoc, bool>> filter, DeleteOptions options = null)
		{
			string collectionName = typeof(TDoc).Name;
			DeleteMany<TDoc>(collectionName, filter, options);
		}

		/// <summary>
		/// 批量删除
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		/// <param name="filter"></param>
		/// <param name="options"></param>
		public void DeleteMany<TDoc>(string collectionName, Expression<Func<TDoc, bool>> filter, DeleteOptions options = null)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			colleciton.DeleteMany(filter, options);
		}

		/// <summary>
		/// 清空表数据
		/// </summary>
		/// <typeparam name="TDoc"></typeparam>
		/// <param name="collectionName"></param>
		public void ClearCollection<TDoc>(string collectionName)
		{
			var colleciton = GetMongoCollection<TDoc>(collectionName);
			var inddexs = colleciton.Indexes.List();
			List<IEnumerable<BsonDocument>> docIndexs = new List<IEnumerable<BsonDocument>>();
			while (inddexs.MoveNext())
			{
				docIndexs.Add(inddexs.Current);
			}
			var mongoDatabase = GetMongoDatabase();
			mongoDatabase.DropCollection(collectionName);

			if (!CollectionExists(mongoDatabase, collectionName))
			{
				CreateCollection<TDoc>(collectionName);
			}

			if (docIndexs.Count > 0)
			{
				colleciton = mongoDatabase.GetCollection<TDoc>(collectionName);
				foreach (var index in docIndexs)
				{
					foreach (IndexKeysDefinition<TDoc> indexItem in index)
					{
						try
						{
							colleciton.Indexes.CreateOne(indexItem);
						}
						catch
						{ }
					}
				}
			}

		}
	}
}
