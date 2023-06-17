

using Microsoft.AspNetCore.Mvc;

namespace MQTT_Api_Server_Lifesaver.Extention
{
    public static class ExtensionServiceInject
    {
        public static void InjectExtensionService(this IServiceCollection services)
        {
            services.IdGenInit();
            services.InjectAppsettingConf();
            services.AddCorsService();
            //services.AddSwaggerGen();
            services.TaskInit();
            services.DBInit();
            services.StartProcessQueue();
            // 禁止使用MVC自带的参数校验方案
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public static void InjectExtensionAPP(this IApplicationBuilder app)
        {
            app.UseCorsService();
        }


    }
}
