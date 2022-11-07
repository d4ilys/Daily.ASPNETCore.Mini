using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.Host;
using Daily.ASPNETCore.Mini.MiddleWare;
using Daily.ASPNETCore.Mini.MiddleWare.Extension;
using Daily.ASPNETCore.Mini.MVC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini
{
    public class WebApplication : IApplicationBuilder
    {
        public static WebApplicationBuilder CreateBuilder(Action<IConfigurationBuilder>? configBuilder = null) =>
            new(configBuilder);

        private IApplicationBuilder? _applicationBuilder { get; set; } = null;

        private IHost? _host { get; set; } = null;

        public WebApplication(IHost host)
        {
            if (host != null)
            {
                _host = host;
                _applicationBuilder = _host.ServicesProvider.GetService<IApplicationBuilder>();
            }
        }

        /// <summary>
        /// 启动主机 组装管道
        /// </summary>
        public Task Run(string? url = null)
        {
            var requestDelegate = _applicationBuilder.UseEndpoint(_host.Services);

            _host.Services.AddTransient<RequestDelegate>(provider => requestDelegate);

            _host?.StartAsync();

            return Task.CompletedTask;
        }

        public IApplicationBuilder Use(Action<HttpContext, Action> middleware) =>
            _applicationBuilder?.Use(middleware);
    }
}