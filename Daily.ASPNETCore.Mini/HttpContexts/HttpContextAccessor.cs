namespace Daily.ASPNETCore.Mini.HttpContexts
{
    namespace Microsoft.AspNetCore.Http
    {
        public class HttpContextAccessor : IHttpContextAccessor
        {
            private static readonly AsyncLocal<HttpContexts.HttpContext> HttpContextCurrent = new AsyncLocal<HttpContexts.HttpContext>();

            public HttpContexts.HttpContext HttpContext
            {
                get => HttpContextCurrent.Value;
                set => HttpContextCurrent.Value = value;
            }
        }
    }
}
