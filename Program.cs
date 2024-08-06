using com.hollysys.Industrial_control_alarm_system.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyProject.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region 数据库配置
var connectionString = builder.Configuration.GetConnectionString("LocalhostConnection");
builder.Services.AddDbContext<IndustrialControlAlarmSystemContext>(options =>
  options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
#endregion
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // 启用 XML 注释（请确保生成 XML 文档）
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// 配置 RedisCacheService
var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;
builder.Services.AddSingleton(new RedisCacheService(redisConnectionString!));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
