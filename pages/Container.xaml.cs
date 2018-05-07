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
using WpfApp2.feedback;
using System.Windows.Media.Animation;

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
        //DynamicView dv;

        Server s;
        Stuffa.MediaPlayer mp;

        public Container()
        {
            mp = new Stuffa.MediaPlayer(this);
            pc = new PlayerControl(this);
            settings = new Settings(this);
            //dv = new DynamicView(this);
            InitializeComponent();
            ev = new EditView(this);
            pv = new PlaylistView(this);
            s = new Server(this);

            playerControl.Content = pc;
            DynamicView.Content = ev;
            PlaylistView.Content = pv;
            SettingsView.Content = settings;
        }

        internal void TurnOnBPMShuffle()
        {
            pc.SetToBPMShuffle();
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

        private void settingsButtonOpenUp(object sender, MouseButtonEventArgs e)
        {
            // OLD FUNCTIONALITY
            //if (!inSettings)
            //{
            //    DynamicView.Content = settings;
            //    inSettings = true;
            //}
            //else
            //{
            //    DynamicView.Content = ev;
            //    inSettings = false;
            //}

            // NEW 
            DoubleAnimation slideIn = new DoubleAnimation();
            slideIn.AutoReverse = false;
            slideIn.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            var translateTo = new TranslateTransform(100, 0);
            var translateFrom = new TranslateTransform(0,100);
            SettingsView.RenderTransform = new TranslateTransform(100, 0);

                slideIn.From = 100.0;
                slideIn.To = 0.0;
                SettingsView.BeginAnimation(TranslateTransform.XProperty, slideIn, HandoffBehavior.SnapshotAndReplace);
                //SettingsView.BeginAnimation(OpacityProperty, slideIn);

                inSettings = true;

            //SettingsButtonClose.Visibility = Visibility.Visible;
            //SettingsButtonOpen.Visibility = Visibility.Collapsed;


        }

        private void settingsButtonCloseUp(object sender, MouseButtonEventArgs e)
        {
            // OLD FUNCTIONALITY
            //if (!inSettings)
            //{
            //    DynamicView.Content = settings;
            //    inSettings = true;
            //}
            //else
            //{
            //    DynamicView.Content = ev;
            //    inSettings = false;
            //}

            // NEW 
            DoubleAnimation slideIn = new DoubleAnimation();
            slideIn.AutoReverse = false;
            slideIn.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            var translateTo = new TranslateTransform(100, 0);
            var translateFrom = new TranslateTransform(0, 100);
            SettingsView.RenderTransform = new TranslateTransform(100, 0);

                slideIn.From = 0.0;
                slideIn.To = 100.0;
                SettingsView.BeginAnimation(TranslateTransform.XProperty, slideIn, HandoffBehavior.SnapshotAndReplace);
                //SettingsView.BeginAnimation(TranslateTransform.XProperty, slideIn);
                //SettingsView.BeginAnimation(OpacityProperty, slideIn);

                inSettings = false;

            //settingsButtonClose.Visibility = Visibility.Collapsed;
            //settingsButtonOpen.Visibility = Visibility.Visible;

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

        internal void PlaySelectedSongDb()
        {
            pc.PlaySong(ev.searchRes.SelectedItem as Music);
        }

        public void snackBarActivate(string message)
        {
            var messageQueue = SnackBarDialog.MessageQueue;

            //the message queue can be called from any thread
            Task.Factory.StartNew(() => messageQueue.Enqueue(message, "OKAY", () => { }));
            pc.setMobileActive();
        }

        public void snackBarActivatePlaylistCreated(string name)
        {
            ev.snackBarActivate(name + " created!");
        }

        internal void getBPM()
        {
            // Add code to delegate data to server then to phone
            int bpm = mp.getBPM();
        }

        internal void getInternval()
        {
            int interval = mp.getInterval();
        }
        internal bool LoadNewMusic(List<string> paths)
        {
            bool retVal = true;
            //load new music into current playlist
            List<Music> same = mp.LoadNewMusic(paths, false);
            this.showSelectedPlaylist();

            if (same.Count > 0)
            {
                retVal = false;
                ev.snackBarActivate();
            }
            return retVal;
        }

        internal void removeMusic(int index)
        {
            mp.RemoveMusic(index);
            this.showSelectedPlaylist();

        }

        internal void searchAllMusic(string searchTerm)
        {
            ev.LoadSearch(mp.searchAllMusic(searchTerm));
            //ev.setMarked(mp.GetMusicFromPlaylist(), mp.getAllBpm(105, 3));
        }

        internal void playBpm(int Bpm)
        {
            ev.setMarked(mp.GetMusicFromPlaylist(), mp.getMarksForBPMShuffle());
            this.changeBPM(Bpm);

        }

        internal void removePlaylist(int index)
        {
            mp.DeletePlaylist(index);
            pv.updatePlaylists(GetPlaylists());
        }

        internal bool newPlaylist(string name)
        {
            bool created = false;
            if (mp.addNewPlaylist(name))
            {
                created = true;
                pv.updatePlaylists(GetPlaylists());
                snackBarActivatePlaylistCreated(name);
            }
            return created;

        }
        internal void getRandomSong()
        {
            // Add switch for different shuffle states 
            int index = 0;
            int shuffleVal = pc.getShuffleState();
            if (ev.currentPlaylist.Items.Count > 0)
            {
                switch (shuffleVal)
                {
                    case 1:
                        index = mp.getIndexForNonShuffle();
                        break;
                    case 2:
                        index = mp.getIndexForNextSong();
                        break;
                    case 0:
                        index = mp.getIndexForBPMShuffle();
                        ev.setMarked(mp.GetMusicFromPlaylist(), mp.getMarksForBPMShuffle());
                        if (settings.GetAutoState())
                        {
                            int newBPM = settings.getBPM() + settings.GetRange();
                            settings.setBPM(newBPM);
                            mp.changeBPM(newBPM);

                        }
                        break;
                    default:
                        index = mp.getIndexForNonShuffle();
                        break;
                }

                Music temp = mp.GetSongObj(index);
                ev.setHighlight(index);
                pc.PlaySong(temp);
            }


        }
        // Sets the interval
        internal void setInterval(int interval)
        {
            mp.setInterval(interval);
            this.SendStateToServerOnUpdate();
        }
        // arguement = new BPM
        internal void changeBPM(int newBPM)
        {
            mp.changeBPM(newBPM);
        }

        internal void LoadNewMusicDatabse()
        {
            mp.LoadNewMusicSQL(getNewMusicPaths());
        }

        internal void LoadNewMusicDatabase(List<string> paths)
        {
            mp.LoadNewMusicSQL(paths);
        }
        internal void LoadNewMusic()
        {
            LoadNewMusic(getNewMusicPaths());
        }

        internal List<string> getNewMusicPaths()
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
                    return dlg.FileNames.ToList<string>();


                }

            }
            catch
            {

            }
            return null;

        }

        internal void AddDupletts()
        {
            mp.AddDupletts();
            this.showSelectedPlaylist();


        }

        internal void spacePressed()
        {
            //check if the searchbox is highlited
            if (!ev.IsSearchBarActive() && !pv.isTextBoxActive())
            {
                //pause the music
                this.TogglePlay();
            }
        }

        public Dictionary<string, object> getPlayerState()
        {
            Dictionary<string, object> send = new Dictionary<string, object>();
            foreach(var i in pc.getPlayerState())
            {
                send.Add(i.Key, i.Value);
            }

            foreach(var i in settings.getPlayerState())
            {
                send.Add(i.Key, i.Value);
            }

            Console.WriteLine("Populated state is:");
            foreach (var val in send)
            {
                Console.WriteLine(val);
            }
            Console.WriteLine("======================");

            return send;
}

        public void TogglePlay() => pc.TogglePlay();
        public void NextSong() => pc.NextSong();
        public void GetCurrentVolumeAsInt() => pc.GetCurrentVolumeAsInt();

        public void SendStateToServerOnUpdate()
        {
            string json = JsonConvert.SerializeObject(getPlayerState());
            s.Send(ServerMsg.Create(SocketServer.Action.UPDATE, json));
        }

        internal void MoveMusic(int from, int to)
        {
            mp.MoveMusic(from, to);

            

            this.showSelectedPlaylist();
        }

        private void CheckIfSpace(object sender, KeyEventArgs e)
        {

            Console.WriteLine(e.Key.ToString());
            if (e.Key.ToString() == "Space")
            {
                spacePressed();
            }
            
        }


        internal void EditMusic(Music toEdit, string newBpm, string newTitle, string newArtist)
        {
            bool resumeMusic = false;
            //can find songs with same name and thing they are the same i know
            if(pc.getMusicTitle() == toEdit.getTitle())
            {
                resumeMusic = true;
                //unload to be able to edit id3 tags
                pc.unloadMusicImmediately();
            }

            //set bpm
            int newBpmInt = 0;
            if(Int32.TryParse(newBpm, out newBpmInt))
            {
                toEdit.setBPM(newBpmInt);
            }

            //set Title
            toEdit.setTitle(newTitle);

            //set artist
            toEdit.setArtist(newArtist);

            //update database
            mp.updateDatabase(toEdit);

            if (resumeMusic)
            {
                pc.PlaySong(toEdit);
            }
        }

        public void Replay()
        {
            pc.RestartSong();
        }

        public void ChangeJump(int newVal)
        {
            settings.ChangeJump(newVal);
        }


        public void SetInterval(int val)
        {
            settings.SetInterval(val);
        }
        public void SetVolume(float val)
        {
            pc.SetVolume(val);
        }

        public void ToggleAutoplay()
        {
            settings.ToggleAutoplay();
        }

        public void SetBaseBpm(int val) => settings.SetBaseBpm(val);
    }
}
