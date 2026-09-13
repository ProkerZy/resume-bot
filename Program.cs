using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using TgBot;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["DbConnection"]));
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IVacancySource, TrudvsemVacancySource>();

var host = builder.Build();
host.Run();
