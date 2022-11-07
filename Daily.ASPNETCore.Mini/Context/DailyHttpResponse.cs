using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;

namespace Daily.ASPNETCore.Mini.Context
{ 
    public class DailyHttpResponse
    {
        public HttpResponseStatus Status { get; set; } = HttpResponseStatus.OK;
        public IByteBuffer Content { get; set; }
        public HttpHeaders Headers { get; set; } = new DefaultHttpHeaders();
    }
}
