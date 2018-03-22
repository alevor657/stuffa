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
	 	 public static List<Playlist> playlists;
		 public static int currentPlaylist;

        private EditView ev;
        private PlayerControl pc;
        
		 public MediaPlayer()
		 {
			playlists = new List<Playlist>();
			//playlists.Add(new Playlist("Jonas bugg", 1));
			//playlists.Add(new Playlist("Anders fox", 1));
			// kommentar gällande testning därav svenska:

			// Kommentera ut nedanstående funktionerom det ej fungerar, försök lägga till musik som 
			//finns på egen dator enligt konstruktorer i Music istället för dessa två funktionsanrop.

			//playlists.ElementAt(0).generateTestPlaylist();
			//playlists.ElementAt(1).generateTestPlaylist();
		 }

        public MediaPlayer(EditView ev, PlayerControl pc)
        {
            this.ev = ev;
            this.pc = pc;
            playlists = new List<Playlist>();

        }

        public static bool addPlaylist(Playlist toAdd)
        {
            // JUST FOR TESTING 
            // Uncomment this part when there is a search function here
            bool didAdd = false;
            // if(!searchPlaylist(playlist.getName())
            playlists.Add(new Playlist(toAdd));
            didAdd = true;
            //}

            return didAdd;
        }

        public static void loadPlaylist(int playlistPos)
        {
            
        }

        // Call this function to populate the list showing all possible playlists.
        public static String[] getPlayListNames()
        {
            String[] temp = new String[playlists.Count];
            int increment = 0;
            foreach (Playlist i in playlists)
            {
                temp[increment] = i.ToString();
                increment++;
            }
            return temp;
        }
        // Call this function to populate the list showing all music in a playlist after selecting a playlist to edit
        public static List<Music> getMusicFromPlaylist(int pos)
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
            return playlists[pos].getAllMusic();
        }


    }
}