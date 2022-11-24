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
        /// <summary>
        /// 创建WebApplicationBuilder
        /// </summary>
        /// <param name="configBuilder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder CreateBuilder(Action<IConfigurationBuilder>? configBuilder = null) =>
            new (configBuilder);

        //管道组装
        public IApplicationBuilder? ApplicationBuilder { get; set; } = null;

        //主机
        private IHost? Host { get; set; } = null;

        //通过构造函数初始化主机
        public WebApplication(IHost? host)
        {
            if (host != null)
            {
                Host = host;
                //IOC容器内获取提前注册好 管道对象
                ApplicationBuilder = Host.ApplicationServices.GetService<IApplicationBuilder>();
            }
        }

        /// <summary>
        /// 启动主机 组装管道
        /// </summary>
        public Task Run(string? url = null)
        {
            //注册终结点，也就是MVC逻辑
            ApplicationBuilder.UseEndpoint(Host.ApplicationServices);

            //启动主机
            Host?.StartAsync();

            return Task.CompletedTask;
        }

        //添加中间件
        public IApplicationBuilder Use(Action<HttpContext, Action> middleware) =>
            ApplicationBuilder?.Use(middleware);
    }
}