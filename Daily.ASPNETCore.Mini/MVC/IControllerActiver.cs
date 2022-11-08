using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.MVC
{
    public interface IControllerActiver
    {
        void CreateApplicationPartManager(IServiceCollection service);
    }
}
