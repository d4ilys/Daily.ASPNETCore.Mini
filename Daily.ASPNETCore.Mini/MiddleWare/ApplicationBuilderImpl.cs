using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Common;
using Daily.ASPNETCore.Mini.Context;
using static Daily.ASPNETCore.Mini.MiddleWare.ApplicationBuilderImpl;

namespace Daily.ASPNETCore.Mini.MiddleWare
{
    /// <summary>
    /// 实现中间件逻辑
    /// </summary>
    /// <typeparam name="HttpContext"></typeparam>
    public class ApplicationBuilderImpl : IApplicationBuilder
    {
        private Action<HttpContext> _completeFunc { get; set; } = null;

        private readonly IList<Func<Action<HttpContext>, Action<HttpContext>>> _pipelines =
            new List<Func<Action<HttpContext>, Action<HttpContext>>>();

        public IApplicationBuilder UseEndpoint(Action<HttpContext> endpoint)
        {
            _completeFunc = endpoint;
            return this;
        }

        public IApplicationBuilder Use(Action<HttpContext, Action> middleware)
        {
            //先不执行，存储到一个委托的集合
            //这里传递委托最重要！
            Func<Action<HttpContext>, Action<HttpContext>> middlewareFunc = next =>
            {
                return context =>
                {
                    //执行自定义中间件传递的委托
                    middleware.Invoke(context, () =>  //这个委托在自定义中间件中的执行
                    {
                        next(context); //真正的next
                    });
                };
            };
            _pipelines.Add(middlewareFunc);
            return this;
        }

        //组装中间件
        public RequestDelegate Build()
        {
            //这个是兜底的
            var request = _completeFunc;
            foreach (var pipeline in _pipelines.Reverse())
            {
                request = pipeline(request);
            }
            ConsoleHelper.WriteLine($"The pipeline has been assembled complete..");
            return new RequestDelegate(request);
        }
    }
}