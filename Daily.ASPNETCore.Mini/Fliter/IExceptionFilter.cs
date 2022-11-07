using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Fliter.Context;

namespace Daily.ASPNETCore.Mini.Fliter
{
    public interface IExceptionFilter
    {
         void OnException(ExceptionContext exception);
    }
}
