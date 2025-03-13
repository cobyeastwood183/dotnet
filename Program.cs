using Sentry;
using System.Threading;
using App.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Sentry
builder.WebHost.UseSentry(o =>
{
    o.Dsn = "";
    o.Debug = true;
});

var app = builder.Build();

app.UseRouting();

CronJobService.ConfigureCronJobEndpoints(app);

app.Run();
