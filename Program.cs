using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using MQTT_Api_Server_Lifesaver.Mqtt_GPC;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.InjectAppsettingConf();
builder.Services.AddControllers();
builder.Services.AddCorsService();
builder.InjectGRPC();
builder.WebHost.UseUrls(builder.Configuration.GetValue<string>("StartUrl"));

var app = builder.Build();

app.UseRouting();
app.UseCorsService();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/StaticFiles"
});


app.UseEndpoints(op => {
    op.MapControllers();
    op.MapGrpcService<MQTTGrpcService>();
    op.Map("/", () => Results.Redirect("/staticfiles/404.html#")).RequireCors(t => t.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

app.Run();
