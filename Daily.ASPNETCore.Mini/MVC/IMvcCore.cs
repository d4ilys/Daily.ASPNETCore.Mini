using Daily.ASPNETCore.Mini.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Codecs.Http.WebSockets;

namespace Daily.ASPNETCore.Mini.MVC
{
    internal interface IMvcCore
    {
        Task MvcExecutor(HttpContext context);
    }
}
