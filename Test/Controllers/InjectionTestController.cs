using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.MVC.ControllersRule;
using Daily.ASPNETCore.Mini.MVC.HttpAttribute;
using Daily.Service.TestService;
using Services;

namespace Test.Controllers
{
    [DynamicController]
    public class InjectionTestController
    {
        //属性注入
        [Autowired] public IHttpContextAccessor HttpContextAccessor { get; set; }

        //构造函数注入
        public IUserService _userService;

        public ITestService _testService;

        public InjectionTestController(IUserService userService, ITestService testService)
        {
            _userService = userService;
            _testService = testService;
        }

        /// <summary>
        /// 测试属性注入
        /// </summary>
        /// <returns></returns>
        public string TestPropertyInject()
        {
            return _userService.Test("测试");
        }

        /// <summary>
        /// 测试构造函数注入
        /// </summary>
        /// <returns></returns>
        public string TestConstructorInject()
        {
            return HttpContextAccessor.HttpContext.Request.Uri;
        }
    }
}