using System.Reflection;
using Daily.ASPNETCore.Mini.MVC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.Services
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddControllers(this IServiceCollection service,
            Action<MvcOptions>? option = null)
        {
            //添加MVC配置
            var mvcOption = new MvcOptions();
            option?.Invoke(mvcOption);
            //添加过滤器
            mvcOption.Fliters.ForEach(type =>
            {
                //使用反射注册过滤器
                var interfaceType = type.GetInterfaces().FirstOrDefault();
                service.AddTransient(interfaceType, type);
            });
            //MVC扫描器
            IControllerActiver controllersBus = new ControllerActiver();
            //创建ApplicationPartManager保存扫描到的Controller的信息
            controllersBus.CreateApplicationPartManager(service,mvcOption.Assembly);
            return service;
        }
    }

    public class MvcOptions
    {
        public List<Type> Fliters { get; set; } = new List<Type>();

        public Assembly? Assembly { get; set; } = null;
    }
}