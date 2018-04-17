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
        Brush b;

        public PlaylistView(Container c)
        {
            InitializeComponent();
            b = this.NewPlaylistButton.Background;
            container = c;



            PlaylistList.ItemsSource = new List<string>(container.GetPlaylists());

            //pl = new List<Playlist>();

            //pl.Add(new Playlist("Best Songs", 0));
            //pl.Add(new Playlist("Not my Best Songs", 1));

            //Music song1 = new Music("D:\\Nedladdningar\\Vicetone - Way Back(feat.Cozi Zuehlsdorff).mp3", "Way Back", "Vicetone", 108);
            //Music song2 = new Music("D:\\Nedladdningar\\Gustav_final.mp3", "Gustavs Final", "Gustav", 110);

            //pl[0].addNewSong(song1);
            //pl[0].addNewSong(song2);
            //pl[1].addNewSong(song2);
            ////pl[0].generateTestPlaylist();
            ////pl[1].generateTestPlaylist();
            //PlaylistList.Items.Add("Best Songs");
            //PlaylistList.Items.Add("Not my Best Songs");
        }

        public void updatePlaylists(List<string> pl)
        {
            PlaylistList.ItemsSource = null;
            PlaylistList.ItemsSource = pl;

        }

        public void AddPlaylist_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("Created playlist with:  " + (eventArgs.Parameter ?? ""));

            //you can cancel the dialog close:
            //eventArgs.Cancel();

            if (!Equals(eventArgs.Parameter, true)) return;

           
                if (!string.IsNullOrWhiteSpace(PlaylistName.Text) && container.newPlaylist(PlaylistName.Text.Trim()))
                {
                    PlaylistList.SelectedIndex = PlaylistList.Items.Count - 1;
                    clearDialog();
                }
                else
                {
                    eventArgs.Cancel();

                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    mySolidColorBrush.Color = (Color)ColorConverter.ConvertFromString("#781714");

                   
                    PlaylistName.BorderBrush = mySolidColorBrush;

                    errTxt.Text = getWhyErr(PlaylistName.Text.Trim());
                }
            
        }

        private string getWhyErr(string s)
        {
            string ret = "";
            if(s.Contains('"'))
            {
                ret = "invalid character: \"";
            }
            else if (s.Contains('\\'))
            {
                ret = "invalid character: \\";
            }
            else if (s.Contains('/'))
            {
                ret = "invalid character: /";
            }
            else if (s.Contains('?'))
            {
                ret = "invalid character: ?";
            }
            else if (s.Contains('<'))
            {
                ret = "invalid character: <";
            }
            else if (s.Contains('>'))
            {
                ret = "invalid character: >";
            }
            else if (s.Contains('|'))
            {
                ret = "invalid character: |";
            }
            else
            {
                ret = "name allready exists";
            }
            return ret;
        }

        private void PlaylistList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistList.SelectedIndex != -1)
            {
                container.showSelectedPlaylist();
                PlaylistList.ScrollIntoView(PlaylistList.SelectedItem);
                //ev.loadPlaylist(pl[PlaylistList.SelectedIndex].getAllMusic());
            }
        }

        private void removePlaylistOnIndex(object sender, RoutedEventArgs e)
        {

            if (PlaylistList.SelectedIndex >= 0)
            {
                int index = 0;
                if (PlaylistList.SelectedIndex > 0)
                {
                    index = PlaylistList.SelectedIndex - 1;
                }

                container.removePlaylist(this.PlaylistList.SelectedIndex);

                PlaylistList.SelectedIndex = index;
                
            }
        }

        private void AddMusic(object sender, RoutedEventArgs e)
        {
            container.LoadNewMusic();
        }

        private void clearDialog()
        {
            Console.WriteLine("new playlist pressed");
            this.PlaylistName.Text = "";

            this.PlaylistName.BorderBrush = b;
            this.errTxt.Text = "";
            

        }

        internal bool isTextBoxActive()
        {
            return this.PlaylistName.IsSelectionActive;
        }
    }
}
