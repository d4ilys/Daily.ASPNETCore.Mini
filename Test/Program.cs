using System.Reflection;
using Daily.ASPNETCore.Mini;
using Microsoft.AspNetCore.Builder.Extension;
using Daily.ASPNETCore.Mini.Services;
using Daily.Service.TestServiceaaa;
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

app.Run();