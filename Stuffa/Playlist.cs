using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Stuffa;

namespace WpfApp2
{
    class Playlist
    {
        private List<Tuple<int, int>> BPM;
        private List<Tuple<string, int>> artists;
        private string path;
        private string name;
        private string filetype;
        private List<Stuffa.Music> music;

        

        private void sortBPM()
        {
            //sorting with an avrige of O(n log(n))?    (list.Sort is n logn)
            //sorting on BPM
            BPM = BPM.OrderBy(e => e.Item1).ToList();
        }

        private void sortArtists()
        {
            //sorting with an avrige of O(n log(n))?    (list.Sort is n logn)
            //sorting on BPM
            artists = artists.OrderBy(e => e.Item1).ToList();
        }

        //whnen one instance of the BPM is found, find all the other ones near it matching the value on position pos
        private List<int> getBPMpos(int pos)
        {
            List<int> ret = new List<int>();
            int BPMserach = BPM[pos].Item1;
            ret.Add(BPM[pos].Item2);


            for (int i = pos - 1; i > 0; i--)
            {
                if (BPM[i].Item1 == BPMserach)
                {
                    //put the index in ret
                    ret.Add(BPM[i].Item2);
                }
                else
                {
                    //end loop
                    i = 0;
                }

            }
            ret.Reverse();

            for (int i = pos + 1; i < BPM.Count; i++)
            {
                if (BPM[i].Item1 == BPMserach)
                {
                    //put the index in ret
                    ret.Add(BPM[i].Item2);
                }
                else
                {
                    //end loop
                    i = BPM.Count;
                }

            }

            return ret;


        }

        private List<int> getBPMpos(int BPMsearch, int startPos, int endPos)
        {if (startPos < endPos)
            {
                int startVal = this.BPM[startPos].Item1;
                int endVal = this.BPM[endPos].Item1;


                if (startVal == BPMsearch)
                {
                    return getBPMpos(startPos);
                }
                else if (endVal == BPMsearch)
                {
                    return getBPMpos(endPos);
                }
                else
                {
                    int middlePos = (startPos + endPos) / 2;
                    int middleVal = this.BPM[middlePos].Item1;
                    if (middleVal == BPMsearch)
                    {
                        return getBPMpos(middlePos);

                    }
                    else if (middleVal > BPMsearch)
                    {
                        return this.getBPMpos(BPMsearch, startPos + 1, middlePos - 1);


                    }
                    else
                    {
                        return this.getBPMpos(BPMsearch, middlePos + 1, endPos - 1);

                    }

                }
            }
            else
            {
                List<int> ret = new List<int>();

                if (startPos == endPos)
                {
                    if(BPM[endPos].Item1 == BPMsearch)
                    {
                        ret.Add(BPM[endPos].Item2);
                    }
                }
                return ret;
            }

        }

        public List<Music> searchBPM(int nr)
        {
            List<Music> ret = new List<Music>();
            List<int> indexes = getBPMpos(nr, 0, BPM.Count-1);
            foreach (int i in indexes)
            {
                ret.Add(music[i]);
            }
            return ret;
        }


        public Playlist()
        {
            this.music = new List<Stuffa.Music>();
            this.BPM = new List<Tuple<int, int>>();
            this.artists = new List<Tuple<string, int>>();
        }

        public Playlist(string fullPath)
        {
            this.music = new List<Stuffa.Music>();
            this.BPM = new List<Tuple<int, int>>();
            this.artists = new List<Tuple<string, int>>();

            int pathPos = fullPath.LastIndexOf("\\");
            int fileTypePos = fullPath.LastIndexOf(".");
            if (pathPos > 0 && fileTypePos > 0)
            {
                this.path = fullPath.Substring(0, pathPos);
                this.name = fullPath.Substring(pathPos + 1, fileTypePos - pathPos - 1);
                this.filetype = fullPath.Substring(fileTypePos);

            }
        }

        public bool savePlaylist()
        {
            bool retVal = false;

            try
            {
                List<string> musicTracks = new List<string>();
                foreach (Music i in music)
                {
                    musicTracks.Add(i.getFullPath());
                }
                // package Newtonsoft.Json (Json.Net) need to be installed. 
                // to install go to "project" > "manage NuGet packages..." > "Brows" > type "Newtonsoft.Json" / "Json.Net" > "install"
                string json = JsonConvert.SerializeObject(musicTracks.ToArray());
                System.IO.File.WriteAllText(this.getFullPath(), json);
                retVal = true;
            }catch { }
            return retVal;
        }

        public bool loadNewMusic()
        {
            bool retVal = false;
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".mp3";
                dlg.Filter = "MP3 Files (*.mp3)|*.mp3|M4A Files (*.m4a)|*.m4a|FLAC Files (*.flac)|*.flac";
                dlg.Multiselect = true;

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();


                // Get the selected file name and display in a TextBox 
                if (result == true)
                {

                    // Open document 
                    string[] musicPaths = dlg.FileNames;

                    //check if music allready added
                    foreach (string musicPath in musicPaths)
                    {
                        bool add = true;
                        foreach (Music i in music)
                        {
                            if (i.getFullPath() == musicPath)
                            {
                                add = false;
                            }
                        }
                        if (add)
                        {
                            music.Add(new Music(musicPath));
                        }
                    }

                    //add to array


                    //save to file
                    savePlaylist();
                    retVal = true;

                }

            }
            catch
            {

            }
            return retVal;
        }

        public Music getMusic(int index)
        {
            Music ret = null;
            try
            {
                ret = this.music[index];
            }
            catch { }
            return ret;
        }

        public void loadMusic()
        {
            try
            {
                using (StreamReader r = new StreamReader(@getFullPath()))
                {
                    //get JSONm object as string
                    string json = r.ReadToEnd();
                    //convert JSON string to a List array of strings
                    List<string> musicTracks = JsonConvert.DeserializeObject<List<string>>(json);

                    //remove any old data
                    music.Clear();

                    //for every music. add to music
                    foreach (string i in musicTracks.ToArray())
                    {
                        Music newMusic = new Music(i);
                        music.Add(newMusic);

                    }
                }
            }
            catch
            {
                Console.WriteLine("could not acces text files on " + path);
            }
        }

        public List<Music> getMusic()
        {
            return this.music;
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


        public void loadBPM()
        {
            BPM.Clear();
            int index = 0;
            foreach (Music i in music)
            {
                BPM.Add(Tuple.Create<int, int>(i.getBPM(), index));
                index++;
            }
            sortBPM();
        }

        public void loadArtists()
        {
            artists.Clear();

            int index = 0;

            foreach (Music i in music)
            {
                artists.Add(Tuple.Create<string, int>(i.getArtist(), index));
                index++;
            }
            sortArtists();
        }


    }
}
