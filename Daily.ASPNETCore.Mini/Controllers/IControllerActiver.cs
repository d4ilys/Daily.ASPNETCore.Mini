using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.ASPNETCore.Mini.Controllers
{
    public interface IControllerActiver
    {
        void CreateApplicationPartManager(IServiceCollection service);
    }
}
