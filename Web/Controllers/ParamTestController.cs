using Daily.ASPNETCore.Mini.MVC.HttpAttribute;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class ParamTestController
    {
        /// <summary>
        /// Json请求
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public object TestJsonParam(Dto dto)
        {
            return dto;
        }
        /// <summary>
        /// Json请求-异步方法
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> TestJsonParamAsync(Dto dto)
        {
            return await Task.Run(() => dto);
        }
        /// <summary>
        /// Url请求方式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        [HttpGet]
        public object TestUrlParam(string name, int age)
        {
            return $"{name}:{age}";
        }

        /// <summary>
        /// Url Json请求方式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        [HttpPost]
        public object TestUrlAndJsonParam(string name, int age, Dto dto)
        {
            return $"{JsonConvert.SerializeObject(dto)}:{name}:{age}";
        }
    }

    public class Dto
    {
        public int Id
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
    }
}
