using MongoDB.Bson.Serialization.Attributes;

namespace MQTT_Api_Server_Lifesaver.Model
{
    public class DeviceDetail
    {
        [BsonElement("_id")]
        public long Id { get; set; } 
        public string? DeviceId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime OnlineTime { get; set; }
        public DateTime OfflineTime { get; set; }
        public string? Remark { get; set; }

    }
}
