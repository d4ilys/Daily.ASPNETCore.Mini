using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Fliter;
using Daily.ASPNETCore.Mini.Fliter.Context;

namespace Web.Fliter
{
    public class CustomActionFliter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("方法执行前");
            //断路器
            //context.IsInterrupt = true;
            //context.ActionResult = new
            //{
            //    success = 0,
            //    msg = "执行前被改了"
            //};
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("方法执行后");
            //断路器
            //context.IsInterrupt = true;
            //context.ActionResult = new
            //{
            //    success = 0,
            //    msg = "执行后被改了"
            //};
        }
    }
}
