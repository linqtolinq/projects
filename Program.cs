var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http:*:7256");
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
