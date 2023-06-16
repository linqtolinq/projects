using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using MQTT_Api_Server_Lifesaver.DB;
using MQTT_Api_Server_Lifesaver.Mqtt_GPC;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigAPPEnvironment();
builder.Services.InjectExtensionService();
builder.Services.AddControllers();
builder.InjectGRPC();

//builder.WebHost.UseUrls(builder.Configuration.GetValue<string>("StartUrl"));



var app = builder.Build();

app.UseRouting();
app.InjectExtensionAPP();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/StaticFiles"
});

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEndpoints(op =>
{
    op.MapControllers();
    op.MapGrpcService<MQTTGrpcService>();
    op.Map("/", () => Results.Redirect("/staticfiles/404.html#")).RequireCors(t => t.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

app.Run();
