using Newtonsoft.Json;

namespace SocketServer
{
    public class ServerMsg
    {
        public string type { get; set; } = "";
        public string payload { get; set; } = "";

        public static string Create(string m, string payload = null)
        {
            ServerMsg s = new ServerMsg();
            s.type = m;
            s.payload = payload;

            return JsonConvert.SerializeObject(s);
        }
    }
}