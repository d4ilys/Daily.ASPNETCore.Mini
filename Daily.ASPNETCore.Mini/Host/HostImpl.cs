using Microsoft.AspNetCore.Builder;
using Daily.ASPNETCore.Mini.NettyServer;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.Host
{
    internal class HostImpl:IHost
    {
        public IServiceProvider ServicesProvider { get; set; }

        public Task StartAsync()
        {
            var nettyServer = ServicesProvider.GetService<INettyServer>();
            nettyServer.RunServer(ServicesProvider).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
