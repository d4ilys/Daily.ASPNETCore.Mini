using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.MVC.ControllersRule;

namespace Web.Controllers
{

    [DynamicController]
    public class HomeController
    {
        public string Hello()
        {
            return "Hello Mini ASP.NET Core ";
        }
    }
}
