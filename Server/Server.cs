using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using Newtonsoft.Json;
using Stuffa;
using WebSocketSharp;
using WebSocketSharp.Server;
using SocketServer;
using WpfApp2.pages;
using System.Windows.Threading;

namespace SocketServer
{
    public class Handler : WebSocketBehavior
    {
        private static Container Container;

        public static void SetContainer(Container c)
        {
            Container = c;
        }

        protected override void OnOpen() {
            Console.WriteLine("opened");
            Container.Dispatcher.Invoke(() => Container.snackBarActivate("Connected!"));
        }

        protected override void OnClose(CloseEventArgs e)
        {

            Console.WriteLine("connection closed");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("error");
        }

        //when message is recieved
        protected override void OnMessage(MessageEventArgs msg)
        {

            Console.WriteLine("-------------msg from server-------------");

            //converts the message from json obj to a ServerMsg class
            ServerMsg parseMsg = JsonConvert.DeserializeObject<ServerMsg>(msg.Data);

            Console.WriteLine(msg.Data.ToString());

            switch (parseMsg.type) {
                case "PLAY":
                    Container.Dispatcher.Invoke(Container.TogglePlay);
                    Send(ServerMsg.Create(Action.PLAY_SUCCESS));
                    break;
                case "NEXT_TRACK":
                    Container.Dispatcher.Invoke(Container.NextSong);
                    string json1 = ParsePlayerState(
                        Container.Dispatcher.Invoke(Container.getPlayerState)
                        );
                    Send(ServerMsg.Create(Action.REQUEST_STATE_SUCCESS, json1));
                    break;
                case "PAUSE":
                    Container.Dispatcher.Invoke(Container.TogglePlay);
                    Send(ServerMsg.Create(Action.PAUSE_SUCCESS));
                    break;
                case "REQUEST_STATE":
                    string json = ParsePlayerState(
                        Container.Dispatcher.Invoke(Container.getPlayerState)
                        );
                    Send(ServerMsg.Create(Action.REQUEST_STATE_SUCCESS, json));
                    break;
                //...
                default:
                    return;
            }
        }

        private string ParsePlayerState(Dictionary<string, object> state)
        {
            return JsonConvert.SerializeObject(state, Formatting.Indented);
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

        public void Send(string msg)
        {
            wssv.WebSocketServices.Broadcast(msg);
        }

        public void Stop()
        {
            wssv.Stop();
        }
    }
}
