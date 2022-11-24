using DotNetty.Codecs.Http;

namespace Daily.ASPNETCore.Mini.HttpContexts
{
    public class HttpContext
    {
        public DailyHttpResponse? Response { get; set; }

        public IFullHttpRequest? Request { get; set; }

        public IServiceProvider? RequestServices { get; set; }
    }
}