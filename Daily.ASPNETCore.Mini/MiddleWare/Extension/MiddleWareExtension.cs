﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Common;
using Daily.ASPNETCore.Mini.HttpContexts;
using Daily.ASPNETCore.Mini.MiddleWare;
using Daily.ASPNETCore.Mini.MVC;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder.Extension
{
    public static class MiddleWareExtension
    {
        public static IApplicationBuilder UseStaticFile(this IApplicationBuilder builder)
        {
            return builder.Use((context, next) => { next(); });
        }

        public static void UseEndpoint(this IApplicationBuilder builder, IServiceProvider services)
        {
            builder?.UseEndpointMiddleware(async context =>
            {
                //MVC逻辑在这里
                var exector = services.GetService<IMvcCore>();
                await exector.MvcExecutor(context);
            }).Build();
        }
    }
}