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
        MasterPlaylist master;

        private int currentPlaylist;
		private List<int> recentlyPlayedIndexes; 
        private Container container;

        private List<Music> sameMusic;
        
		 public MediaPlayer()
		 {
			playlists = new List<Playlist>();
            masterPlaylist = new Playlist();
		 }

        public MediaPlayer(Container container)
        {
            master = new MasterPlaylist();
            string folder = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            this.container = container;
			
            this.masterPlaylist = new Playlist(folder + "\\playlists\\All music.txt");
			this.recentlyPlayedIndexes = new List<int>();
            // start generate testplaylist

            
            playlists = new List<Playlist>();
            /*playlists.Add(new Playlist("Jonas bugg", 1));
            playlists.Add(new Playlist("Anders fox", 1));
            playlists.Add(new Playlist(Directory.GetCurrentDirectory().Substring(0, 16) + "\\playlists\\TestPlaylist.txt"));

            playlists[0].generateTestPlaylist();
            playlists.ElementAt(1).generateTestPlaylist();

            List<string> music = new List<string>();
            music.Add(Directory.GetCurrentDirectory().Substring(0, 16) + "\\Musik\\Scraping_The_Sewer.mp3");
            music.Add(Directory.GetCurrentDirectory().Substring(0, 16) + "\\Musik\\Young_And_Old_Know_Love.mp3");*/
            //playlists[2].loadNewMusic(music, false);
            // Find solution to populating the list with indexes greater than the maximum number of songs.
            for (int i=0; i < 5; i++)
            {
                this.recentlyPlayedIndexes.Insert(i, (this.masterPlaylist.getSize() + 1));
            }
            /*int temp = currentPlaylist;
            currentPlaylist = 2;
            this.LoadNewMusic(music, false);
            currentPlaylist = temp;*/

            
            //end Test


            string[] fileTypes = new string[1];
            fileTypes[0] = ".txt";

            List<string> playListNames = ProcessDirectory(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()) + "\\playlists", fileTypes);

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

        /*
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
        */
        public List<Music> LoadNewMusic(List<string> paths, bool addAll = false)
        {
            if(!this.masterPlaylist.getIfLoaded())
            {
                this.masterPlaylist.loadMusic();
            }
            master.InsertNewMusicThread(paths);
            //this.masterPlaylist.loadNewMusic(paths, false);

            sameMusic = this.playlists[this.currentPlaylist].loadNewMusic(paths, addAll);
            return sameMusic;
        }

        //add the latest found dupletts to the CURRENT playlist
        public void AddDupletts()
        {
            this.playlists[currentPlaylist].loadNewMusic(sameMusic, true);
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
            bool created = false;
            //Console.WriteLine(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()));
            if(ValuídPlaylistName(name))
            {
                this.playlists.Add(new Playlist(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()) + "\\playlists\\" + name + ".txt"));
                playlists[playlists.Count - 1].savePlaylist();
                created = true;
            }




            return created;
        }

        private bool ValuídPlaylistName(string name)
        {
            bool retVal = false;
            if (!(name.Contains('"') || name.Contains("/") || name.Contains("\\") || name.Contains(":") || name.Contains("?") || name.Contains("<") || name.Contains(">") || name.Contains("|")))
            {
                
                retVal = true;
                foreach(Playlist i in playlists)
                {
                    if(i.ToString() == name)
                    {
                        retVal = false;
                    }
                }
            }
            return retVal;
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
            if(currentPlaylist >= 0 && currentPlaylist < playlists.Count)
            {
                return playlists[currentPlaylist].getMusic(selectedIndex);
            }
            else
            {
                return null;
            }
        }

        public List<Music> searchAllMusic(string searchTerm)
        {
            return master.search(searchTerm);//this.masterPlaylist.searchArtistBpmTitle(searchTerm);
        }

        public List<int> getAllBpm(int Bpm, int range = 1)
        {
            if (currentPlaylist == -1)
            {
                return masterPlaylist.searchBPMIndex(Bpm, range);
            }
            else
            {
                return this.playlists[currentPlaylist].searchBPMIndex(Bpm, range); ;
            }
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

		public int getIndexForNextSong()
		{
			Random r = new Random();

			int indexForNext = r.Next(0, this.playlists[this.currentPlaylist].getSize());
			if (this.playlists[this.currentPlaylist].getSize() > 5)
			{
				while (this.recentlyPlayedIndexes.Contains(indexForNext))
				{
					indexForNext = r.Next(0, this.playlists[this.currentPlaylist].getSize());
				}
			}
			this.recentlyPlayedIndexes.RemoveAt(4);
			this.recentlyPlayedIndexes.Insert(0, indexForNext);
			return indexForNext;
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