using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Context;

namespace Daily.ASPNETCore.Mini.MiddleWare
{
    public delegate void RequestDelegate(HttpContext context);
}
