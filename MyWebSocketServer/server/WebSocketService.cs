using SuperSocket.SocketBase.Config;
using SuperWebSocket;
using System;
using System.Net;
using MyWebSocketServer.Sys;
using MyWebSocketServer.JSON;
using System.Threading;
using System.Windows.Forms;
using MyWebSocketServer;

namespace MyWebSocketServer.Server
{
    public class WebSocketService: IDisposable 
    {
        public Form1.UpdatePrinting updateTxtDelegate;

        private WebSocketServer server;
        private JSONSerializer jsonSerializer;

        public WebSocketService(int port,Form1 form)
        {
            Console.WriteLine("websocketserver服务器启动..." + Thread.CurrentThread.GetHashCode());
            server = new WebSocketServer();
            Console.WriteLine("WebSocketService..." + Thread.CurrentThread.GetHashCode());
            jsonSerializer = new JSONSerializer();
            server.NewMessageReceived += (sock, message) =>
            {
                try
                {
                    // Dictionary < String,String >  msgJson = jsonSerializer.Deserialize<Dictionary<String,String>>(message);
                    

                    Console.WriteLine("收到新的消息..." + message);
                    form.Invoke(form.UpdatePrintingDelegate, message);

                    Console.WriteLine("服务线程..." + Thread.CurrentThread.GetHashCode());
                    sock.Send("hello"+Thread.CurrentThread.GetHashCode());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    sock.Close();
                }
            };

            server.SessionClosed += (sock, message) =>
            {
                Console.WriteLine("session连接断开...");

            };


            server.NewSessionConnected += (sock) =>
            {
                Console.WriteLine("打开新的连接...");
            };

            if (!server.Setup(new ServerConfig { Port = 8881, MaxConnectionNumber = 10 }) || !server.Start())
            {
                WebSocketException.ThrowServerError("启动失败...");
            }
        }

        public void Dispose()
        {
            if (server != null)
            {
                server.Dispose();
                server = null;
            }
        }

        public void closeServer() {
            //关闭服务器
            if (server != null)
            {
                server.Dispose();
                server = null;
            }
        }
     
    }
}
