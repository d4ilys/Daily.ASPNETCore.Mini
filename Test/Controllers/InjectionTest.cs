using System.Reflection;
using Daily.ASPNETCore.Mini.HttpContexts;
using Daily.ASPNETCore.Mini.MVC.ControllersRule;
using Daily.ASPNETCore.Mini.MVC.HttpAttribute;
using Daily.Service.TestServiceaaa;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace Test.Controllers
{
    [DynamicController]
    public class InjectionTest
    {
        //属性注入
        [Autowired] public IHttpContextAccessor HttpContextAccessor { get; set; }

        //构造函数注入
        public IUserService UserService;

        private readonly ITestService TestService;

        public InjectionTest(IUserService userService, ITestService testService)
        {
            UserService = userService;
            TestService = testService;
        }

        /// <summary>
        /// 测试属性注入
        /// </summary>
        /// <returns></returns>
        public string TestPropertyInject()
        {
            //这里为空
            TestService?.ContructorInjectTest();
            return UserService.Test("测试");
        }

        /// <summary>
        /// 测试构造函数注入
        /// </summary>
        /// <returns></returns>
        public string? TestConstructorInject()
        {
            return HttpContextAccessor.HttpContext.Request?.Uri;
        }

        /// <summary>
        /// 测试HttpContext RequestService
        /// </summary>
        /// <returns></returns>
        public string? TestHttpContextRequestServiceInject()
        {
           var iTestService =  HttpContextAccessor.HttpContext.RequestService.GetService<ITestService>();
           iTestService.PropertInjectTest();
            return "请看控制台";
        }
    }
}