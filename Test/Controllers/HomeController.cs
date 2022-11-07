using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.Controllers.HttpAttribute;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Test.Controllers
{
    [Route("HomeUpdate")]
    public class HomeController
    {

        public object Tests()
        {
            return "1";
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