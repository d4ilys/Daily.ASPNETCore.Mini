using DotNetty.Codecs.Http;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.MiddleWare;
using Daily.ASPNETCore.Mini.NettyServer;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using System.Net.Http;

namespace Materal.DotNetty.Server.CoreImpl
{
    public class ChannelHandler : SimpleChannelInboundHandler<IFullHttpRequest>
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly HttpContextDelegate _httpContextDelegate;

        public ChannelHandler(RequestDelegate requestDelegate, HttpContextDelegate httpContextDelegate)
        {
            _requestDelegate = requestDelegate;
            _httpContextDelegate = httpContextDelegate;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IFullHttpRequest request)
        {
            request.Retain(byte.MaxValue);
            Task.Run(async () =>
            {
                //得到Request开始执行中间件
                var httpContext = new HttpContext();
                httpContext.Request = request;
                httpContext.Response = new DailyHttpResponse();
                _httpContextDelegate.Invoke(httpContext);
                _requestDelegate.Invoke(httpContext);
                //返回Response断开Http连接
                await SendHttpResponseAsync(ctx, httpContext.Response, httpContext);
            });
        }

        /// <summary>
        /// 发送Http返回
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="response"></param>
        private async Task SendHttpResponseAsync(IChannelHandlerContext ctx, DailyHttpResponse response,
            HttpContext context)
        {
            IFullHttpResponse result;
            if (response.Content != null)
            {
                result = new DefaultFullHttpResponse(HttpVersion.Http11, response.Status, response.Content);
            }
            else
            {
                result = new DefaultFullHttpResponse(HttpVersion.Http11, response.Status);
            }

            //默认的Header
            foreach ((AsciiString key, object value) in GetDefaultHeaders())
            {
                result.Headers.Set(key, value);
            }

            if (response.Headers != null)
            {
                foreach (var responseHeader in response?.Headers)
                {
                    result.Headers.Set(responseHeader.Key, responseHeader.Value);
                }
            }

            context.Request.Retain(0);
            await ctx.Channel.WriteAndFlushAsync(result);
            await ctx.CloseAsync();
        }

        /// <summary>
        /// 获得默认的HttpHeaders
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private Dictionary<AsciiString, object> GetDefaultHeaders(string contentType = null)
        {
            var result = new Dictionary<AsciiString, object>
            {
                { HttpHeaderNames.Date, DateTime.Now },
                { HttpHeaderNames.Server, "Daily" },
                { HttpHeaderNames.AcceptLanguage, "zh-CN,zh;q=0.9" }
            };
            if (!string.IsNullOrEmpty(contentType))
            {
                result.Add(HttpHeaderNames.ContentType, contentType);
            }

            return result;
        }
    }
}