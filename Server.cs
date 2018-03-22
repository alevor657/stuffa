using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SocketServer
{
    public class Handler : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Recieved: ");
            Console.WriteLine(e.Data.ToString());

        }

        protected override void OnOpen() => Console.WriteLine("opened");

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("connection closed");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("error");
        }
    }

    public class Server
    {
        public static void Init()
        {
            var wssv = new WebSocketServer(1340);
            wssv.AddWebSocketService<Handler>("/remote");
            wssv.Start();
            Console.WriteLine($"Listening on {wssv.Address}:{wssv.Port}");
            foreach (var path in wssv.WebSocketServices.Paths)
                Console.WriteLine("- {0}", path);
        }
    }
}
