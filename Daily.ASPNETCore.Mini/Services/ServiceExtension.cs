using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Controllers;
using Daily.ASPNETCore.Mini.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.Services
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddControllers(this IServiceCollection service)
        {
            var controllersBus = service.BuildServiceProvider().GetService<IControllersBus>();
            controllersBus.CreateApplicationPartManager(service);
            return service;
        }
    }
}