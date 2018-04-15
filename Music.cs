using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdSharp.Tagging.ID3v2;
using System.Windows.Controls;

namespace Stuffa
{
    public class Music : IEquatable<Music>
    {

        //MediaElement player = new MediaElement();
        
        public string Path { get; set; }
        private string name;
        private string filetype;

        //declare get and set methodes so they dont loop
        public int Bpm { get { getBpm();  return this.RealBpm;  } set { this.RealBpm = value; } }
        private int RealBpm;
        public string Artist { get { getArtist(); return RealArtist; } set { this.RealArtist = value; } }
        private string RealArtist;
        public string Title {
            get {
                
                getTitle();
                return this.RealTitle;
            }
            set { this.RealTitle = value; }
        }

        private string RealTitle;

        public Music(string Path = "unknown", string name = "unknown", int Bpm = -2, string Artist = "unknown", string Title = "unknown")
        {
            this.Bpm = Bpm;
            
        }
        
        public int getBpm()
        {
            //if Bpm is not jet loaded from memory
            if (RealBpm == -2)
            {
                getData();
            }
            return this.RealBpm;
        }

        public string getArtist()
        {
            if (RealArtist == null)
            {
                getData();
            }
            return this.RealArtist;
        }
        public string getTitle()
        {
            if(RealTitle == null)
            {
                getData();

            }
            return this.RealTitle;
        }

        private void getData()
        {

            try
            {
                //get ID3 tag
                IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());

                //load '
                this.RealTitle = fileInfo.Title;
                if (RealTitle == null)
                {
                    RealTitle = name;
                }

                //load Artist
                this.RealArtist = fileInfo.Artist;
                if (this.RealArtist == "" || this.RealArtist == null)
                {
                    this.RealArtist = "unknown";
                }

                //load Bpm
                string BpmString = fileInfo.BPM;
                int trueBpm = 0;
                if (BpmString != null)
                {
                    if (BpmString.Length > 3)
                    {
                        int ind = BpmString.LastIndexOf('.');
                        string subString1;
                        string subString2 = "";
                        if (ind > 0)
                        {
                            subString1 = BpmString.Substring(0, ind);
                            trueBpm = Convert.ToInt32(subString1);
                            subString2 = BpmString.Substring(ind + 1);
                            int test = Convert.ToInt32(subString2);
                            if (test > 49)
                            {
                                trueBpm++;
                            }
                        }
                        else
                        {
                            ind = BpmString.LastIndexOf(',');
                            if (ind > 0)
                            {
                                subString1 = BpmString.Substring(0, ind);
                                trueBpm = Convert.ToInt32(subString1);
                                subString2 = BpmString.Substring(ind + 1);
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
                        trueBpm = Convert.ToInt32(BpmString);
                    }
                    RealBpm = trueBpm;
                }
                else
                {
                    RealBpm = 0;
                }
            }
            catch
            {

                this.Artist = "unknown";
                this.Title = "unknown";
                this.Bpm = -1;
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
                this.Artist = n;

            }
            catch
            {
                Console.WriteLine("did not save Artist to ID3 tag\n" + getFullPath() + "\nif file is open, close it and try again");
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
                this.Title = n;

            }
            catch
            {
                Console.WriteLine("did not save Title to ID3 tag\n" + getFullPath()+ "\nif file is open, close it and try again");
                ret = false;

            }
            return ret;

        }


        public bool setBPM(int n)

        {
            bool ret = true;

                try
                {

                    IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());
                    fileInfo.BPM = n.ToString();
                    fileInfo.Save(getFullPath());
                    this.Bpm = n;

                }
                catch
                {
                    Console.WriteLine("did not save BPM to ID3 tag\n" + getFullPath() + "\n* if file is open, close it and try again\n* check that you enterd an integer");
                    ret = false;
                }


            


            
            return ret;


        }


        public Music(string fullPath)
        {
            int PathPos = fullPath.LastIndexOf("\\");
            int fileTypePos = fullPath.LastIndexOf(".");
            if (PathPos > 0 && fileTypePos > 0)
            {
                this.Path = fullPath.Substring(0, PathPos);
                this.name = fullPath.Substring(PathPos + 1, fileTypePos - PathPos -1);
                this.filetype = fullPath.Substring(fileTypePos);

            }
            this.Bpm = -2;

        }

        public Music(string Path, string name, string filetype)
        {
            this.Path = Path;
            this.name = name;
            this.filetype = filetype;
            this.Bpm = -2;
        }

        public Music(string Path, string Title, string Artist, int Bpm)
        {
            this.Path = Path;
            this.Title = Title;
            this.Artist = Artist;
            this.Bpm = Bpm;
        }
        public override string ToString()
        {
            return getTitle();

        }

        public string getPath()
        {
            return Path;
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
            if (Path != null)
            {
                if (this.Path.EndsWith("\\"))
                {
                    return this.Path + this.name + this.filetype;
                }
                else
                {
                    return this.Path + "\\" + this.name + this.filetype;
                }
            }
            return "";
        }

        public void generateTestData(string Path, string name, string filetype, int Bpm, string Artist, string titel)
        {
            this.Path = Path;
            this.name = name;
            this.filetype = filetype;
            this.Bpm = Bpm;

            if(Artist == "")
            {
                Artist = "unknown";
            }
            this.Artist = Artist;

            if(titel == "")
            {
                titel = name;
            }
            this.Title = titel;
        }

        public bool Equals(Music other)
        {
            if (other == null)
                return false;
        
            bool equals = false;
            if (Artist == other.Artist && Title == other.Title)
                equals = true;
            return equals;
        }
    }
}

