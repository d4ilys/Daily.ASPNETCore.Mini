using Microsoft.AspNetCore.Builder;
using Daily.ASPNETCore.Mini.NettyServer;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.Host
{
    internal class HostImpl:IHost
    {
        public IServiceProvider ApplicationServices { get; set; }

        public Task StartAsync()
        {
            //容器中获取提前准备好的 NettyServer
            var nettyServer = ApplicationServices.GetService<INettyServer>();
            //启动
            nettyServer.RunServer(ApplicationServices).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
