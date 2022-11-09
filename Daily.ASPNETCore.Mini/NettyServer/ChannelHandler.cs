using System.Diagnostics;
using DotNetty.Codecs.Http;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Daily.ASPNETCore.Mini.NettyServer;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using System.Net.Http;
using Daily.ASPNETCore.Mini.Common;
using Daily.ASPNETCore.Mini.HttpContexts;
using Microsoft.Extensions.DependencyInjection;

namespace Materal.DotNetty.Server.CoreImpl
{
    public class ChannelHandler : SimpleChannelInboundHandler<IFullHttpRequest>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _contextAccessor;

        public ChannelHandler(IServiceProvider serviceProvider,
            IHttpContextAccessor contextAccessor)
        {
            _serviceProvider = serviceProvider;
            _contextAccessor = contextAccessor;
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
                //AsyncLocal线程安全
                _contextAccessor.HttpContext = httpContext;
                //创建Scope
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    httpContext.RequestService = serviceScope.ServiceProvider;
                    RequestDelegateProvider.CreateRequestDelegate()(httpContext);
                    //返回Response断开Http连接
                    await SendHttpResponseAsync(ctx, httpContext.Response, httpContext);
                }
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