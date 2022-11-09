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
            var mvcOption = new MvcOptions();
            option?.Invoke(mvcOption);
            mvcOption.Fliters.ForEach(type =>
            {
                var interfaceType = type.GetInterfaces().FirstOrDefault();
                service.AddTransient(interfaceType, type);
            });
            IControllerActiver controllersBus = new ControllerActiver();
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