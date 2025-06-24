var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<SystemInfoServiceImpl>(); // 여기에 등록
app.MapGet("/", () => "gRPC 서버입니다. 브라우저 대신 클라이언트를 사용하세요.");

app.Run();
