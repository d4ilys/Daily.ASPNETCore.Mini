using Daily.ASPNETCore.Mini;
using Daily.ASPNETCore.Mini.MiddleWare.Extension;
using Daily.ASPNETCore.Mini.Services;
using Daily.Service.TestService;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Test.Fliter;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers(options =>
{
    options.Fliters.Add(typeof(CustomActionFliter));
    options.Fliters.Add(typeof(CustomExceptionFliter));
});

builder.Services.AddTransient<ITestService, TestServiceImpl>();

builder.Services.AddTransient<IUserService, UserServiceImpl>();

var app = builder.Build();

app.UseStaticFile();

app.UseSwagger();

app.Run();

