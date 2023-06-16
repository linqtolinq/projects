using MongoDB.Bson.Serialization.Attributes;

namespace MQTT_Api_Server_Lifesaver.Model
{
    public class MqttPayload
    {
        [BsonElement("_id")]
        public long Id { get; set; } // ID，主键
        public string? ClientId { get; set; } 
        public string? Node { get; set; } 
        public string? Qos { get; set; } 
        public string? From { get; set; } 
        public string? Topic { get; set; }
        public string? Payload { get; set; } 
        public DateTime CreateTime { get; set; }
        public MqttPayload()
        {
            this.CreateTime = APPTime.GetAPPTime();
        }
    }
}
