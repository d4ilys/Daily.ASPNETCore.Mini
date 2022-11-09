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
            var nettyServer = ApplicationServices.GetService<INettyServer>();
            nettyServer.RunServer(ApplicationServices).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
