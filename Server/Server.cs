using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using Newtonsoft.Json;
using Stuffa;
using WebSocketSharp;
using WebSocketSharp.Server;
using WpfApp2.server;

namespace SocketServer
{
    public class Handler : WebSocketBehavior
    {
        string port = "";
        //when message is resived
        protected override void OnMessage(MessageEventArgs msg)
        {

            Console.WriteLine("-------------msg from server-------------");

            //converts the message from json obj to a ServerMsg class
            ServerMsg parseMsg = JsonConvert.DeserializeObject<ServerMsg>(msg.Data.ToString());

            Console.WriteLine(msg.Data.ToString());
            Console.WriteLine("BPM: " + parseMsg.BPM.ToString());
            Console.WriteLine("song: " + parseMsg.song);
            Console.WriteLine("port: " + parseMsg.port);

            if (parseMsg.port != port && parseMsg.port != parseMsg.noPort)
            {
                //check if message is from the same network
                //if (parseMsg.port.StartsWith(GetIp()) || parseMsg.port.StartsWith("127.0.0.1")){
                port = parseMsg.port;

                //}
            }


            //send information to controller class withs then informs server what to send back
            sendMessage(parseMsg.song);


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

        //send back or send just send a message to the mobile
        public void sendMessage(string msg)
        {
            Send(msg);
        }
        public void test()
        {

        }
    }

    public class Server
    {
        public static void Init()
        {
            var wssv = new WebSocketServer(1340);
            wssv.AddWebSocketService<Handler>("/remote");
            wssv.KeepClean = false;
            wssv.Start();
            Console.WriteLine($"Listening on {wssv.Address}:{wssv.Port}");
            foreach (var path in wssv.WebSocketServices.Paths)
                Console.WriteLine("- {0}", path);
        }
    }
}
