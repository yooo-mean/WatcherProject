using Grpc.Core;
using SystemInfoGrpc;

public class SystemInfoServiceImpl : SystemInfoService.SystemInfoServiceBase
{
    public override Task<SystemInfoReply> SendSystemInfo(SystemInfoRequest request, ServerCallContext context)
    {
        Console.WriteLine("���� JSON:");
        Console.WriteLine(request.Json);

        return Task.FromResult(new SystemInfoReply
        {
            Message = "�������� JSON ���� �Ϸ�!"
        });
    }
}