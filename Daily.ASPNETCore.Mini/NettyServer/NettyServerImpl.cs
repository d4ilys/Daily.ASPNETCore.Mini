using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Daily.ASPNETCore.Mini.Common;
using DotNetty.Codecs.Http;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Materal.DotNetty.Server.CoreImpl;
using Microsoft.Extensions.DependencyInjection;

namespace Daily.ASPNETCore.Mini.NettyServer
{
    public class NettyServerImpl : INettyServer
    {
        public async Task RunServer(IServiceCollection service)
        {
            //创建ServerBootstartp
            var bootstrap = new ServerBootstrap();
            //绑定事件组
            IEventLoopGroup mainGroup = new MultithreadEventLoopGroup(1);
            IEventLoopGroup workGroup = new MultithreadEventLoopGroup();
            bootstrap.Group(mainGroup, workGroup);
            //绑定服务端的通道
            bootstrap.Channel<TcpServerSocketChannel>();
            //配置处理器
            bootstrap.Option(ChannelOption.SoBacklog, 8192);
            bootstrap.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                var channelPipeline = channel.Pipeline;
                channelPipeline.AddLast(new HttpServerCodec());
                channelPipeline.AddLast(new HttpObjectAggregator(65536));
                channelPipeline.AddLast(service.BuildServiceProvider().GetService<ChannelHandler>());
            }));
            //配置主机和端口号
            IPAddress ipAddress = GetTrueIPAddress();
            var port = 9616;
            IChannel bootstrapChannel = await bootstrap.BindAsync(ipAddress, port);
            ConsoleHelper.WriteLine($"Now listening on：http://{ipAddress.ToString()}:{port}..");
            WaitServerStop();
            //第六步：停止服务
            await bootstrapChannel.CloseAsync();
        }

        /// <summary>
        /// 等待服务停止
        /// </summary>
        private void WaitServerStop()
        {
            ConsoleHelper.WriteLine("Application started. Press Ctrl+C to shut down..");
            Console.CancelKeyPress += (sender, e) => { Environment.Exit(0); };
            while (true)
                Thread.Sleep(100);
        }

        /// <summary>
        /// 获得真实IP地址
        /// </summary>
        /// <returns></returns>
        private IPAddress GetTrueIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
            ipAddresses = ipAddresses.Where(m => m.ToString().IsIPv4()).ToArray();
            IPAddress result = ipAddresses.Last();
            return result;
        }
    }
}