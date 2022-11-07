using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Common;
using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.Controllers.Attributes;
using Daily.ASPNETCore.Mini.Controllers.HttpAttribute;
using Daily.ASPNETCore.Mini.Host;
using Daily.ASPNETCore.Mini.Models;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;
using static DotNetty.Codecs.Http.HttpContentEncoder;
using HttpMethod = System.Net.Http.HttpMethod;

namespace Daily.ASPNETCore.Mini.Controllers
{
    public class ControllersBus : IControllersBus
    {
        public void CreateApplicationPartManager(IServiceCollection service)
        {
            //获取所有dll
            var allDll = Directory.GetFiles(AppContext.BaseDirectory, "*.dll");
            var applicationPartManager = new ApplicationPartManager();
            foreach (var path in allDll)
            {
                var assembly = Assembly.LoadFile(path);
                foreach (var type in assembly.GetTypes())
                {
                    //判断是不是控制器
                    var controllerName = GetControllerName(type);
                    if (!string.IsNullOrEmpty(controllerName))
                    {
                        applicationPartManager.ApplicationParts.Add(new ApplicationPart()
                        {
                            Name = controllerName,
                            Type = type
                        });
                    }
                }
            }

            //添加所有的控制器信息到
            service.AddSingleton(applicationPartManager);

            //查看控制器是不是标记Route特性
            string GetRouteName(Type type)
            {
                var routeAttribute = type.GetCustomAttribute<RouteAttribute>();
                var routeAttributeKey = routeAttribute?.Key;
                string result = null;
                if (!string.IsNullOrEmpty(routeAttributeKey))
                {
                    result = routeAttributeKey;
                }

                return result;
            }

            //通过控制器规则获取控制器名称
            string GetControllerName(Type type)
            {
                //如果是接口、抽象类、特性
                if (type.IsInterface || type.IsAbstract || type.IsSubclassOf(typeof(Attribute)))
                {
                    return string.Empty;
                }

                //类名以Controller结尾
                if (type.Name.EndsWith("Controller"))
                {
                    return GetRouteName(type) ?? type.Name.Substring(0, type.Name.Length - 10);
                }

                //是否标记了IDynamicController特性
                if (Attribute.IsDefined(type, typeof(DynamicControllerAttribute)))
                {
                    return GetRouteName(type) ?? type.Name;
                }

                //是否继承了IDynamicController接口
                if (type.IsSubclassOf(typeof(IDynamicController)))
                {
                    return GetRouteName(type) ?? type.Name;
                }

                return string.Empty;
            }
        }

        //执行MVC
        public async Task ControllerExecutor(IServiceProvider service, HttpContext context)
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
                var type = applicationParts.FirstOrDefault().Type;
                //创建控制器的实例
                var controller = Activator.CreateInstance(type);

                //处理路由标记特性
                var routeActionMethed = type.GetMethods()
                    .Where(m => m.GetCustomAttribute<RouteAttribute>()?.Key == actionName).FirstOrDefault();
                if (routeActionMethed != null)
                {
                    actionName = routeActionMethed.Name;
                }

                var methodInfo = type.GetMethod(actionName);
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

                //处理返回值类型
                if (actionResult is Task task)
                {
                    dynamic taskObj = task;
                    actionResult = await taskObj;
                }

                if (actionResult != null)
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
            }
            else
            {
                context.Response.Status = HttpResponseStatus.BadRequest;
                return;
            }
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
                if (urls.Length < 4)
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
            context.Response.Headers.Add(HttpHeaderNames.ContentType, "application/octet-stream");
        }

        /// <summary>
        /// 获取Json返回
        /// </summary>
        /// <param name="actionResult"></param>
        /// <returns></returns>
        private void GetJsonResponse(object actionResult, HttpContext context)
        {
            context.Response.Headers.Add(HttpHeaderNames.ContentType, "application/json;charset=utf-8");
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
            context.Response.Headers.Add(HttpHeaderNames.ContentType, "application/json;charset=utf-8");
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