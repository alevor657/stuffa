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
        private string path;
        private string name;
        private string filetype;
        private List<Stuffa.Music> music;

        public Playlist()
        {
            this.music = new List<Stuffa.Music>();
        }

        public Playlist(string fullPath)
        {
            this.music = new List<Stuffa.Music>();

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

        public bool loadNewMusic(string path)
        {
            bool retVal = false;
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".mp3";
                dlg.Filter = "MP3 Files (*.mp3)|*.mp3|M4A Files (*.m4a)|*.m4a|FLAC Files (*.flac)|*.flac";

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();


                // Get the selected file name and display in a TextBox 
                if (result == true)
                {

                    // Open document 
                    string musicPath = dlg.FileName;

                    //check if music allready added
                    foreach (Music i in music)
                    {
                        if(i.getFullPath() == musicPath)
                        {
                            return false;
                        }
                    }

                    //add to array
                    music.Add(new Music(musicPath));


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


    }
}
