using DemoBot.Models.Configurations;
using DemoBot.Web.Extensions;
using Newtonsoft;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAppConfigurations(builder.Configuration);
builder.Host.ConfigureCustomLogging();

builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddCustomRouting();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDevTools();
builder.Services.AddCors(setup =>
{
    setup.AddPolicy("DefaultPolicy", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCustomSwagger();
app.UseCors("DefaultPolicy");

app.UseHttpsRedirection();
app.UseMiddleware<TestMiddleware>();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    var token = app.Configuration.GetConfiguration<BotConfigurations>().AuthToken;
    endpoints.MapControllerRoute
    (
        name: "TelegramWebHookRoute",
        pattern: $"bot/{token}",
        new { controller = "WebHookController", action = "Post" }
    );
    endpoints.MapControllers();
});
app.Run();
