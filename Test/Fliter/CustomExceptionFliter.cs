using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Fliter;
using Daily.ASPNETCore.Mini.Fliter.Context;

namespace Test.Fliter
{
    public class CustomExceptionFliter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.IsInterrupt = true;
            context.ActionResult = new
            {
                code = 500,
                msg = "服务器发生异常"
            };
        }
    }
}