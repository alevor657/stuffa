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
	 	 public List<Playlist> playlists;
		 public int currentPlaylist;

        private Container container;
        
		 public MediaPlayer()
		 {
			playlists = new List<Playlist>();
		 }

        public MediaPlayer(Container container)
        {
            this.container = container;

            playlists = new List<Playlist>();
            playlists.Add(new Playlist("Jonas bugg", 1));
            playlists.Add(new Playlist("Anders fox", 1));
            playlists.Add(new Playlist(Directory.GetCurrentDirectory().Substring(0, 16) + "\\playlists\\TestPlaylist.txt"));

            playlists[0].generateTestPlaylist();
            playlists.ElementAt(1).generateTestPlaylist();

            List<string> music = new List<string>();
            music.Add(Directory.GetCurrentDirectory().Substring(0, 16) + "\\Musik\\Scraping_The_Sewer.mp3");
            music.Add(Directory.GetCurrentDirectory().Substring(0, 16) + "\\Musik\\Young_And_Old_Know_Love.mp3");
            playlists[2].loadNewMusic(music, false);

        }

        public void SetCurrentPlaylist(int pos)
        {
            this.currentPlaylist = pos;
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
            return this.playlists[this.currentPlaylist].loadNewMusic(paths, addAll);
        }

        public bool RemoveMusic(int index)
        {
            return this.playlists[this.currentPlaylist].RemoveMusic(index);
        }

        public string GetCurrentPlaylistName()
        {
            return this.playlists[this.currentPlaylist].ToString();
        }

        internal string GetSongStr(int selectedIndex)
        {
            return playlists[currentPlaylist].getMusic(selectedIndex).getFullPath();
        }
        internal Music GetSongObj(int selectedIndex)
        {
            return playlists[currentPlaylist].getMusic(selectedIndex);
        }

        public static void LoadPlaylist(int playlistPos)
        {
            
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
            //currentPlaylist = pos;
            //String[] temp = new String[playlists.Count];
            //int increment = 0;
            //foreach (Music i in playlists[pos].getMusic())
            //{
            //    // the ToString() method only returns the title currently, edit in case more information is desired
            //    temp[increment] = i.ToString();
            //    increment++;
            //}
            return playlists[currentPlaylist].getAllMusic();
        }


    }
}