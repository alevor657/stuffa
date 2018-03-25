using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdSharp.Tagging.ID3v2;
using TagLib;
using System.Windows.Controls;

namespace Stuffa
{
    public class Music : IEquatable<Music>
    {

        MediaElement player = new MediaElement();
        
        public string Path { get; set; }
        private string name;
        private string filetype;
        public int Bpm { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }

        public Music(string Path = "unknown", string name = "unknown", int Bpm = -2, string Artist = "unknown", string Title = "unknown")
        {
            this.Bpm = Bpm;
            
        }
        
        public int getBpm()
        {
            //if Bpm is not jet loaded from memory
            if (Bpm == -2)
            {
                getData();
            }
            return this.Bpm;
        }

        public string getArtist()
        {
            if (Artist == null)
            {
                getData();
            }
            return this.Artist;
        }
        public string getTitle()
        {
            if(Title == null)
            {
                getData();

            }
            return this.Title;
        }

        private void getData()
        {

            try
            {
                //get ID3 tag
                IID3v2Tag fileInfo = new ID3v2Tag(getFullPath());

                //load '
                this.Title = fileInfo.Title;
                if (Title == null)
                {
                    Title = name;
                }

                //load Artist
                this.Artist = fileInfo.Artist;
                if (this.Artist == "" || this.Artist == null)
                {
                    this.Artist = "unknown";
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
                    Bpm = trueBpm;
                }
                else
                {
                    Bpm = 0;
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
                    this.Bpm = res;

                }
                catch
                {
                    Console.WriteLine("did not save BPM to ID3 tag\n" + getFullPath() + "\n* if file is open, close it and try again\n* check that you enterd an integer");
                    ret = false;
                }


            }
            else
            {
                Console.WriteLine("did not save Bpm to ID3 tag\n" + getFullPath());
                Console.WriteLine("please insert a Integer");
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
            this.getData();

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
            if (this.Path.EndsWith("\\"))
            {
                return this.Path + this.name + this.filetype;
            }
            else
            {
                return this.Path + "\\" + this.name + this.filetype;
            }
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

