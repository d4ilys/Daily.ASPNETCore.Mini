using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.NettyServer
{
    public interface INettyServer
    {
        public Task RunServer(IServiceCollection service);
    }
}
