using MaterialDesignThemes.Wpf;
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
using Stuffa;
using System.Collections;

namespace WpfApp2.pages
{
    /// <summary>
    /// Interaction logic for Container.xaml
    /// </summary>
    public partial class Container : Page
    {

        bool inSettings = false;
        PlayerControl pc;
        EditView ev;
        Settings settings;
        PlaylistView pv;
        Stuffa.MediaPlayer mp;
        public Container()
        {
            mp = new Stuffa.MediaPlayer(this);
            pc = new PlayerControl(this);
            settings = new Settings(this);
            InitializeComponent();
            ev = new EditView(this);
            pv = new PlaylistView(this);
            playerControl.Content = pc;
            DynamicView.Content = ev;
            PlaylistView.Content = pv;
        }

        internal List<string> GetPlaylists()
        {
            return mp.GetPlaylistNames();
        }

        private void settingsButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(!inSettings)
            {
                DynamicView.Content= settings;
                inSettings = true;
            }
            else
            {
                DynamicView.Content = ev;
                inSettings = false;
            }
        }

        internal void showSelectedPlaylist()
        {
            if (pv != null)
            {
                mp.SetCurrentPlaylist(pv.PlaylistList.SelectedIndex);
                ev.LoadPlaylist(mp.GetMusicFromPlaylist());
                ev.PlaylistName.Content = mp.GetCurrentPlaylistName();
            }
        }

        internal void PlaySelectedSong()
        {
            pc.PlaySong(mp.GetSongObj(ev.currentPlaylist.SelectedIndex));
        }

        public void snackBarActivate(string message)
        {
            var messageQueue = SnackBarDialog.MessageQueue;

            //the message queue can be called from any thread
            Task.Factory.StartNew(() => messageQueue.Enqueue(message, "OKAY", () => { }));
        }

        internal void LoadNewMusic(List<string> paths)
        {
            //load new music into current playlist
            mp.LoadNewMusic(paths, false);
            this.showSelectedPlaylist();            
        }

        internal void removeMusic(int index)
        {
            mp.RemoveMusic(index);
            this.showSelectedPlaylist();

        }

        internal void searchAllMusic(string searchTerm)
        {
            ev.LoadSearch(mp.searchAllMusic(searchTerm));


        }

        internal void removePlaylist(int index)
        {
            mp.DeletePlaylist(index);
            pv.updatePlaylists(GetPlaylists());
        }

        internal void newPlaylist(string name)
        {
            if(mp.addNewPlaylist(name))
            {
                pv.updatePlaylists(GetPlaylists());
            }

        }

    }
}
