using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.HttpContexts;

namespace Daily.ASPNETCore.Mini.MiddleWare
{
    public interface IApplicationBuilder
    {
        public IApplicationBuilder Use(Action<HttpContext, Action>  middleware)
        {
            return null;
        }

        internal void Build()
        {
        }

        public IApplicationBuilder UseEndLogic(Action<HttpContext> endpoint)
        {
            return this;
        }
    }
}
