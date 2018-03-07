using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SocketServer
{
    public class Handler : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            // e.Data
            /*
                {
                    payload: 'bpm',
                    action: '140'
                }

                OR

                {
                    payload: 'changeSong'
                    action: -1 // Prev song
                }

                ...
             */
            // Parse e.Data from json

            switch (e.Data.payload)
            {
                case "change song":
                    ChangeSong();
                    break;
                case ...

            }
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

    public class StuffaRemoteSocket
    {
        /*
            Start server. Port can be specified (optional)
         */
        public static void Start(int port = 1340)
        {
            var wssv = new WebSocketServer(port);
            wssv.AddWebSocketService<Handler>("/remote");
            wssv.Start();
            Console.WriteLine($"Listening on {wssv.Address}:{wssv.Port}");
            foreach (var path in wssv.WebSocketServices.Paths)
                Console.WriteLine("- {0}", path);

            // wssv.Stop();
        }
    }
}
