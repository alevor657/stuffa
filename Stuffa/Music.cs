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
        public string filetype;

        public Music()
        {

        }
        
        public Music(string fullPath)
        {
            int pathPos = fullPath.LastIndexOf("\\");
            int fileTypePos = fullPath.LastIndexOf(".");
            if (pathPos > 0 && fileTypePos > 0)
            {
                this.path = fullPath.Substring(0, pathPos);
                this.name = fullPath.Substring(pathPos + 1, fileTypePos - pathPos -1);
                this.filetype = fullPath.Substring(fileTypePos);

            }
        }

        public Music(string path, string name, string filetype)
        {
            this.path = path;
            this.name = name;
            this.filetype = filetype;
        }
        public override string ToString()
        {
            return this.name;

        }

        public string getPath()
        {
            return path;
        }

        public string getName()
        {
            return this.name;
        }

        public string getFiletype()
        {
            return this.filetype;
        }

        public string getFullPath()
        {
            if(this.path.EndsWith("\\"))
            {
                return this.path + this.name + this.filetype;
            }
            else
            {
                return this.path + "\\" + this.name + this.filetype;
            }
        }
    }
}

