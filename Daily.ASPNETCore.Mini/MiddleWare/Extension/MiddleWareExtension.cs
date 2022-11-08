using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Common;
using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.MVC;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.MiddleWare.Extension
{
    public static class MiddleWareExtension
    {
        public static IApplicationBuilder UseStaticFile(this IApplicationBuilder builder)
        {
            return builder.Use((context, next) => { next(); });
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder)
        {
            return builder.Use((context, next) => { next(); });
        }

        public static RequestDelegate UseEndpoint(this IApplicationBuilder builder, IServiceCollection services)
        {
            var requestDelegate = builder?.UseEndLogic(async context =>
            {
                //MVC逻辑在这里
                var exector = services.BuildServiceProvider().GetService<IMvcCore>();
                await exector.MvcExecutor(context);
            }).Build();
            return requestDelegate;
        }
    }
}