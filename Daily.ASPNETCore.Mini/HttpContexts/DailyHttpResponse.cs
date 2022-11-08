using DotNetty.Buffers;
using DotNetty.Codecs.Http;

namespace Daily.ASPNETCore.Mini.HttpContexts
{ 
    public class DailyHttpResponse
    {
        public HttpResponseStatus Status { get; set; } = HttpResponseStatus.OK;
        public IByteBuffer Content { get; set; }
        public HttpHeaders Headers { get; set; } = new DefaultHttpHeaders();
    }
}
