using System.Text.Json;
using System.Text.Json.Serialization;

namespace MQTT_Api_Server_Lifesaver.Extention
{
    public class NumberConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //Console.WriteLine();
            return long.Parse(reader.GetString() ?? "0");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
