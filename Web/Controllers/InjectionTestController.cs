using Daily.ASPNETCore.Mini.HttpContexts;
using Daily.ASPNETCore.Mini.MVC.ControllersRule;
using Daily.ASPNETCore.Mini.MVC.HttpAttribute;
using Services;
using Web.TestServiceaaa;

namespace Web.Controllers
{
    [DynamicController]
    public class InjectionTestController
    {
        //属性注入
        [Autowired]
        public IHttpContextAccessor HttpContextAccessor
        {
            get; set;
        }

        //构造函数注入
        public IUserService UserService;

        public ITestService TestService;

        public InjectionTestController(IUserService userService, ITestService testService)
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
    }
}