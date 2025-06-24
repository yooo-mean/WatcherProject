using Grpc.Core;
using SystemInfoGrpc;

public class SystemInfoServiceImpl : SystemInfoService.SystemInfoServiceBase
{
    public override Task<SystemInfoReply> SendSystemInfo(SystemInfoRequest request, ServerCallContext context)
    {
        Console.WriteLine("받은 JSON:");
        Console.WriteLine(request.Json);

        return Task.FromResult(new SystemInfoReply
        {
            Message = "서버에서 JSON 수신 완료!"
        });
    }
}