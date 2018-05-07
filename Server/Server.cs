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
            Container.Dispatcher.Invoke(() => Container.snackBarActivate("Mobile phone connected!"));
        }

        protected override void OnClose(CloseEventArgs e)
        {

            Console.WriteLine("connection closed");
            Container.Dispatcher.Invoke(() => Container.snackBarActivate("Mobile phone disconnected!"));
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
                case "PAUSE":
                    Container.Dispatcher.Invoke(Container.TogglePlay);
                    Send(ServerMsg.Create(Action.PAUSE_SUCCESS));
                    break;
                case "NEXT_TRACK":
                    Container.Dispatcher.Invoke(Container.NextSong);
                    SyncState();
                    break;
                case "REPLAY":
                    Container.Dispatcher.Invoke(Container.Replay);
                    break;
                case "REQUEST_STATE":
                    SyncState();
                    break;
                // ??
                case "VOLUME_CHANGE":
                    Container.Dispatcher.Invoke(Container.GetCurrentVolumeAsInt);
                    Send(ServerMsg.Create(Action.VOLUME_CHANGE));
                    break;
                case "SET_SOUND":
                    float val = (Int32.Parse(parseMsg.payload))/100f;
                    Container.Dispatcher.Invoke(new System.Action(() => Container.SetVolume(val)));
                    Container.Dispatcher.Invoke(Container.SendStateToServerOnUpdate);
                    break;
                case "SET_BPM":
                    Container.Dispatcher.Invoke(() => Container.SetBaseBpm(int.Parse(parseMsg.payload)));
                    break;
                case "SET_BPM_INTERVAL":
                    Container.Dispatcher.Invoke(() => Container.SetInterval(
                            int.Parse(parseMsg.payload)
                        ));
                    break;
                case "BPM_AUTOPLAY_TOGGLE":
                    Container.Dispatcher.Invoke(Container.ToggleAutoplay);
                    break;
                case "SET_BPM_STEP":
                    int nr;
                    Int32.TryParse(parseMsg.payload, out nr);
                    Container.Dispatcher.Invoke(new System.Action(() => Container.ChangeJump(nr)));
                    Container.Dispatcher.Invoke(Container.SendStateToServerOnUpdate);
                    break;
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

        private string ParsePlayerState(Dictionary<string, object> state)
        {
            return JsonConvert.SerializeObject(state, Formatting.Indented);
        }

        public void SyncState()
        {
            string json = ParsePlayerState(
                Container.Dispatcher.Invoke(Container.getPlayerState)
            );
            Console.WriteLine(json);
            Send(ServerMsg.Create(Action.UPDATE, json));
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
            Console.WriteLine($"Sending: {msg}");
            wssv.WebSocketServices.Broadcast(msg);
        }

        public void Stop()
        {
            wssv.Stop();
        }
    }
}
