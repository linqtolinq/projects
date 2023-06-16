namespace MQTT_Api_Server_Lifesaver.Extention
{
    public static class APPTime
    {
        public static DateTime GetAPPTime()
        {
            return DateTime.Now;
        }
    }

    public static class APPEnvironment
    {
        private static IWebHostEnvironment? appenvironment;
        public static void ConfigAPPEnvironment(this WebApplicationBuilder builder)
        { 
            appenvironment = builder.Environment;
        }
        public static IWebHostEnvironment GetAPPEnvironment()
        {
            return appenvironment!;
        }
    }
}
