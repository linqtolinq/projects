using Microsoft.Extensions.FileProviders;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigAPPEnvironment();
builder.Services.InjectExtensionService();
builder.Services.AddControllers().AddJsonOptions(
        options => {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new NumberConverter());
        }); ;
builder.InjectGRPC();

builder.Services.AddSignalR();
builder.Services.AddHostedService<Worker>();
//builder.WebHost.UseUrls(builder.Configuration.GetValue<string>("StartUrl"));

var app = builder.Build();
app.UseMiddleware<ErrorHandExtension>();
app.UseRouting();
app.InjectExtensionAPP();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/StaticFiles"
});

//if (builder.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseEndpoints(op =>
{
    op.MapHub<ClockHub>("/hubs/clock");
    op.MapControllers();
    op.MapGrpcService<MQTTGrpcService>();
    op.Map("/", () => Results.Redirect("/staticfiles/404.html#")).RequireCors(t => t.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

app.Run();
