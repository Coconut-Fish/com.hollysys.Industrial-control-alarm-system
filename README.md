# com.hollysys.Industrial-control-alarm-system  
## 使用说明  
### 1.MySQL  
若没有MySQL请先安装MySQL，在`appsettings.json`中更改`LocalhostConnection`的`Uid`和`Pwd`为自己的数据库账号密码，并在其中创建`industrial-control-alarm-system`数据库，或选择在`appsettings.json`中更改`LocalhostConnection`的`Database`为你的数据库名称。
### 2.数据库迁移
若没有`EF Core.NET 命令行接口 (CLI) 工具`或`Visual Studio`请先安装其中之一  
`EF Core.NET 命令行接口 (CLI) 工具`安装方式:打开cmd窗口输入`dotnet tool install --global dotnet-ef`详情请参考[官方文档](https://learn.microsoft.com/zh-cn/ef/core/cli/dotnet)
- `EF Core.NET 命令行接口 (CLI) 工具`使用方法：在项目根目录下打开cmd窗口输入`dotnet ef database update`更新数据库，这将会在你的数据库中创建表
- `Visual Studio`使用方法：在`NuGet`包管理器控制台中输入`Update-Database`更新数据库，这将会在你的数据库中创建表
### 3.Redis说明
目前Redis使用云Redis，若要使用本地Redis请在`appsettings.json`中更改`Redis`下的`ConnectionString`,详情请参考[官方文档](https://weihanli.github.io/StackExchange.Redis-docs-zh-cn/Configuration.html)
### 4.NuGet还原
在项目根目录下打开cmd窗口输入`dotnet restore`还原NuGet包；Visual Studio右键解决方案还原NuGet包
### 5.启动项目
在项目根目录下打开cmd窗口输入`dotnet run`启动项目，浏览器输入`http://localhost:5235/swagger/index.html`进入Swagger页面；Visual Studio直接启动项目即可
### 6.其他配置说明
更改IP地址请在`appsettings.json`中更改`Ip`的值，更改端口请在`LaunchSettings.json`中更改`applicationUrl`的值,若有其他问题请联系2440638601@qq.com