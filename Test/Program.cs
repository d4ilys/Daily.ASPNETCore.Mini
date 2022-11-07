using Daily.ASPNETCore.Mini;
using Daily.ASPNETCore.Mini.MiddleWare.Extension;
using Daily.ASPNETCore.Mini.NettyServer;
using Daily.ASPNETCore.Mini.Services;
using Microsoft.Extensions.DependencyInjection;
using Test;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();

var app = builder.Build();

app.UseStaticFile();

app.UseSwagger();

app.Run();

