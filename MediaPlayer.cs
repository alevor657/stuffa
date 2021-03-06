﻿using System;
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
using System.Collections.Generic;
using System.Threading;
using WpfApp2;
using System.Windows.Threading;
using WpfApp2.pages;

namespace Stuffa
{
     public class MediaPlayer
	 {
	 	private List<Playlist> playlists;
        MasterPlaylist master;

        private int currentPlaylist;
		private List<int> recentlyPlayedIndexes;
        private int mostRecentSong;
        private Container container;

        private List<Music> sameMusic;
        


        private int indexForNonShuffle;
        private int BPM;
        private int BPMInterval;
         public MediaPlayer()
		 {
			playlists = new List<Playlist>();
		 }
        // Sets BPM + range
        public void setBpm(int bpm, int range)
        {
            this.BPM = bpm;
            this.BPMInterval = range;
        }
        // Gets BPM only
        public int getBPM()
        {
            return this.BPM;
        }
        // Gets BPM interval only
        public int getInterval()
        {
            return this.BPMInterval;
        }
        // 
        public void MoveMusic(int from, int to)
        {
            this.playlists[currentPlaylist].MoveMusic(from, to);
        }
        // Constructor, initalizes variables and playlist(s)
        public MediaPlayer(Container container)
        {
            master = new MasterPlaylist();
            string folder = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            this.container = container;
            this.indexForNonShuffle = 0;
			this.recentlyPlayedIndexes = new List<int>();
            this.BPM = 208;
            this.BPMInterval = 2;
            this.mostRecentSong = -1;
            // start generate testplaylist
            
            
            playlists = new List<Playlist>();
           
            for (int i=0; i < 5; i++)
            {
                this.recentlyPlayedIndexes.Insert(i, -1);
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

        public void updateDatabase(Music m)
        {
            this.master.updateMusic(m.getFullPath(), m.Bpm, m.Title, m.Artist);
        }
        // Changes the cuurrent active playlist and resets playlist-specific settings 
        public void SetCurrentPlaylist(int pos)
        {
            this.currentPlaylist = pos;
            this.indexForNonShuffle = 0;
            for (int i=0; i < 5; i++)
            {
                this.recentlyPlayedIndexes[i] = -1;
            }
        }

        public int  CountMusic()
        {
            int count = 0;
            if (currentPlaylist != -1)
            {
                count = playlists[currentPlaylist].getSize();
            }
            return count;
        }

       
        public List<Music> LoadNewMusic(List<string> paths, bool addAll = false)
        {

            master.InsertNewMusicThread(paths);

            sameMusic = this.playlists[this.currentPlaylist].loadNewMusic(paths, addAll);
            return sameMusic;
        }


        //adds new music to databse not any playlist
        public void LoadNewMusicSQL(List<string> paths)
        {
            master.InsertNewMusicThread(paths);
        }


        //add the latest found dupletts to the CURRENT playlist
        public void AddDupletts()
        {
            this.playlists[currentPlaylist].loadNewMusic(sameMusic, true);
        }

        public bool RemoveMusic(int index)
        {

            return this.playlists[this.currentPlaylist].RemoveMusic(index);
        }

        //Adds a new playlist to the list of playlists and creates files for it
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
        // Checks for unallowed characters in playlist names
        private bool ValuídPlaylistName(string name)
        {
            bool retVal = false;
            if (!(name == "" || name.Contains('"') || name.Contains("/") || name.Contains("\\") || name.Contains(":") || name.Contains("?") || name.Contains("<") || name.Contains(">") || name.Contains("|")))
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

        // Gets names of playlists 
        public string GetCurrentPlaylistName()
        {
            string name = "no playlist selected";
            if (this.currentPlaylist != -1)
            {
                name = this.playlists[this.currentPlaylist].ToString();

            }

            return name;
        }

        internal string GetSongStr(int selectedIndex)
        {
            


            return playlists[currentPlaylist].getMusic(selectedIndex).getFullPath();
        }

        internal void setInterval(int interval)
        {
            if (interval < 0)
            {
                this.BPMInterval = 0;
            }
            else
            {
                this.BPMInterval = interval;
            }
        }

        internal void changeBPM(int newBPM)
        {
            this.BPM = newBPM;
        }

        internal Music GetSongObj(int selectedIndex)
        {

            if(currentPlaylist >= 0 && currentPlaylist < playlists.Count)
            {
                this.recentlyPlayedIndexes.RemoveAt(4);
                this.recentlyPlayedIndexes.Insert(0, selectedIndex);
                this.indexForNonShuffle = selectedIndex;
                return playlists[currentPlaylist].getMusic(selectedIndex);
                this.mostRecentSong = selectedIndex;
            }
            else
            {
                return null;
            }
        }

        public List<Music> searchAllMusic(string searchTerm, bool searchNew = false)
        {
            List<Music> ret = master.search(searchTerm, false, searchNew);
            if(ret.Count > 400)
            {
                ret = ret.GetRange(0, 399);
            }
            return ret;
        }

        public List<int> getAllBpm(int Bpm, int range = 1)
        {
            if (currentPlaylist != -1)
            {
                return this.playlists[currentPlaylist].searchBPMIndex(Bpm, range); ;
            }
            return null;

        }


        // Call this function to populate the list showing all possible playlists.
        public List<string> GetPlaylistNames()
        {
            List<string> temp = new List<string>();


            foreach (Playlist i in playlists)
            {
                temp.Add(i.ToString());
            }
            return temp;
        }
        // Call this function to populate the list showing all music in a playlist after selecting a playlist to edit
        public List<Music> GetMusicFromPlaylist()
        {


            if (currentPlaylist != -1)
            {

                if (!playlists[currentPlaylist].getIfLoaded())
                {
                    //loads music from memory
                    playlists[currentPlaylist].loadMusic();
                }
                return playlists[currentPlaylist].getAllMusic();
            }
            return null;

        }

        public bool DeletePlaylist(int index)
        {
            bool ret = false;
            
            if (index >= 0 && index < playlists.Count)
            {
                File.Delete(playlists[index].getFullPath());
                this.playlists.RemoveAt(index);
                ret = true;
            }
            return ret;
        }

        public void sortOnChoice(int choice)
        {
            switch(choice)
            {
                case 0:
                    this.playlists[currentPlaylist].SortListOnBPM();
                    break;
                case 1:
                    this.playlists[currentPlaylist].SortListOnArtist();
                    break;
                case 2:
                    this.playlists[currentPlaylist].SortListOnTitle();
                    break;
                default:
                    break;
            }

        }

		public int getIndexForNextSong()
		{
			Random r = new Random();
            bool validBPM = false;
			int indexForNext = r.Next(0, this.playlists[this.currentPlaylist].getSize());
			if (this.playlists[this.currentPlaylist].getSize() > 5)
			{
                if (this.playlists[currentPlaylist].getBPM(indexForNext) != -1)
                {
                    validBPM = true;
                }
                while (this.recentlyPlayedIndexes.Contains(indexForNext) || !validBPM)
				{
                    validBPM = false;
					indexForNext = r.Next(0, this.playlists[this.currentPlaylist].getSize());
                    if (this.playlists[currentPlaylist].getBPM(indexForNext) != -1)
                    {
                        validBPM = true;
                    }
                }
			}
			
			return indexForNext;
		}
        public int getIndexForBPMShuffle()
        {
            bool validBPM = false;
            Random r = new Random();
            List<int> temp = this.playlists[this.currentPlaylist].searchBPMIndex(this.BPM, this.BPMInterval);
            int tempInterval = this.BPMInterval;
            while (temp.Count() < 6 && ((tempInterval < (this.BPMInterval*2)) || (temp.Count < 1) ))
            {
                temp = this.playlists[this.currentPlaylist].searchBPMIndex(this.BPM, ++tempInterval);
            }

            int randNr = r.Next(0, temp.Count);
            int index = temp.ElementAt<int>(randNr);
            if (this.playlists[currentPlaylist].getBPM(index) != -1)
            {
                validBPM = true;
            }
            if (temp.Count >= 6)
            {
                while (this.recentlyPlayedIndexes.Contains(index) || !validBPM)
                {
                    validBPM = false;
                    randNr = r.Next(0, temp.Count);
                    index = temp.ElementAt<int>(randNr);
                    if (this.playlists[currentPlaylist].getBPM(index) != -1)
                    {
                        validBPM = true; ;
                    }
                }
            }
            else if (temp.Count >= 2)
            {
                while (index == mostRecentSong)
                {
                    randNr = r.Next(0, temp.Count);
                    index = temp.ElementAt<int>(randNr);
                }
            }
            return index;


        }

        internal List<Music> searchPlaylist(int pos, string searchTerm)
        {
            if(pos < playlists.Count && pos >= 0)
            {
                return playlists[pos].searchArtistBpmTitle(searchTerm);
            }
            return null;
        }

        public List<int> getMarksForBPMShuffle()
        {
            Random r = new Random();
            List<int> temp = this.playlists[this.currentPlaylist].searchBPMIndex(this.BPM, this.BPMInterval);
            int tempInterval = this.BPMInterval;
            while (temp.Count() < 6 && ((tempInterval < (this.BPMInterval * 2)) || (temp.Count < 1)))
            {
                temp = this.playlists[this.currentPlaylist].searchBPMIndex(this.BPM, ++tempInterval);
            }
            return temp;
        }

        public int getIndexForNonShuffle()
        {
            
            this.indexForNonShuffle++;
            if (indexForNonShuffle % this.playlists[currentPlaylist].getSize() == 0)
            {
                this.indexForNonShuffle = 0;
            }
            while (this.playlists[currentPlaylist].getBPM(indexForNonShuffle) == -1)
            {
                this.indexForNonShuffle++;
            }
            return indexForNonShuffle;
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

        internal bool IsPlaylistSelected()
        {
            if(currentPlaylist >= 0)
            {
                return true;
            }
            return false;
        }

        internal void RemoveMusicLibrary(Music m)
        {
            master.Remove(m);
        }
    }
}