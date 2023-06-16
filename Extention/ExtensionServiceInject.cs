

namespace MQTT_Api_Server_Lifesaver.Extention
{
    public static class ExtensionServiceInject
    {
        public static void InjectExtensionService(this IServiceCollection services)
        {
            services.IdGenInit();
            services.InjectAppsettingConf();
            services.AddCorsService();
            services.AddSwaggerGen();
            services.TaskInit();
            services.DBInit();
            services.StartProcessQueue();
        }

        public static void InjectExtensionAPP(this IApplicationBuilder app)
        {
            app.UseCorsService();
        }


    }
}
