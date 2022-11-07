using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Common;
using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.Controllers.HttpAttribute;
using Daily.ASPNETCore.Mini.Fliter;
using Daily.ASPNETCore.Mini.Fliter.Context;
using Daily.ASPNETCore.Mini.Models;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Daily.ASPNETCore.Mini.Controllers
{
    internal class ControllerFactory : IControllerFactory
    {
        //执行MVC
        public async Task ControllerExecutor(HttpContext context)
        {
            var service = context.ServiceProvider;
            try
            {
                var applicationPartManager = service.GetService<ApplicationPartManager>();
                //根据路由匹配对应的方法
                var (controllerName, actionName, isSuccess) = RouteMatch(context);
                //如果匹配失败直接返回400
                if (!isSuccess)
                {
                    context.Response.Status = HttpResponseStatus.BadRequest;
                    return;
                }

                //根据路由名称查找控制器Type
                var applicationParts =
                    applicationPartManager.ApplicationParts.Where(part => part.Name == controllerName);
                //如果存在控制器接着处理
                if (applicationParts.Any())
                {
                    //根据路由匹配对应的控制器
                    var controllerType = applicationParts.FirstOrDefault().Type;
                    //创建控制器的实例
                    var controller = CreateController(controllerType, context);

                    //处理路由标记特性
                    var routeActionMethed = controllerType.GetMethods()
                        .Where(m => m.GetCustomAttribute<RouteAttribute>()?.Key == actionName).FirstOrDefault();
                    if (routeActionMethed != null)
                    {
                        actionName = routeActionMethed.Name;
                    }

                    var methodInfo = controllerType.GetMethod(actionName);
                    //如果没有方法抛出异常
                    if (methodInfo == null)
                    {
                        context.Response.Status = HttpResponseStatus.BadRequest;
                        return;
                    }

                    //处理Http请求类型特性
                    var httpMethodAttribute = methodInfo.GetCustomAttribute<HttpMethodAttribute>();
                    var httpMethodHandlerResult = httpMethodAttribute?.Handler(context);
                    if (httpMethodHandlerResult != null && !httpMethodHandlerResult.Value)
                        return;

                    //获取Body类型的参数
                    string bodyParams = GetBodyParams(context);
                    //获取URL上的参数
                    var urlParams = GetUrlParams(context);
                    //获取方法上的参数
                    var parameters = methodInfo.GetParameters();
                    object? actionResult;
                    //Action过滤器
                    var actionFilter = service.GetService<IActionFilter>();
                    //Action执行前
                    var actionExecutingContext = new ActionExecutingContext() { HttpContext = context };
                    if (actionFilter != null)
                    {
                        actionFilter.OnActionExecuting(actionExecutingContext);
                    }

                    //如果中断执行
                    if (!actionExecutingContext.IsInterrupt)
                    {
                        //如果有参数则绑定参数
                        if (parameters.Length > 0)
                        {
                            object[] @params = new object[parameters.Length];
                            for (var i = 0; i < parameters.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(bodyParams) && parameters[i].ParameterType.IsClass &&
                                    parameters[i].ParameterType != typeof(string))
                                {
                                    @params[i] = JsonConvert.DeserializeObject(bodyParams, parameters[i].ParameterType);
                                    continue;
                                }

                                if (urlParams.ContainsKey(parameters[i].Name))
                                {
                                    @params[i] = urlParams[parameters[i].Name].ConvertTo(parameters[i].ParameterType);
                                    continue;
                                }

                                if (@params[i] == null)
                                {
                                    break;
                                }
                            }

                            //绑定完成参数后执行这个方法
                            actionResult = methodInfo.Invoke(controller, @params);
                        }
                        else
                        {
                            //没有参数直接执行
                            actionResult = methodInfo.Invoke(controller, null);
                        }
                    }
                    else
                    {
                        actionResult = actionExecutingContext.ActionResult;
                    }

                    //Action执行后
                    var actionExecutedContext = new ActionExecutedContext()
                    {
                        HttpContext = context
                    };
                    //执行过滤器
                    if (actionFilter != null)
                    {
                        actionFilter.OnActionExecuted(actionExecutedContext);
                    }

                    //如果中断替换返回值
                    if (actionExecutedContext.IsInterrupt)
                    {
                        actionResult = actionExecutedContext.ActionResult;
                    }

                    //处理返回值类型
                    if (actionResult is Task task)
                    {
                        dynamic taskObj = task;
                        actionResult = await taskObj;
                    }

                    if (actionResult != null)
                    {
                        ResultHandler(actionResult, context);
                    }
                }
                else
                {
                    context.Response.Status = HttpResponseStatus.BadRequest;
                    return;
                }
            }
            catch (Exception e)
            {
                //触发异常过滤器
                var exceptionFilter = service.GetService<IExceptionFilter>();
                if (exceptionFilter != null)
                {
                    var exceptionContext = new ExceptionContext
                    {
                        HttpContext = context
                    };
                    exceptionFilter.OnException(exceptionContext);
                    if (exceptionContext.IsInterrupt)
                    {
                        ResultHandler(exceptionContext.ActionResult, context);
                    }
                }
            }
        }

        /// <summary>
        /// 返回值处理
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public void ResultHandler(object actionResult, HttpContext context)
        {
            if (actionResult is Stream stream)
            {
                GetStreamResponse(stream, context);
            }
            else if (actionResult.GetType().IsClass && !(actionResult is string))
            {
                GetJsonResponse(actionResult, context);
            }
            else
            {
                GetTextResponse(actionResult, context);
            }
        }

        /// <summary>
        /// 创建控制器实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public object CreateController(Type type, HttpContext context)
        {
            var service = context.ServiceProvider;
            //构造函数注入
            //获取所有构造函数
            var contruncts = type.GetConstructors();
            var constructor = contruncts.OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();
            var constructorParamList = new List<object>();
            //有参数开始构造
            if (constructor.GetParameters().Any())
            {
                foreach (var paramInfo in constructor.GetParameters())
                {
                    var paramObject = service.GetService(paramInfo.ParameterType);
                    constructorParamList.Add(paramObject);
                }
            }

            //创建实例
            var controllerInstance = constructorParamList.Any()
                ? Activator.CreateInstance(type, constructorParamList.ToArray())
                : Activator.CreateInstance(type);
            //模拟SpringBoot字段注解注入
            foreach (var propertyInfo in type.GetProperties().Where(p => p.IsDefined(typeof(AutowiredAttribute), true)))
            {
                var propValue = service.GetService(propertyInfo.PropertyType);
                propertyInfo.SetValue(controllerInstance, propValue);
            }

            return controllerInstance;
        }

        /// <summary>
        /// 路由匹配
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Tuple<string, string, bool> RouteMatch(HttpContext context)
        {
            string controllerName = string.Empty;
            string actionName = string.Empty;
            bool isSucces = true;
            if (!context.Request.Uri.StartsWith("/api/"))
            {
                isSucces = false;
            }
            else
            {
                string[] urls = context.Request.Uri.Split("/");
                if (urls.Length <= 4)
                {
                    controllerName = urls[2];
                    actionName = urls[3].Split("?")[0];
                }
            }

            return new Tuple<string, string, bool>(controllerName, actionName, isSucces);
        }

        /// <summary>
        /// 获取流返回
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private void GetStreamResponse(Stream stream, HttpContext context)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            stream.Dispose();
            context.Response.Content = Unpooled.WrappedBuffer(bytes);
            var mime = "application/octet-stream";
            if (!context.Response.Headers.Contains(new AsciiString(mime)))
                context.Response.Headers.Add(HttpHeaderNames.ContentType, mime);
        }

        /// <summary>
        /// 获取Json返回
        /// </summary>
        /// <param name="actionResult"></param>
        /// <returns></returns>
        private void GetJsonResponse(object actionResult, HttpContext context)
        {
            var mime = "application/json;charset=utf-8";
            if (!context.Response.Headers.Contains(new AsciiString(mime)))
                context.Response.Headers.Add(HttpHeaderNames.ContentType, mime);
            var body = JsonConvert.SerializeObject(actionResult);
            byte[] bodyData = string.IsNullOrEmpty(body) ? new byte[0] : Encoding.UTF8.GetBytes(body);
            context.Response.Content = Unpooled.WrappedBuffer(bodyData);
        }

        /// <summary>
        /// 获取文本返回
        /// </summary>
        /// <param name="actionResult"></param>
        /// <returns></returns>
        private void GetTextResponse(object actionResult, HttpContext context)
        {
            var mime = "application/text;charset=utf-8";
            if (!context.Response.Headers.Contains(new AsciiString(mime)))
                context.Response.Headers.Add(HttpHeaderNames.ContentType, mime);
            var body = actionResult.ToString();
            byte[] bodyData = string.IsNullOrEmpty(body) ? new byte[0] : Encoding.UTF8.GetBytes(body);
            context.Response.Content = Unpooled.WrappedBuffer(bodyData);
        }

        /// <summary>
        /// 获得Body参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string GetBodyParams(HttpContext context)
        {
            string result = context.Request.Content.ReadString(context.Request.Content.Capacity, Encoding.UTF8);
            return result;
        }

        /// <summary>
        /// 获取Url参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetUrlParams(HttpContext context)
        {
            var result = new Dictionary<string, string>();
            string[] tempString = context.Request.Uri.Split('?');
            if (tempString.Length <= 1)
                return result;
            string[] paramsString = tempString[1].Split('&');
            foreach (string param in paramsString)
            {
                if (string.IsNullOrEmpty(param))
                    continue;
                string[] values = param.Split('=');
                if (values.Length != 2 || result.ContainsKey(values[0]))
                    continue;
                result.Add(values[0], values[1]);
            }

            return result;
        }
    }
}