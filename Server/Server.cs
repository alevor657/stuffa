using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using Newtonsoft.Json;
using Stuffa;
using WebSocketSharp;
using WebSocketSharp.Server;
using WpfApp2.server;
using WpfApp2.pages;
using System.Windows.Threading;

namespace SocketServer
{
    public class Handler : WebSocketBehavior
    {
        private static Container Cont;

        public static void SetContainer(Container c)
        {
            Cont = c;
        }

        //when message is recieved
        protected override void OnMessage(MessageEventArgs msg)
        {

            Console.WriteLine("-------------msg from server-------------");

            //converts the message from json obj to a ServerMsg class
            ServerMsg parseMsg = JsonConvert.DeserializeObject<ServerMsg>(msg.Data);

            Console.WriteLine(msg.Data.ToString());

            switch (parseMsg.action) {
                case "PLAY":
                    Cont.Dispatcher.Invoke(Cont.PlaySelectedSong);
                    Send("test");
                    break;
                case "PAUSE":
                    //...
                default:
                    return;
            }
        }

        public static string GetIp()
        {
            //get name of host
            string hostName = Dns.GetHostName();
            Console.WriteLine(hostName);

            //get the Ip address
            string localIp = Dns.GetHostByName(hostName).AddressList[0].ToString();
            Console.WriteLine("Ip address is : " + localIp);
            return localIp;
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

        public void test()
        {

        }
    }

    public class Server
    {
        WebSocketServer wssv;

        public Server(Container c)
        {
            wssv = new WebSocketServer(1340);
            Handler.SetContainer(c);
            wssv.AddWebSocketService<Handler>("/remote");
            wssv.KeepClean = false;
            wssv.Start();
            Console.WriteLine($"Listening on {wssv.Address}:{wssv.Port}");
            foreach (var path in wssv.WebSocketServices.Paths)
                Console.WriteLine("- {0}", path);
        }

        public void Stop()
        {
            wssv.Stop();
        }
    }
}
