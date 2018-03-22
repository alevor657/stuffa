using MaterialDesignThemes.Wpf;
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
    public partial class PlaylistView : Page
    {
        Container container;
        EditView ev;
        List<Playlist> pl;
        public PlaylistView(Container c, EditView ev)
        {
            InitializeComponent();
            container = c;
            this.ev = ev;

            pl = new List<Playlist>();
            
            pl.Add(new Playlist("Best Songs", 0));
            pl.Add(new Playlist("Not my Best Songs", 1));

            //Music song1 = new Music("D:\\Nedladdningar\\Vicetone - Way Back(feat.Cozi Zuehlsdorff).mp3", "Way Back", "Vicetone", 108);
            //Music song2 = new Music("D:\\Nedladdningar\\Gustav_final.mp3", "Gustavs Final", "Gustav", 110);

            //pl[0].addNewSong(song1);
            //pl[0].addNewSong(song2);
            //pl[1].addNewSong(song2);
            pl[0].generateTestPlaylist();
            pl[1].generateTestPlaylist();
            PlaylistList.Items.Add("Best Songs");
            PlaylistList.Items.Add("Not my Best Songs");
        }



        public void AddPlaylist_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("Created playlist with:  " + (eventArgs.Parameter ?? ""));

            //you can cancel the dialog close:
            //eventArgs.Cancel();

            if (!Equals(eventArgs.Parameter, true)) return;

            if (!string.IsNullOrWhiteSpace(PlaylistName.Text))
            {
                PlaylistList.Items.Add(PlaylistName.Text.Trim());
                container.snackBarActivate(PlaylistName.Text.Trim() + " created!");
            }
        }

        private void PlaylistList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ev.loadPlaylist(pl[PlaylistList.SelectedIndex].getAllMusic());
        }
    }
}
