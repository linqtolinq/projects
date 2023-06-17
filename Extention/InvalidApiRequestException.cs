namespace MQTT_Api_Server_Lifesaver.Extention
{
    public class InvalidApiRequestException : Exception
    {
        public InvalidApiRequestException(string message) : base(message) { }
    }
}
