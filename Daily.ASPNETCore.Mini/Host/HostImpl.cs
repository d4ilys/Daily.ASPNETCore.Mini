using Daily.ASPNETCore.Mini.NettyServer;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.Host
{
    internal class HostImpl:IHost
    {
        public IServiceProvider ServicesProvider { get; set; }

        public IServiceCollection Services { get; set; }

        public Task StartAsync()
        {
            var nettyServer = Services.BuildServiceProvider().GetService<INettyServer>();
            nettyServer.RunServer(Services).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
