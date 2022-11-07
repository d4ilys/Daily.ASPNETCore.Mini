using Daily.ASPNETCore.Mini.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.ASPNETCore.Mini.Controllers
{
    internal interface IControllerFactory
    {
        Task ControllerExecutor(HttpContext context);
    }
}
