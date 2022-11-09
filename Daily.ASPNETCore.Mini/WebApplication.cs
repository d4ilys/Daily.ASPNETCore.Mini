using Daily.ASPNETCore.Mini.Host;
using Daily.ASPNETCore.Mini.HttpContexts;
using Daily.ASPNETCore.Mini.MiddleWare;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extension;
using Daily.ASPNETCore.Mini.MVC;
using Daily.ASPNETCore.Mini.NettyServer;
using Materal.DotNetty.Server.CoreImpl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini
{
    public class WebApplication : IApplicationBuilder
    {
        public static WebApplicationBuilder CreateBuilder(Action<IConfigurationBuilder>? configBuilder = null) =>
            new(configBuilder);

        public IApplicationBuilder? ApplicationBuilder { get; set; } = null;

        private IHost? Host { get; set; } = null;

        public WebApplication(IHost? host)
        {
            if (host != null)
            {
                Host = host;
                ApplicationBuilder = Host.ServicesProvider.GetService<IApplicationBuilder>();
            }
        }

        /// <summary>
        /// 启动主机 组装管道
        /// </summary>
        public Task Run(string? url = null)
        {
            //TODO：这里在HTTP接收那里创建
            ApplicationBuilder.UseEndpoint(Host.ServicesProvider);

            Host?.StartAsync();

            return Task.CompletedTask;
        }

        public IApplicationBuilder Use(Action<HttpContext, Action> middleware) =>
            ApplicationBuilder?.Use(middleware);
    }
}