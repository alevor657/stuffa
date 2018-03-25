using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;
using System.Threading;
using WpfApp2;
using System.Windows.Threading;
using WpfApp2.pages;

namespace Stuffa
{
     public class MediaPlayer
	 {
        private Playlist masterPlaylist;
	 	 private List<Playlist> playlists;
		 private int currentPlaylist;

        private Container container;
        
		 public MediaPlayer()
		 {
			playlists = new List<Playlist>();
            masterPlaylist = new Playlist();
		 }

        public MediaPlayer(Container container)
        {
            string folder = Directory.GetCurrentDirectory().Substring(0, 16);
            this.container = container;

            this.masterPlaylist = new Playlist(folder + "\\playlists\\All music.txt");

            // start generate testplaylist

            playlists = new List<Playlist>();
            playlists.Add(new Playlist("Jonas bugg", 1));
            playlists.Add(new Playlist("Anders fox", 1));
            playlists.Add(new Playlist(Directory.GetCurrentDirectory().Substring(0, 16) + "\\playlists\\TestPlaylist.txt"));

            playlists[0].generateTestPlaylist();
            playlists.ElementAt(1).generateTestPlaylist();

            List<string> music = new List<string>();
            music.Add(Directory.GetCurrentDirectory().Substring(0, 16) + "\\Musik\\Scraping_The_Sewer.mp3");
            music.Add(Directory.GetCurrentDirectory().Substring(0, 16) + "\\Musik\\Young_And_Old_Know_Love.mp3");
            //playlists[2].loadNewMusic(music, false);

            int temp = currentPlaylist;
            currentPlaylist = 2;
            this.LoadNewMusic(music, false);
            currentPlaylist = temp;


            //end Test


            string[] fileTypes = new string[1];
            fileTypes[0] = ".txt";

            List<string> playListNames = ProcessDirectory(Directory.GetCurrentDirectory().Substring(0, 16) + "\\playlists", fileTypes);

            foreach(string name in playListNames)
            {
                if (!name.EndsWith("\\All music.txt"))
                {
                    playlists.Add(new Playlist(name));
                }

            }

        }

        public void SetCurrentPlaylist(int pos)
        {
            this.currentPlaylist = pos - 1;
        }

        public bool AddPlaylist(string playlistName)
        {
            // JUST FOR TESTING 
            // Uncomment this part when there is a search function here
            bool didAdd = false;
            // if(!searchPlaylist(playlist.getName())
            playlists.Add(new Playlist(playlistName));
            didAdd = true;
            //}

            return didAdd;
        }

        public List<Music> LoadNewMusic(List<string> paths, bool addAll = false)
        {
            this.masterPlaylist.loadNewMusic(paths, false);
            return this.playlists[this.currentPlaylist].loadNewMusic(paths, addAll);
        }

        public bool RemoveMusic(int index)
        {
            if (this.currentPlaylist == -1)
            {
                return false;// this.masterPlaylist.RemoveMusic(index);
            }
            return this.playlists[this.currentPlaylist].RemoveMusic(index);
        }

        public bool addNewPlaylist(string name)
        {
            this.playlists.Add(new Playlist(Directory.GetCurrentDirectory().Substring(0, 16) + "\\playlists\\" + name + ".txt"));

            return true;
        }

        public string GetCurrentPlaylistName()
        {
            string name;
            if (this.currentPlaylist == -1)
            {
                name = masterPlaylist.ToString();
            }
            else {
                name = this.playlists[this.currentPlaylist].ToString();
            }
            return name;
        }

        internal string GetSongStr(int selectedIndex)
        {
            
            if( this.currentPlaylist == -1)
            {
                return this.masterPlaylist.getMusic(selectedIndex).getFullPath();
            }

            return playlists[currentPlaylist].getMusic(selectedIndex).getFullPath();
        }
        internal Music GetSongObj(int selectedIndex)
        {
            if (this.currentPlaylist == -1)
            {
                return this.masterPlaylist.getMusic(selectedIndex);
            }

            return playlists[currentPlaylist].getMusic(selectedIndex);
        }

        public static void LoadPlaylist(int playlistPos)
        {
            
        }

        // Call this function to populate the list showing all possible playlists.
        public List<string> GetPlaylistNames()
        {
            List<string> temp = new List<string>();
            temp.Add(masterPlaylist.ToString());

            foreach (Playlist i in playlists)
            {
                temp.Add(i.ToString());
            }
            return temp;
        }
        // Call this function to populate the list showing all music in a playlist after selecting a playlist to edit
        public List<Music> GetMusicFromPlaylist()
        {
            //currentPlaylist = pos;
            //String[] temp = new String[playlists.Count];
            //int increment = 0;
            //foreach (Music i in playlists[pos].getMusic())
            //{
            //    // the ToString() method only returns the title currently, edit in case more information is desired
            //    temp[increment] = i.ToString();
            //    increment++;
            //}
            if (this.currentPlaylist == -1)
            {
                if(!masterPlaylist.getIfLoaded())
                {
                    masterPlaylist.loadMusic();
                }
                return this.masterPlaylist.getAllMusic();
            }

            
            if (!playlists[currentPlaylist].getIfLoaded())
            {
                //loads music from memory
                playlists[currentPlaylist].loadMusic();
            }

            return playlists[currentPlaylist].getAllMusic();
        }

        public bool DeletePlaylist(int index)
        {
            bool ret = false;
            index--;
            if (index >= 0 && index < playlists.Count)
            {
                File.Delete(playlists[index].getFullPath());
                this.playlists.RemoveAt(index);
                ret = true;
            }
            return ret;
        }

        //get the path to all files that is of type "fileTypes" in the directory "TargetDirectory"
        public static List<String> ProcessDirectory(string targetDirectory, string[] fileTypes)
        {
            List<string> files = new List<string>();
            // Process the list of files found in the directory.
            try
            {
                //get all files in directory
                string[] fileEntries = Directory.GetFiles(targetDirectory);

                //for each file in directory
                foreach (string fileName in fileEntries)
                {
                    //check if file ends with the specified "fileTypes"
                    for (int i = 0; i < fileTypes.Length; i++)
                    {
                        if (fileName.EndsWith(fileTypes[i]))
                        {
                            //add file to list
                            files.Add(fileName);

                        }
                    }
                }

                /*
                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessDirectoryMusic(subdirectory, list);*/
            }
            catch { }
            return files;
        }

    }
}