using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdSharp.Tagging.ID3v2;

namespace Stuffa
{
    class Music
    {
        private string path;
        private string name;
        private string filetype;
        private int BPM;
        private string artist;
        private string title;

        public Music()
        {
            this.BPM = -2;
        }
        
        public int getBPM()
        {
            //if BPM is not jet loaded from memory
            if (BPM == -2)
            {
                IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());

                string bpmString = fileInfo.BPM;
                int trueBpm = 0;
                if (bpmString != null)
                {
                    if (bpmString.Length > 3)
                    {
                        int ind = bpmString.LastIndexOf('.');
                        string subString1;
                        string subString2 = "";
                        if (ind > 0)
                        {
                            subString1 = bpmString.Substring(0, ind);
                            trueBpm = Convert.ToInt32(subString1);
                            subString2 = bpmString.Substring(ind + 1);
                            int test = Convert.ToInt32(subString2);
                            if (test > 49)
                            {
                                trueBpm++;
                            }
                        }
                        else
                        {
                            ind = bpmString.LastIndexOf(',');
                            if (ind > 0)
                            {
                                subString1 = bpmString.Substring(0, ind);
                                trueBpm = Convert.ToInt32(subString1);
                                subString2 = bpmString.Substring(ind + 1);
                                int test = Convert.ToInt32(subString2);
                                if (test > 49)
                                {
                                    trueBpm++;
                                }
                            }
                        }
                    }
                    else
                    {
                        trueBpm = Convert.ToInt32(bpmString);
                    }
                    BPM = trueBpm;
                }
                else
                {
                    BPM = 0;
                }
            }
            return this.BPM;
        }

        public string getArtist()
        {
            if (artist == null)
            {
                IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());

                artist = fileInfo.Artist;
                if(artist == "")
                {
                    artist = "unknown";
                }
            }
            return this.artist;
        }
        public string getTitle()
        {
            if(title == null)
            {
                IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());

                this.title = fileInfo.Title;
                Console.WriteLine(title);
                if (title == null)
                {
                    title = name;
                }

            }
            return this.title;
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
            this.BPM = -2;

        }

        public Music(string path, string name, string filetype)
        {
            this.path = path;
            this.name = name;
            this.filetype = filetype;
            this.BPM = -2;
        }
        public override string ToString()
        {
            return getTitle();

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
            if (this.path.EndsWith("\\"))
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

