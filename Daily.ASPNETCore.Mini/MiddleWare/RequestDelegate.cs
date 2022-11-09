using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.HttpContexts;

namespace Microsoft.AspNetCore.Builder
{
    public delegate void RequestDelegate(HttpContext context);
}
