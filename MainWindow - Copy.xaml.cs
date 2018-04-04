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
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Windows.Threading;

namespace Stuffa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        PlaylistView playlistPage = new PlaylistView();
        Settings settingsPage = new Settings();

        //deaclear variables
        int settingsCounter = 0;
        bool isMaximized = false;
        static bool isPlaying = false;
        static bool mediaFileIsOpen = false;
        static Playlist curentPlaylist;

        DispatcherTimer songTimer = new DispatcherTimer();


        //chenge BPM on current music
        public void changeBPM(Music m, string bpm)
        {
            if (player.Source.ToString() == m.getFullPath())
            {
                player.Source = null;
            }
            if (!m.setBPM(bpm))
            {
                MessageBox.Show("Did not change BPM\nCheck if the file is open in another program\n" + m.getFullPath(), "open?",
    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //loads current playlist into ListBox
        public static void LoadCurentPlaylist(ListBox playlistList)
        {
            //tell currentPlaylist to get all the music
            curentPlaylist.loadMusic();

            //get all music from currentPlayliist
            List<Music> toAdd = curentPlaylist.getMusic();

            //remove all old items from the ListBox item
            playlistList.Items.Clear();
            // add the "back" button first on the playlistList
            playlistList.Items.Add("back");

            // add every Music to the playlistList
            foreach (Music i in toAdd)
            {
                playlistList.Items.Add(i);
            }



        }
        // DENNA SKA FÖRAS ÖVER TILL PLAYLIST FÖNSTRET SOM FINNS ---------------------------------------------------
        //insert the given playlistList of music into the given ListBox
        private static void showMusic(List<Music> music, ListBox playlistList)
        {
            // remove all old items from ListBox
            playlistList.Items.Clear();
            // add back button
            playlistList.Items.Add("back");
            // inser desired items
            foreach (Music i in music)
            {
                playlistList.Items.Add(i);
            }
        }

        //get the path to the playlists, this is lokated in the folder "Musik"
        public static string getPlaylistsPath()
        {
            // get the path to the current executing file
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;

            // fix so the path leads to the folder "Music"
            int pos = path.LastIndexOf('\\');
            path = path.Substring(0, pos);

            pos = path.LastIndexOf('\\');
            path = path.Substring(0, pos);

            pos = path.LastIndexOf('\\');
            path = path.Substring(0, pos);
            /*
            pos = path.LastIndexOf('\\');
            path = path.Substring(0, pos);
            */
            path += "\\Musik";
            return path;
        }

        // view all the playlists
        public static void goToPlaylists(ListBox playlistList)
        {

            ProcessDirectoryJson(getPlaylistsPath(), playlistList);
        }

        //get the path to all files that is of type "fileTypes" in the directory "TargetDirectory"
        public static List<String> ProcessDirectory(string targetDirectory, ListBox playlistList, string[] fileTypes, int cap)
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
                    for (int i = 0; i < cap; i++)
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

        //get JSON files (.txt) and show them as Playlists on the given ListBox
        public static void ProcessDirectoryJson(string targetDirectory, ListBox playlistList)
        {
            // get only .txt files
            string[] fileTypes = new string[1];
            fileTypes[0] = ".txt";

            // clear the given ListBox
            playlistList.Items.Clear();

            // get all the files in targetDirecctory withs ends with .txt and show them as Playlists
            ProcessPlaylist(ProcessDirectory(targetDirectory, playlistList, fileTypes, 1), playlistList);
        }

        // get .mp3 and .m4a files and show them on playlistList
        public static void ProcessDirectoryMusic(string targetDirectory, ListBox playlistList)
        {
            // remove all old items
            playlistList.Items.Clear();

            // get only .mp3 and .m4a files
            string[] fileTypes = new string[2];
            fileTypes[0] = ".mp3";
            fileTypes[1] = ".m4a";
            //this line first takes all the matching files in the funktion ProcessDirectory
            //then it adds the musik to the playlistList in processMusic
            showMusic(ProcessDirectory(targetDirectory, playlistList, fileTypes, 2), playlistList);

        }

        //inset Playlist(s) into ListBox
        public static void ProcessPlaylist(List<string> paths, ListBox playlistList)
        {


            for (int i = 0; i < paths.Count; ++i)
            {
                playlistList.Items.Add(new Playlist(paths[i]));
            }
        }

        //insert one or more Music into ListBox
        public static void showMusic(List<string> paths, ListBox playlistList)
        {
            for (int i = 0; i < paths.Capacity; ++i)
            {
                playlistList.Items.Add(new Music(paths[i]));
            }

        }

        //pause or Play the mediaElement -------------------- EVENTUELLT ONÖDIG


        public void pausePlay()
        {

            if (isPlaying)
            {

                player.Pause();
                isPlaying = false;
            }
            else
            {
                player.Play();
                isPlaying = true;
            }

        }

        //ToDo uppdate/remove
        public void pausePlayServer()
        {
            this.Dispatcher.Invoke(() => { pausePlay(); });
        }
        //ToDo uppdate/remove
        public void startServer()
        {
            Server server = new Server();
            while (true)
            {
                string message = server.startServer();
                if (message == "P")
                {
                    pausePlayServer();
                }
                this.Dispatcher.Invoke(() => { /*serverMessage.Text = "message from  server: " + message;*/ });

            }
        }



        //main function
        public MainWindow()
        {
            // give this thread a name 
            Thread.CurrentThread.Name = "parent";
            InitializeComponent();
            PlaylistView playlistPage = new PlaylistView();
            Settings settingsPage = new Settings();
            DynamicView.Content = playlistPage;

            // show all playlists
            goToPlaylists(playlistList);

            //start a temporary server until better is developed TODO: Update/remove
            Thread serverThread = new Thread(startServer);
            serverThread.IsBackground = true;
            serverThread.Start();

            songTimer.Interval = new TimeSpan(500);
            songTimer.Tick += TimerTicker;
        }

        void TimerTicker(object sender, EventArgs e)
        {

            trackSlider.Value = player.Position.TotalSeconds;
            TimeSpan currentTime = new TimeSpan(0, player.Position.Duration().Minutes, player.Position.Duration().Seconds);
            trackTime.Content = currentTime.ToString().Substring(3);
        }


        //here follows all buttons

        // when user selects another element in  playlistList
        private void playlistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistList.SelectedItem != null)
            {

                string name = playlistList.SelectedItem.ToString();
                Console.WriteLine("file: " + name);

                if (false) //TODO: if it is a playplaylistList 
                {

                }
                else
                {
                    try
                    {
                        //convert selected item to Music
                        Music music = playlistList.SelectedItem as Music;
                        //get path to file
                        string path = music.getFullPath();

                        TagLib.File tagFile = TagLib.File.Create(path);
                        string songName = tagFile.Tag.Title;
                        var length = tagFile.Properties.Duration;

                        //label.Content = length;

                        //text.Text = path + "\n\n" + "Title: " + songName + "\nBPM: ";

                        player.Source = new Uri(path, UriKind.RelativeOrAbsolute);


                        player.Play();
                        songTimer.Start();
                        isPlaying = true;
                        BitmapImage image = new BitmapImage(new Uri("../img/pause-white.png", UriKind.Relative));
                        //playButton.Source = image;
                    }
                    catch
                    {
                    }
                }
                // Denna funkar bara efter att man har lagt till  en ny låt - därför utkommenterad
                //textBox.Text = "Path: " + songs[0].path + " Title: " + songs[0].name;
            }


        }


        // pause play button

        private void playButtonUp(object sender, MouseButtonEventArgs e)
        {

            //if (mediaFileIsOpen)
            //{



                if (isPlaying)
                {

                    BitmapImage image = new BitmapImage(new Uri("../img/play-white.png", UriKind.Relative));
                    playButton.Source = image;
                    isPlaying = false;
                    player.Pause();
                    songTimer.Stop();

                }
                else
                {

                    BitmapImage image = new BitmapImage(new Uri("../img//pause-white.png", UriKind.Relative));
                    playButton.Source = image;
                    isPlaying = true;
                    player.Play();
                    songTimer.Start();
                }
            //}
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        // open file explorer and get new awzome music
        private void open_song_click(object sender, RoutedEventArgs e)
        {
            if (curentPlaylist != null)
            {
                curentPlaylist.loadNewMusic();
            }
        }

        private void trackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Position = TimeSpan.FromSeconds(trackSlider.Value);
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            /*
            List<string> musicTracks = new List<string>();
            foreach (var i in list.Items)
            {
                musicTracks.Add(i.ToString());
            }
            // package Newtonsoft.Json (Json.Net) need to be installed. 
            // to install go to "project" > "manage NuGet packages..." > "Brows" > type "Newtonsoft.Json" / "Json.Net" > "install"
            string json = JsonConvert.SerializeObject(musicTracks.ToArray());
            System.IO.File.WriteAllText(@"D:\path8ihbjhgnbbv.txt", json);*/


        }
        // load the selected playlist if the user dont want to dubbleclick
        private void load_Click(object sender, RoutedEventArgs e)
        {
            //List<string> musicTracks = new List<string>();
            if (playlistList.SelectedItem.ToString() != null)
            {
                curentPlaylist = playlistList.SelectedItem as Playlist;
                LoadCurentPlaylist(playlistList);
                //curPlaylistBox.Text = curentPlaylist.ToString();
            }
            /*
            using (StreamReader r = new StreamReader(@"D:\path8ihbjhgnbbv.txt"))
            {
                string json = r.ReadToEnd();
                List<string> musicTracks = JsonConvert.DeserializeObject<List<string>>(json);
                foreach (string i in musicTracks.ToArray())
                {
                    list.Items.Add(i);
                }
            }*/

        }

        // if user doubleclick on list element. then go into playlist och back to playlists or nothing
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            // FIXA SÅ ATT VI LADDAR MUSIKEN TILL PLAYLISTVIEWENS CURRENT PLAYLIST ----------------------------------------------------------------------------------------------------------------------------------------------------------
            try
            {
                Playlist selected = playlistList.SelectedItem as Playlist;
                if (playlistList.SelectedItem.ToString() == "back")
                {
                    goToPlaylists(playlistList);
                    //curPlaylistBox.Text = "";

                }
                else if (selected != null)  //TODO: check if of type playlist
                {
                    curentPlaylist = playlistList.Items[playlistList.SelectedIndex] as Playlist;
                    LoadCurentPlaylist(playlistList);
                    //curPlaylistBox.Text = curentPlaylist.ToString();


                }
            }
            catch
            {
                Console.WriteLine("please click on a object");
            }

        }
        // when user right click in playlistList show options
        public void RightClickBox()
        {
            //rightClick.Visibility = Visibility.Visible;

            //Point p = Mouse.GetPosition(Application.Current.MainWindow);
            //rightClick.Margin = new Thickness(p.X, p.Y, 0, 0);
            //rightClick.Items.Clear();
            //rightClick.Items.Add("X");
            //rightClick.Items.Add("remove");
            //rightClick.Items.Add("add song to...");

        }

        private void RightClickBoxHandler(object sender, SelectionChangedEventArgs e)
        {
            //rightClick.Visibility = Visibility.Hidden;


        }

        private void ListBox_mouseLeft(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("mouse down");

            //rightClick.Visibility = Visibility.Hidden;

        }

        private void ListBox_mouseRight(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("mouse down");

            RightClickBox();

        }

        private void newPlaylist(object sender, RoutedEventArgs e)
        {

            string path = getPlaylistsPath();

            Console.WriteLine(path + "\\" + playlistName.Text + ".txt");
            System.IO.File.WriteAllText(@path + "\\" + playlistName.Text + ".txt", "[]");
        }
        // DEN HÄR SKA LIGGA I PLAYLIST VYN
        //private void searchBPMButton(object sender, RoutedEventArgs e)
        //{

        //    int n;
        //    bool isNumeric = int.TryParse(searchFieldBPM.Text, out n);
        //    if (isNumeric && curentPlaylist != null)
        //    {
        //        //curentPlaylist.loadBPM();

        //        showMusic(curentPlaylist.searchBPM(n), playlistList);


        //    }
        //}

        //private void searchTitlesButton(object sender, RoutedEventArgs e)
        //{
        //    if (curentPlaylist != null)
        //    {
        //        showMusic(curentPlaylist.searchTitles(searchFieldTitle.Text), playlistList);
        //    }

        //}

        private void mediaOpen(object sender, RoutedEventArgs e)
        {
            mediaFileIsOpen = true;
            TimeSpan ts = player.NaturalDuration.TimeSpan;
            trackSlider.Maximum = ts.TotalSeconds;
            var openDuration = player.NaturalDuration.TimeSpan;
            var theDuration = new TimeSpan(0, openDuration.Minutes, openDuration.Seconds);
            durationLabel.Content = theDuration.ToString().Substring(3);


        }

        private void closeUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void closeEnter(object sender, MouseEventArgs e)
        {
            closeButton.Source = new BitmapImage(new Uri("../img/close-hover.png", UriKind.Relative));
        }

        private void closeLeave(object sender, MouseEventArgs e)
        {
            closeButton.Source = new BitmapImage(new Uri("../img/close.png.png", UriKind.Relative));
        }


        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            settingsCounter++;
            if (settingsCounter % 2 == 1)
            {
                DynamicView.Content = settingsPage;
            }

            else
            {
                DynamicView.Content = playlistPage;
            }
        }

        private void closeDown(object sender, MouseButtonEventArgs e)
        {
            closeButton.Source = new BitmapImage(new Uri("../img/close-clicked.png", UriKind.Relative));
        }

        private void maximizeUp(object sender, MouseButtonEventArgs e)
        {
            if (!isMaximized)
            {
                WindowState = WindowState.Maximized;
                isMaximized = true;
            }
            else
            {
                WindowState = WindowState.Normal;
                isMaximized = false;
            }
        }

        private void minimizeUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maximizeDown(object sender, MouseButtonEventArgs e)
        {
            maximizeButton.Opacity = 0.25;
        }

        private void maximizeEnter(object sender, MouseEventArgs e)
        {
            maximizeButton.Opacity = 1;
        }
        private void maximizeLeave(object sender, MouseEventArgs e)
        {
            maximizeButton.Opacity = 0.5;
        }

        private void minimizeEnter(object sender, MouseEventArgs e)
        {
            minimizeButton.Opacity = 1;
        }

        private void minimizeDown(object sender, MouseButtonEventArgs e)
        {
            minimizeButton.Opacity = 0.25;
        }

        private void minimizeLeave(object sender, MouseEventArgs e)
        {
            minimizeButton.Opacity = 0.5;
        }

        private void playSettingEnter(object sender, MouseEventArgs e)
        {
            playSettingButton.Opacity = 1;
        }

        private void playSettingLeave(object sender, MouseEventArgs e)
        {
            playSettingButton.Opacity = 0.5;
        }

        private void volumeFadeIn(object sender, RoutedEventArgs e)
        {
            // TODO --------------------------------------------------------------------------------------
            
        }

        private void volumeFadeOut(object sender, RoutedEventArgs e)
        {
            // TODO --------------------------------------------------------------------------------------
        }
    }
}