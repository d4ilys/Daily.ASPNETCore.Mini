using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Codecs.Http;

namespace Daily.ASPNETCore.Mini.Context
{
    public class HttpContext
    {
        public DailyHttpResponse? Response { get; set; }

        public IFullHttpRequest? Request { get; set; }
    }
}