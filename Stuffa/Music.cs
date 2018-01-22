using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stuffa
{
    class Music
    {
        public string path;
        public string name;

        public Music()
        {

        }
        public Music(string path, string name)
        {
            this.path = path + name + ".mp3";
            this.name = name;
        }
    }
}

