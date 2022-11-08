using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Fliter.Context;

namespace Daily.ASPNETCore.Mini.Fliter
{
    public interface IActionFilter
    {
        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="context"></param>
        void OnActionExecuting(ActionExecutingContext context);

        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="context"></param>
        void OnActionExecuted(ActionExecutedContext context);
    }
}
