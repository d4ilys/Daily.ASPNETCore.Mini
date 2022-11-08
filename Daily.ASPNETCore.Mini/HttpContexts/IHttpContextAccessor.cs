namespace Daily.ASPNETCore.Mini.HttpContexts
{
    public interface IHttpContextAccessor
    {
        public HttpContexts.HttpContext HttpContext { get; set; }
    }
}