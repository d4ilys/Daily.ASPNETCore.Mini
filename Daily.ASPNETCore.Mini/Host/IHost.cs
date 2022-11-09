using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.Host
{
    public interface IHost
    {
        IServiceProvider ServicesProvider { get; set; }

        Task StartAsync();

        Task StopAsync();
    }
}