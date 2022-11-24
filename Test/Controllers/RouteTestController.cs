using Daily.ASPNETCore.Mini.MVC.ControllersRule;
using Daily.ASPNETCore.Mini.MVC.HttpAttribute;

namespace Test.Controllers
{
    [DynamicController]
    [Route("RouteTest1")]
    public class RouteTestController
    {
        [HttpGet]
        [Route("Test")]
        public string Test()
        {
            return "路由匹配成功";
        }
    }
}
