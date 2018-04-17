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
using SocketServer;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

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
        Server s;
        Stuffa.MediaPlayer mp;

        public Container()
        {
            mp = new Stuffa.MediaPlayer(this);
            pc = new PlayerControl(this);
            settings = new Settings(this);
            InitializeComponent();
            ev = new EditView(this);
            pv = new PlaylistView(this);
            s = new Server(this);

            playerControl.Content = pc;
            DynamicView.Content = ev;
            PlaylistView.Content = pv;
        }


        [FlagsAttribute]
        private enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            // Legacy flag, should not be used. 
            // ES_USER_PRESENT   = 0x00000004, 
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
        }

        private static class SleepUtil
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        }

        public void PreventSleep()
        {
            if (SleepUtil.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                | EXECUTION_STATE.ES_SYSTEM_REQUIRED
                | EXECUTION_STATE.ES_AWAYMODE_REQUIRED) == 0) //Away mode for Windows >= Vista 
                SleepUtil.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                    | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                    | EXECUTION_STATE.ES_SYSTEM_REQUIRED); //Windows < Vista, forget away mode 
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

        public void snackBarActivatePlaylistCreated(string name)
        {
            ev.snackBarActivate(name + " created!");
        }

        internal void LoadNewMusic(List<string> paths)
        {
            //load new music into current playlist
            List<Music> same = mp.LoadNewMusic(paths, false);
            this.showSelectedPlaylist();

            if (same.Count > 0)
            {
                ev.snackBarActivate();
            }
        }

        internal void removeMusic(int index)
        {
            mp.RemoveMusic(index);
            this.showSelectedPlaylist();

        }

        internal void searchAllMusic(string searchTerm)
        {
            ev.LoadSearch(mp.searchAllMusic(searchTerm));
            ev.setMarked(mp.GetMusicFromPlaylist(), mp.getAllBpm(105, 3));
        }

        internal void playBpm(int Bpm, int range)
        {
            ev.setMarked(mp.GetMusicFromPlaylist(), mp.getAllBpm(Bpm, range));

        }

        internal void removePlaylist(int index)
        {
            mp.DeletePlaylist(index);
            pv.updatePlaylists(GetPlaylists());
        }

        internal bool newPlaylist(string name)
        {
            bool created = false;
            if(mp.addNewPlaylist(name))
            {
                created = true;
                pv.updatePlaylists(GetPlaylists());
                snackBarActivatePlaylistCreated(name);
            }
            return created;

        }
		internal void getRandomSong()
		{
			int index = mp.getIndexForNextSong();
			Music temp = mp.GetSongObj(index);
			pc.PlaySong(temp);
		}


        internal void LoadNewMusic()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".mp3";
                dlg.Filter = "MP3 Files (*.mp3)|*.mp3|M4A Files (*.m4a)|*.m4a|FLAC Files (*.flac)|*.flac";
                dlg.Multiselect = true;

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();


                // if there is any files 
                if (result == true)
                {

                    // get file paths
                    List<string> musicPaths = dlg.FileNames.ToList<string>();
                    LoadNewMusic(musicPaths);
                    

                }

            }
            catch
            {

            }

        }

        internal void AddDupletts()
        {
            mp.AddDupletts();
            this.showSelectedPlaylist();


        }

        internal void spacePressed()
        {
            //check if the searchbox is highlited
            if(!ev.IsSearchBarActive() && !pv.isTextBoxActive())
            {
                //pause the music
                this.TogglePlay();
            }
        }

        public Dictionary<string, object> getPlayerState() => pc.getPlayerState();
        public void TogglePlay() => pc.TogglePlay();
        public void NextSong() => pc.NextSong();

        public void SendStateToServerOnUpdate()
        {
            string json = JsonConvert.SerializeObject(getPlayerState());
            s.Send(ServerMsg.Create(SocketServer.Action.REQUEST_STATE_SUCCESS, json));
        }

        private void CheckIfSpace(object sender, KeyEventArgs e)
        {

            Console.WriteLine(e.Key.ToString());
            if (e.Key.ToString() == "Space")
            {
                spacePressed();
            }
            
        }
    }
}
