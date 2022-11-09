using Daily.ASPNETCore.Mini.Common;
using Daily.ASPNETCore.Mini.Host;
using Daily.ASPNETCore.Mini.HttpContexts;
using Daily.ASPNETCore.Mini.HttpContexts.Microsoft.AspNetCore.Http;
using Daily.ASPNETCore.Mini.MiddleWare;
using Microsoft.AspNetCore.Builder;
using Daily.ASPNETCore.Mini.MVC;
using Daily.ASPNETCore.Mini.NettyServer;
using Materal.DotNetty.Server.CoreImpl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini
{
    public class WebApplicationBuilder
    {
        public IServiceCollection Services { get; set; }

        private readonly List<Action<IServiceCollection>> _servicesAction = new List<Action<IServiceCollection>>();

        public void ConfigService(Action<IServiceCollection> action)
        {
            _servicesAction.Add(action);
        }

        public IConfiguration Configuration { get; }

        public WebApplicationBuilder(Action<IConfigurationBuilder>? configBuilder = null)
        {
            #region 初始化IConfigruation

            var configurationBuilder = new ConfigurationBuilder();
            //委托扩展
            configBuilder?.Invoke(configurationBuilder);
            //得到IConfiguration实例
            Configuration = configurationBuilder.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"))
                .Build();

            ConsoleHelper.WriteLine($"Configuration initialization completed..");

            #endregion

            //IOC容器实例化
            Services = CreateServiceCollection();
        }

        //创建IOC工厂
        public IServiceCollection CreateServiceCollection()
        {
            var services = new ServiceCollection();
            //中间件创建
            services.AddTransient<IApplicationBuilder, ApplicationBuilderImpl>();
            //HttpHandler
            services.AddTransient<ChannelHandler>();
            //NettyServer
            services.AddTransient<INettyServer, NettyServerImpl>();
            //主机
            services.AddTransient<IHost, HostImpl>();
            //HttpContext存储器
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //IConfiguration
            services.AddSingleton(Configuration);
            //Mvc核心逻辑
            services.AddTransient<IMvcCore, MvcCoreImpl>();
            //执行临时委托集合中的注册
            _servicesAction.ForEach(action => action.Invoke(services));
            ConsoleHelper.WriteLine($"IServiceCollection initialization completed..");
            return services;
        }

        //创建WebApplication
        public WebApplication Build()
        {
            //生成IServiceProvider
            var serviceProvider = Services.BuildServiceProvider();
            //实例化Host
            var host = serviceProvider.GetService<IHost>();
            //将IServiceProvider赋值给host中的属性，方面下端使用
            host.ApplicationServices = serviceProvider;
            //创建返回新的WebApplication对象，构造函数传入Host
            return new WebApplication(host);
        }
    }
}