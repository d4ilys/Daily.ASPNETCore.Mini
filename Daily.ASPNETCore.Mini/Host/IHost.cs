using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.Host
{
    public interface IHost
    {
        IServiceProvider ServicesProvider { get; set; }

        IServiceCollection Services { get; set; }

        Task StartAsync();

        Task StopAsync();
    }
}