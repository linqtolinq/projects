using IdGen;
using MQTT_Api_Server_Lifesaver.DB;
using MQTT_Api_Server_Lifesaver.Extention;
using MQTT_Api_Server_Lifesaver.Model;

namespace MQTT_Api_Server_Lifesaver.Mqtt_GPC
{
    public static class ExhookQueue
    {
        public static ConcurrentQueue<ClientConnectedRequest> mqttconnectdqueue = new ConcurrentQueue<ClientConnectedRequest>();
        public static ConcurrentQueue<ClientDisconnectedRequest> mqttdisconnectdqueue = new ConcurrentQueue<ClientDisconnectedRequest>();
        public static ConcurrentQueue<MqttPayload> MqttPayloadQueue = new ConcurrentQueue<MqttPayload>();
        public static void PushConnQueue(this ClientConnectedRequest client)
        {
            mqttconnectdqueue.Enqueue(client);
        }
        public static void PushDisConnQueue(this ClientDisconnectedRequest client)
        {
            mqttdisconnectdqueue.Enqueue(client);
        }
        public static void PushMsgQueue(this MqttPayload client)
        {
            MqttPayloadQueue.Enqueue(client);
        }
        public static async void StartProcessQueue(this IServiceCollection services)
        {
            await Task.Run(async () =>
            {
                ClientConnectedRequest? clientConnectedRequest = new ClientConnectedRequest();
                ClientDisconnectedRequest? clientDisConnectedRequest = new ClientDisconnectedRequest();
                MqttPayload? mqttPayload = new MqttPayload();
                var db = MongoDBHelper.CreateDbContext();
                while (true)
                {
                    if (mqttconnectdqueue.TryDequeue(out clientConnectedRequest))
                    {
                        LogHelper.LogLogin(IdGenFunc.CreateOneId(), $"MQTT客户端【{clientConnectedRequest.Clientinfo.Clientid}】上线--【{APPTime.GetAPPTime()}】");
                        var filter = Builders<DeviceDetail>.Filter.Eq(r => r.DeviceId, clientConnectedRequest.Clientinfo.Clientid.RemoveWhitespaces());
                        var update = Builders<DeviceDetail>.Update.Set(s => s.OnlineTime, APPTime.GetAPPTime())
                        .Set(s=>s.IsOnline,true);
                        var re = await db.DbGetCollection<DeviceDetail>().FindOneAndUpdateAsync(filter, update);
                        if (re == null)
                        {
                            await db.DbGetCollection<DeviceDetail>().InsertOneAsync(new DeviceDetail()
                            {
                                Id = IdGenFunc.CreateOneId(),
                                DeviceId = clientConnectedRequest.Clientinfo.Clientid.RemoveWhitespaces(),
                                OnlineTime = APPTime.GetAPPTime(),
                                CreateTime = APPTime.GetAPPTime(),
                                OfflineTime = APPTime.GetAPPTime(),
                                IsOnline = true,
                                Remark = "无"
                            }) ;
                        }
                    }
                    if (mqttdisconnectdqueue.TryDequeue(out clientDisConnectedRequest))
                    {
                        LogHelper.LogLogin(IdGenFunc.CreateOneId(), $"MQTT客户端【{clientDisConnectedRequest.Clientinfo.Clientid}】离线--【{APPTime.GetAPPTime()}】");

                        var filter = Builders<DeviceDetail>.Filter.Eq(r => r.DeviceId, clientDisConnectedRequest.Clientinfo.Clientid.RemoveWhitespaces());
                        var update = Builders<DeviceDetail>.Update.Set(s => s.OfflineTime, APPTime.GetAPPTime())
                        .Set(s => s.IsOnline, false);
                        var re = await db.DbGetCollection<DeviceDetail>().FindOneAndUpdateAsync(filter, update);
                        if (re == null)
                        {
                            await db.DbGetCollection<DeviceDetail>().InsertOneAsync(new DeviceDetail()
                            {
                                Id = IdGenFunc.CreateOneId(),
                                DeviceId = clientDisConnectedRequest.Clientinfo.Clientid.RemoveWhitespaces(),
                                OnlineTime = APPTime.GetAPPTime(),
                                CreateTime = APPTime.GetAPPTime(),
                                OfflineTime = APPTime.GetAPPTime(),
                                IsOnline = false,
                                Remark = "无"
                            });
                        }
                    }
                    if (MqttPayloadQueue.TryDequeue(out mqttPayload))
                    {
                        await db.DbGetCollection<MqttPayload>().InsertOneAsync(mqttPayload);
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            });
        }
    }

}
