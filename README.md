# com.hollysys.Industrial-control-alarm-system  
## ʹ��˵��  
### 1.MySQL  
��û��MySQL���Ȱ�װMySQL����`appsettings.json`�и���`LocalhostConnection`��`Uid`��`Pwd`Ϊ�Լ������ݿ��˺����룬�������д���`industrial-control-alarm-system`���ݿ⣬��ѡ����`appsettings.json`�и���`LocalhostConnection`��`Database`Ϊ������ݿ����ơ�
### 2.���ݿ�Ǩ��
��û��`EF Core.NET �����нӿ� (CLI) ����`��`Visual Studio`���Ȱ�װ����֮һ  
`EF Core.NET �����нӿ� (CLI) ����`��װ��ʽ:��cmd��������`dotnet tool install --global dotnet-ef`������ο�[�ٷ��ĵ�](https://learn.microsoft.com/zh-cn/ef/core/cli/dotnet)
- `EF Core.NET �����нӿ� (CLI) ����`ʹ�÷���������Ŀ��Ŀ¼�´�cmd��������`dotnet ef database update`�������ݿ⣬�⽫����������ݿ��д�����
- `Visual Studio`ʹ�÷�������`NuGet`������������̨������`Update-Database`�������ݿ⣬�⽫����������ݿ��д�����
### 3.Redis˵��
ĿǰRedisʹ����Redis����Ҫʹ�ñ���Redis����`appsettings.json`�и���`Redis`�µ�`ConnectionString`,������ο�[�ٷ��ĵ�](https://weihanli.github.io/StackExchange.Redis-docs-zh-cn/Configuration.html)
### 4.NuGet��ԭ
����Ŀ��Ŀ¼�´�cmd��������`dotnet restore`��ԭNuGet����Visual Studio�Ҽ����������ԭNuGet��
### 5.������Ŀ
����Ŀ��Ŀ¼�´�cmd��������`dotnet run`������Ŀ�����������`http://localhost:5235/swagger/index.html`����Swaggerҳ�棻Visual Studioֱ��������Ŀ����
### 6.��������˵��
����IP��ַ����`appsettings.json`�и���`Ip`��ֵ�����Ķ˿�����`LaunchSettings.json`�и���`applicationUrl`��ֵ,����������������ϵ2440638601@qq.com