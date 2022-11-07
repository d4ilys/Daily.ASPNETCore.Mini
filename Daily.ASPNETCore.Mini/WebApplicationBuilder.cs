using Daily.ASPNETCore.Mini.Common;
using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.Controllers;
using Daily.ASPNETCore.Mini.Host;
using Daily.ASPNETCore.Mini.MiddleWare;
using Daily.ASPNETCore.Mini.NettyServer;
using Materal.DotNetty.Server.CoreImpl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Daily.ASPNETCore.Mini
{
    public class WebApplicationBuilder
    {
        public IServiceCollection Services { get; }

        public IConfiguration Configuration { get; }

        public WebApplicationBuilder(Action<IConfigurationBuilder>? configBuilder = null)
        {
            #region 初始化IConfigruation

            var configurationBuilder = new ConfigurationBuilder();
            //委托扩展
            configBuilder?.Invoke(configurationBuilder);
            //得到IConfiguration实例
            Configuration = configurationBuilder.Build();

            ConsoleHelper.WriteLine($"Configuration initialization completed..");
            #endregion

            //IOC容器实例化
            Services = CreateServiceCollection();
        }

        //创建IOC工厂
        public IServiceCollection CreateServiceCollection()
        {
            var services = new ServiceCollection();
            services.AddTransient<IApplicationBuilder, ApplicationBuilderImpl>();
            services.AddTransient<ChannelHandler>();
            services.AddTransient<INettyServer, NettyServerImpl>();
            services.AddTransient<IHost, HostImpl>();
            services.AddSingleton(Configuration);
            services.AddTransient<IControllersBus, ControllersBus>();
            services.AddTransient<HttpContextDelegate>(s =>
            {
                return context =>
                {
                    services.AddScoped(provider => context);
                };
            });
            ConsoleHelper.WriteLine($"IServiceCollection initialization completed..");
            return services;
        }

        //创建WebApplication
        public WebApplication Build()
        {
            var serviceProvider = Services.BuildServiceProvider();
            var host = serviceProvider.GetService<IHost>();
            host.ServicesProvider = serviceProvider;
            host.Services = Services;
            return new WebApplication(host);
        }
    }
}