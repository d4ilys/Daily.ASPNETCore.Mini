using Daily.ASPNETCore.Mini.Common;
using DotNetty.Codecs.Http;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Materal.DotNetty.Server.CoreImpl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Daily.ASPNETCore.Mini.NettyServer
{
    public class NettyServerImpl : INettyServer
    {
        static ManualResetEvent _shutdown = new ManualResetEvent(false);
        public async Task RunServer(IServiceProvider serviceProvider)
        {
            //创建ServerBootstartp
            var bootstrap = new ServerBootstrap();
            //绑定事件组
            IEventLoopGroup mainGroup = new MultithreadEventLoopGroup(3);
            IEventLoopGroup workGroup = new MultithreadEventLoopGroup();
            bootstrap.Group(mainGroup, workGroup);
            //绑定服务端的通道
            bootstrap.Channel<TcpServerSocketChannel>();
            //配置处理器
            bootstrap.Option(ChannelOption.SoBacklog, 8192);
            //配置Handler
            bootstrap.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                var channelPipeline = channel.Pipeline;
                channelPipeline.AddLast(new HttpServerCodec());
                channelPipeline.AddLast(new HttpObjectAggregator(65536));
                channelPipeline.AddLast(serviceProvider.GetService<ChannelHandler>());
            }));
            //配置主机和端口号
            var configuration = serviceProvider.GetService<IConfiguration>();
            //默认端口
            var defaultPort = "5020";
            //读取配置文件
            var portConfig = configuration["HostConfig:Port"];
            var ipConfig = configuration["HostConfig:Host"];
            var port = string.IsNullOrWhiteSpace(portConfig) ? defaultPort : portConfig;
            IPAddress ipAddress = string.IsNullOrWhiteSpace(ipConfig) ? GetTrueIPAddress() : IPAddress.Parse(ipConfig);
            IChannel bootstrapChannel = await bootstrap.BindAsync(ipAddress, Convert.ToInt32(port));
            ConsoleHelper.WriteLine($"Now listening on：http://{ipAddress.ToString()}:{port}..");
            //阻塞控制台退出
            await WaitServerStopAsync();
            //第六步：停止服务
            await bootstrapChannel.CloseAsync();
        }

        /// <summary>
        /// 等待服务停止
        /// </summary>
        private async Task WaitServerStopAsync()
        {
            ConsoleHelper.WriteLine("Application started. Press Ctrl+C to shut down..");
            Console.CancelKeyPress += (sender, e) => { Environment.Exit(0); };
            _shutdown.WaitOne();
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