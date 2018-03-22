using Stuffa;
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

namespace WpfApp2.pages
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class EditView : Page
    {

        
        public EditView()
        {
            InitializeComponent();

            // Bara för testing
            //List<Songs> songsInPlaylist = new List<Songs>();
            //songsInPlaylist.Add(new Songs() { Bpm = 132, Title = "Way Back", Artist = "Vicetone" });
            //songsInPlaylist.Add(new Songs() { Bpm = 110, Title = "Fire", Artist = "Burning" });


            //currentPlaylist.ItemsSource = songsInPlaylist;

        }
        public void loadPlaylist(List<Music> musicInPlaylist)
        {
            List<Songs> songsInPlaylist = new List<Songs>();
            for (int i = 0; i < musicInPlaylist.Capacity; i++)
            {
                songsInPlaylist.Add(new Songs() { Bpm = musicInPlaylist[i].getBPM(), Title = musicInPlaylist[i].getTitle(), Artist = musicInPlaylist[i].getArtist() });
            }
            currentPlaylist.ItemsSource = songsInPlaylist;

        }
    }
}

public class Songs
{
    public int Bpm { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
}



