using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Daily.ASPNETCore.Mini.MVC
{
    public interface IControllerActiver
    {
        void CreateApplicationPartManager(IServiceCollection service, Assembly assembly);
    }
}
