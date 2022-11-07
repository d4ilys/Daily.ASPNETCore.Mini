using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daily.ASPNETCore.Mini.Context;
using Daily.ASPNETCore.Mini.Controllers;
using Daily.ASPNETCore.Mini.MiddleWare;
using Daily.ASPNETCore.Mini.Host;
using Daily.ASPNETCore.Mini.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Daily.ASPNETCore.Mini
{
    public class WebApplication : IApplicationBuilder
    {
        public static WebApplicationBuilder CreateBuilder(Action<IConfigurationBuilder>? configBuilder = null) =>
            new(configBuilder);

        private IApplicationBuilder? _applicationBuilder { get; set; } = null;

        private IHost? _host { get; set; } = null;

        public WebApplication(IHost host)
        {
            if (host != null)
            {
                _host = host;
                _applicationBuilder = _host.ServicesProvider.GetService<IApplicationBuilder>();
            }
        }

        /// <summary>
        /// Runs an application and block the calling thread until host shutdown.
        /// </summary>
        /// <param name="url">The URL to listen to if the server hasn't been configured directly.</param>
        public Task Run(string? url = null)
        { 
         
            var requestDelegate =  _applicationBuilder?.UseEndpoint(context =>
            {
                //MVC逻辑在这里
                var controllersBus = _host.ServicesProvider.GetService<IControllersBus>();
                controllersBus.ControllerExecutor(_host.ServicesProvider,context);
            }).Build();

            _host.Services.AddSingleton<RequestDelegate>(requestDelegate);

            _host?.StartAsync();

            return Task.CompletedTask;
        }

        public IApplicationBuilder Use(Action<HttpContext, Action> middleware) =>
            _applicationBuilder?.Use(middleware);
    }
}