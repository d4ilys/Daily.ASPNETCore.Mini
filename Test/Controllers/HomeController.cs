using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.Controllers.HttpAttribute;
using Daily.Service.TestService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Controllers
{
    [Route("Home")]
    public class HomeController
    {
        [Autowired] public ITestService _testService { get; set; }

        [Autowired]
        public IConfiguration Configuration { get; set; }

        public IHttpContextAccessor _contextAccessor;

        [Autowired] public IServiceProvider _serviceProvider { get; set; }

        public HomeController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public object Tests()
        {
            Console.WriteLine(Configuration["Daily"]);
            var contextAccessorHttpContext = _contextAccessor.HttpContext.Request.Uri;
            return contextAccessorHttpContext;
        }

        [HttpPost]
        public object Test(PersonDto dto)
        {
            return dto;
        }

        [HttpGet]
        [Route("Test2Update")]
        public object Test2(string age)
        {
            return age;
        }

        public object Test3(string age, PersonDto dto)
        {
            return "Hello2";
        }

        public async Task<object> TestTask()
        {
            return await Task.Run(() => new
            {
                name = "异步测试",
                success = true
            });
        }
    }

    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}