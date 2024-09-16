using Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Server.Services;
using System.Reflection;
using Microsoft.Extensions.Caching.Distributed;
using Server.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

#region  ˝æ›ø‚≈‰÷√
var connectionString = builder.Configuration.GetConnectionString("MySQLLocalhostConnection");
builder.Services.AddDbContext<IndustrialControlAlarmSystemContext>(options =>
  options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
#endregion
// Add services to the container.

builder.Services.AddSignalR();

//øÁ”Ú≈‰÷√
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
    options.AddPolicy("ws",policy =>
    {
        policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

#region redis≈‰÷√
var RedisConnectionString = builder.Configuration.GetConnectionString("MyRedisConStr");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = RedisConnectionString;
    options.InstanceName = "";
});
builder.Services.AddSingleton<RedisCacheService>(sp =>
{
    var cache = sp.GetRequiredService<IDistributedCache>();
    return new RedisCacheService(cache, RedisConnectionString!);
});
// ≈‰÷√ RedisCacheService
//builder.Services.AddSingleton(new RedisCacheService(RedisConnectionString!));
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // ∆Ù”√ XML ◊¢ Õ£®«Î»∑±£…˙≥… XML Œƒµµ£©
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.Run();
