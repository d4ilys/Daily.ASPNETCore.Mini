using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.ASPNETCore.Mini.Context
{
    using System.Threading;

    namespace Microsoft.AspNetCore.Http
    {
        public class HttpContextAccessor : IHttpContextAccessor
        {
            private static AsyncLocal<HttpContext> _httpContextCurrent = new AsyncLocal<HttpContext>();

            public HttpContext HttpContext
            {
                get
                {
                    return _httpContextCurrent.Value;
                }
                set
                {
                    _httpContextCurrent.Value = value;
                }
            }
        }
    }
}
