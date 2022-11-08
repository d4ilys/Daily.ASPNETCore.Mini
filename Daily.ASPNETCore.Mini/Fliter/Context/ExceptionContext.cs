using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.HttpContexts;

namespace Daily.ASPNETCore.Mini.Fliter.Context
{
    public class ExceptionContext
    {
        public HttpContext HttpContext { get; set; }
        public object? ActionResult { get; set; }
        public bool IsInterrupt { get; set; } = false;
        public Exception Exception { get; set; }
    }
}