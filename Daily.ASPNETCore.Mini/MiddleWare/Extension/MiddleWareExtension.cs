using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Context;

namespace Daily.ASPNETCore.Mini.MiddleWare.Extension
{
    public static class MiddleWareExtension
    {
        public static IApplicationBuilder UseStaticFile(this IApplicationBuilder builder)
        {
            return builder.Use((context, next) =>
            {
                next();
            });
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder)
        {
            return builder.Use((context, next) =>
            {
                next();
            });
        }
    }
}