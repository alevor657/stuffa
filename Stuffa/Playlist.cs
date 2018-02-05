using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    class Playlist
    {
        private string path;
        private string name;
        private string filetype;

        public Playlist()
        {

        }

        public Playlist(string fullPath)
        {
            int pathPos = fullPath.LastIndexOf("\\");
            int fileTypePos = fullPath.LastIndexOf(".");
            if (pathPos > 0 && fileTypePos > 0)
            {
                this.path = fullPath.Substring(0, pathPos);
                this.name = fullPath.Substring(pathPos + 1, fileTypePos - pathPos - 1);
                this.filetype = fullPath.Substring(fileTypePos);

            }
        }

        public override string ToString()
        {
            return this.name;

        }

        public string getFullPath()
        {
            if(path.EndsWith("\\"))
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
