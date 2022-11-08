using DotNetty.Codecs.Http;
using System;
using Daily.ASPNETCore.Mini.HttpContexts;

namespace Daily.ASPNETCore.Mini.MVC.HttpAttribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class HttpMethodAttribute : Attribute
    {
        protected string _name;

        public HttpMethodAttribute(string name)
        {
            _name = name;
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected bool IsMatch(string method)
        {
            return _name.Equals(method, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public bool Handler(HttpContext context)
        {
            if (!IsMatch(context.Request.Method.Name))
            {
                context.Response.Status = HttpResponseStatus.BadRequest;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}