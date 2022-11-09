using Daily.ASPNETCore.Mini.MVC.ControllersRule;
using Daily.ASPNETCore.Mini.MVC.HttpAttribute;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Daily.ASPNETCore.Mini.MVC.Models;
using System.IO;

namespace Daily.ASPNETCore.Mini.MVC
{
    public class ControllerActiver : IControllerActiver
    {
        public void CreateApplicationPartManager(IServiceCollection service, Assembly? assemblyOther = null)
        {
            //获取所有dll
            var allDll = Directory.GetFiles(AppContext.BaseDirectory, "*.dll");
            var applicationPartManager = new ApplicationPartManager();
            foreach (var path in allDll)
            {
                var dllName = Path.GetFileNameWithoutExtension(path);
                var assembly = Assembly.Load(dllName);
                //错误的方式
                //var assembly = Assembly.LoadFile(path);
                if (assemblyOther?.FullName == assembly.FullName)
                    assembly = assemblyOther;

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
    }
}