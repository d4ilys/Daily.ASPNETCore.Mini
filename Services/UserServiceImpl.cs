using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserServiceImpl: IUserService
    {
        public string Test(string type)
        {
            return $"{type}-依赖注入测试成功,";
        }
    }
}
