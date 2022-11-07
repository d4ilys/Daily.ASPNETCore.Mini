using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Controllers;
using Daily.ASPNETCore.Mini.Fliter;
using Daily.ASPNETCore.Mini.Models;
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
            var controllersBus = service.BuildServiceProvider().GetService<IControllerActiver>();
            controllersBus.CreateApplicationPartManager(service);
            return service;
        }
    }

    public class MvcOptions
    {
        public List<Type> Fliters { get; set; } = new List<Type>();
    }
}