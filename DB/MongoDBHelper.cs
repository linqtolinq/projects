namespace MQTT_Api_Server_Lifesaver.DB
{
    public static class MongoDBHelper
    {
        public static IMongoDatabase? Db { get; set; }
        public static string? connectstring { get; set; }

        public static IMongoDatabase CreateDbContext()
        {
            if (APPEnvironment.GetAPPEnvironment().IsDevelopment())
            {
                connectstring = Appsettings.app("MongoDb:Dev_Urls");
            }
            else
            {
                connectstring = Appsettings.app("MongoDb:Pro_Urls");
            }
            if (string.IsNullOrWhiteSpace(connectstring))
            {
                throw new NullReferenceException("连接字符串为空");
            }
            var client = new MongoClient(connectstring);
            var Db_ = client.GetDatabase(Appsettings.app("MongoDb:Database"));
            return Db_;
        }

        public static void DBInit(this IServiceCollection services)
        {

            if (APPEnvironment.GetAPPEnvironment().IsDevelopment())
            {
                connectstring = Appsettings.app("MongoDb:Dev_Urls");
            }
            else
            {
                connectstring = Appsettings.app("MongoDb:Pro_Urls");
            }
            if (string.IsNullOrWhiteSpace(connectstring))
            {
                throw new NullReferenceException("连接字符串为空");
            }
            var client = new MongoClient(connectstring);
            Db = client.GetDatabase(Appsettings.app("MongoDb:Database"));
            Db.InitTables(new List<Type>() {
                typeof(LogFormat),
                typeof(MqttPayload),
                typeof(DeviceDetail)
            });
            services.AddSingleton(Db);
            //services.InitDbSeeds();
        }
        public static IMongoCollection<T> DbGetCollection<T>(this IMongoDatabase mongoDatabase, string collectionname = "null")
        {
            return mongoDatabase.GetCollection<T>((collectionname == "null" || string.IsNullOrEmpty(collectionname)) ? typeof(T).Name : collectionname);
        }
        public static void InitTables(this IMongoDatabase mongoDatabase, List<Type> types)
        {
            foreach (var item in types)
            {
                var filter = new BsonDocument("name", item.Name);
                var collections = mongoDatabase.ListCollections(new ListCollectionsOptions { Filter = filter });
                if (collections.Any() == false)
                {
                    try
                    {
                        mongoDatabase.CreateCollection(item.Name);
                        Console.WriteLine($"------创建【 {item.Name} 】表成功------");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            LogHelper.Log(IdGenFunc.CreateOneId(), "Api服务开始", OprateType.Other);
        }
        public static async void InitTables<T>(this IMongoDatabase mongoDatabase, T t) where T : new()
        {
            var filter = new BsonDocument("name", typeof(T).Name);

            var collections = await mongoDatabase.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });

            if (await collections.AnyAsync() == false)
            {
                await mongoDatabase.DbGetCollection<T>().InsertOneAsync(new T());
            }
        }
    }
}
