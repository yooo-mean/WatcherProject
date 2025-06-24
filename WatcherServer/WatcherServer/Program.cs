var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<SystemInfoServiceImpl>(); // ���⿡ ���
app.MapGet("/", () => "gRPC �����Դϴ�. ������ ��� Ŭ���̾�Ʈ�� ����ϼ���.");

app.Run();
