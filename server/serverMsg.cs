using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.server
{
    public class ServerMsg
    {

        public string song { get; set; } = "";
        public int BPM { get; set; } = 0;
        public string noPort { get; } = "undf";
        public string port { get; set; } = "undf";


        
    }
}
