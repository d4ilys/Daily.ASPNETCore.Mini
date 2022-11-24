using System.Reflection;
using Daily.ASPNETCore.Mini;
using Microsoft.AspNetCore.Builder.Extension;
using Daily.ASPNETCore.Mini.Services;
using Daily.Service.TestServiceaaa;
using Microsoft.Extensions.Configuration;
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

WebApplication app = builder.Build();
#region 中间件测试

//最简单的中间件
app.Use((context, next) =>
{
    Console.WriteLine("我是一个中间件开始执行");
    //执行下一个中间件
    next();
    Console.WriteLine("我是一个中间件完成执行");
});

app.Use((context, next) =>
{
    Console.WriteLine("我是二个中间件开始执行");
    //执行下一个中间件
    next();
    Console.WriteLine("我是二个中间件完成执行");
});

app.Use((context, next) =>
{
    Console.WriteLine("我是三个中间件开始执行");
    //执行下一个中间件
    next();
    Console.WriteLine("我是三个中间件完成执行");
});

#endregion

app.UseStaticFile();


app.Run();