using Daily.ASPNETCore.Mini.Common;
using Daily.ASPNETCore.Mini.Host;
using Daily.ASPNETCore.Mini.HttpContexts;
using Daily.ASPNETCore.Mini.HttpContexts.Microsoft.AspNetCore.Http;
using Daily.ASPNETCore.Mini.MiddleWare;
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
            services.AddTransient<IApplicationBuilder, ApplicationBuilderImpl>();
            services.AddTransient<ChannelHandler>();
            services.AddTransient<INettyServer, NettyServerImpl>();
            services.AddTransient<IHost, HostImpl>();
            services.AddSingleton<IControllerActiver, ControllerActiver>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);
            services.AddTransient<IMvcCore, MvcCoreImpl>();
            _servicesAction.ForEach(action => action.Invoke(services));
            ConsoleHelper.WriteLine($"IServiceCollection initialization completed..");
            return services;
        }

        //创建WebApplication
        public WebApplication Build()
        {
            //todo:这里是错误的
            var serviceProvider = Services.BuildServiceProvider();
            var host = serviceProvider.GetService<IHost>();
            host.ServicesProvider = serviceProvider;
            return new WebApplication(host);
        }
    }
}