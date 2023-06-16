

namespace MQTT_Api_Server_Lifesaver.Extention
{
    public static class GPRC_Service
    {
        public static void InjectGRPC(this WebApplicationBuilder builder)
        {
            builder.Services.AddGrpc();
            builder.Services.AddSingleton<Verifier>();
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 6725, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });

                options.Listen(IPAddress.Any, 6726, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            });
        }
    }
}
