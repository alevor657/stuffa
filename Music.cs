using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdSharp.Tagging.ID3v2;
using TagLib;

namespace Stuffa
{
    public class Music
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
                getData();
            }
            return this.BPM;
        }

        public string getArtist()
        {
            if (artist == null)
            {
                getData();
            }
            return this.artist;
        }
        public string getTitle()
        {
            if(title == null)
            {
                getData();

            }
            return this.title;
        }

        private void getData()
        {

            try
            {
                //get ID3 tag
                IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());

                //load Title
                this.title = fileInfo.Title;
                if (title == null)
                {
                    title = name;
                }

                //load artist
                this.artist = fileInfo.Artist;
                if (this.artist == "")
                {
                    this.artist = "unknown";
                }

                //load BPM
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
            catch
            {
                this.artist = "unknown";
                this.title = "unknown";
                this.BPM = -1;
                Console.WriteLine("/!\\ unable to find ID3 tag for Music with filename " + getName() + getFiletype() + "\ncatch reached in function getData() in Music.cs");
            }

        }

        public bool setArtist(string n)
        {
            bool ret = true;
            try
            {
                IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());
                fileInfo.Artist = n;
                fileInfo.Save(getFullPath());
                this.artist = n;

            }
            catch
            {
                Console.WriteLine("did not save artist to ID3 tag\n" + getFullPath() + "\nif file is open, close it and try again");
                ret = false;
            }
            return ret;
        }

        public bool setTitle(string n)
        {
            bool ret = true;

            try
            {
                IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());
                fileInfo.Title = n;
                fileInfo.Save(getFullPath());
                this.title = n;

            }
            catch
            {
                Console.WriteLine("did not save Title to ID3 tag\n" + getFullPath()+ "\nif file is open, close it and try again");
                ret = false;

            }
            return ret;

        }

        //chanded
        public bool setBPM(string n)
        {
            bool ret = true;
            int res;
            if (int.TryParse(n, out res))
            {

                try
                {

                    IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());
                    fileInfo.BPM = n;
                    fileInfo.Save(getFullPath());
                    this.BPM = res;

                }
                catch
                {
                    Console.WriteLine("did not save BPM to ID3 tag\n" + getFullPath() + "\n* if file is open, close it and try again\n* check that you enterd an integer");
                    ret = false;
                }

            }
            else
            {
                Console.WriteLine("please insert a Integer");
                ret = false;
            }

            
            return ret;


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

        public void generateTestData(string path, string name, string filetype, int BPM, string artist, string titel)
        {
            this.path = path;
            this.name = name;
            this.filetype = filetype;
            this.BPM = BPM;
            this.artist = artist;
            this.title = titel;
        }
       
    }
}

