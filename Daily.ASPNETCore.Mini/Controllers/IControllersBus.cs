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
    public interface IControllersBus
    {
        void CreateApplicationPartManager(IServiceCollection service);
        public Task ControllerExecutor(IServiceProvider service, HttpContext context);
    }
}
