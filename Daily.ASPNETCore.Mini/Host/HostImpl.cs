using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.NettyServer;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.Host
{
    internal class HostImpl:IHost
    {
        public IServiceProvider ServicesProvider { get; set; }

        public IServiceCollection Services { get; set; }

        public async Task StartAsync()
        {
            var nettyServer = Services.BuildServiceProvider().GetService<INettyServer>();
            nettyServer.RunServer(Services).GetAwaiter().GetResult();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
