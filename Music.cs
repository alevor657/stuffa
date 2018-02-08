﻿using System;
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

        public Music()
        {

        }
        
        public int getBPM()
        {
            return this.BPM;
        }

        public string getArtist()
        {
            return this.artist;
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
            
            try //TODO: if BPM/artist not exist put in 0
            {
                this.loadBPM();
                this.loadArtist();
            }
            catch { }
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

        public void loadArtist()
        {
            TagLib.File tagFile = TagLib.File.Create(getFullPath());
            this.artist = tagFile.Tag.FirstAlbumArtist;
        }

        public void loadBPM()
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
            /*
            dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));

            // get the folder and the child
            var folder = shell.NameSpace(System.IO.Path.GetDirectoryName(getFullPath()));
            var item = folder.ParseName(System.IO.Path.GetFileName(getFullPath()));

            // get the item's property by it's canonical name. doc says it's a string
            string stringBPM = item.ExtendedProperty("System.Music.BeatsPerMinute");

            int comma = stringBPM.LastIndexOf('.');
            if(comma < 0)
            {
                comma = stringBPM.LastIndexOf(',');
            }

            int n;
            bool isNumeric = int.TryParse(stringBPM.Substring(0, comma), out n);
            if (isNumeric)
            {
                BPM = n;
            }
            else
            {
                BPM = -1;
            }*/
        }
    }
}
