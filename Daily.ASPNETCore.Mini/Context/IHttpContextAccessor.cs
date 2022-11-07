using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.ASPNETCore.Mini.Context
{
    public interface IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }
    }
}