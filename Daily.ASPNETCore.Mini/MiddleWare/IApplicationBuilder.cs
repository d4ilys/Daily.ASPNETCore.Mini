using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Context;

namespace Daily.ASPNETCore.Mini.MiddleWare
{
    public interface IApplicationBuilder
    {
        public IApplicationBuilder Use(Action<HttpContext, Action>  middleware)
        {
            return null;
        }

        internal RequestDelegate Build()
        {
            return null;
        }

        public IApplicationBuilder UseEndLogic(Action<HttpContext> endpoint)
        {
            return this;
        }
    }
}
