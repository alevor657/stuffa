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



namespace Stuffa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool isPlaying = false;
        static Playlist curentPlaylist;
        //Music[] songs = new Music[3];
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.


        //search the given list using ToString
        public static void searchList(ListBox list, int startPos, string searchString)
        {
            for(int i = startPos; i < list.Items.Count; ++i)
            {
                if(!list.Items[i].ToString().Contains(searchString))
                {
                    list.Items.RemoveAt(i);
                    --i;
                }
            }
        }

        //loads a playlist given path and ListBox
        public static void LoadCurentPlaylist(ListBox list)
        {
            
            curentPlaylist.loadMusic();


            List<Music> toAdd = curentPlaylist.getMusic();

            list.Items.Clear();
            list.Items.Add("back");

            foreach(Music i in toAdd)
            {
                list.Items.Add(i);
            }



        }

        //get the path to the playlists, this is lokated in the folder "Musik"
        public static string getPlaylistsPath()
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;

            int pos = path.LastIndexOf('\\');
            path = path.Substring(0, pos);

            pos = path.LastIndexOf('\\');
            path = path.Substring(0, pos);

            pos = path.LastIndexOf('\\');
            path = path.Substring(0, pos);

            pos = path.LastIndexOf('\\');
            path = path.Substring(0, pos);

            path += "\\Musik";
            return path;
        }

        //go to all playlists
        public static void goToPlaylists(ListBox list)
        {

            ProcessDirectoryJson(getPlaylistsPath(), list);
        }

        //get all files that is of type "fileTypes" in the directory "TargetDirectory"
        public static List<String> ProcessDirectory(string targetDirectory, ListBox list, string[] fileTypes, int cap)
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
                    for(int i = 0; i < cap; i++)
                    {
                        if(fileName.EndsWith(fileTypes[i]))
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

        //get JSON files (txt) and show them on list
        public static void ProcessDirectoryJson(string targetDirectory, ListBox list)
        {
            string[] fileTypes = new string[1];
            fileTypes[0] = ".txt";

            list.Items.Clear();


            ProcessPlaylist(ProcessDirectory(targetDirectory, list, fileTypes, 1), list);
        }

        // get .mp3 and .m4a files and show them on list
        public static void ProcessDirectoryMusic(string targetDirectory, ListBox list)
        {
            list.Items.Clear();

            string[] fileTypes = new string[2];
            fileTypes[0] = ".mp3";
            fileTypes[1] = ".m4a";
            //this line first takes all the matching files in the funktion ProcessDirectory
            //then it adds the musik to the list in processMusic
            processMusic(ProcessDirectory(targetDirectory, list, fileTypes, 2), list);

        }

        //inset List/paths into list
        public static void ProcessPlaylist(List<string> paths, ListBox list)
        {
            
            
            for (int i = 0; i < paths.Count; ++i)
            {
                list.Items.Add(new Playlist(paths[i]));
            }
        }

        //insert List/paths into list
        public static void processMusic(List<string> paths, ListBox list)
        {
            for(int i = 0; i < paths.Capacity; ++i)
            {
                list.Items.Add(new Music(paths[i]));
            }
          
        }

        //pause the player
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

        public void pausePlayServer()
        {
            this.Dispatcher.Invoke(() => { pausePlay(); });
        }

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




        public MainWindow()
        {
            Thread.CurrentThread.Name = "parent";
            InitializeComponent();


            goToPlaylists(list);

            progresBar.Value = 0.5;
            Thread serverThread = new Thread(startServer);
            serverThread.IsBackground = true;
            serverThread.Start();




        }


      

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                
                string name = list.SelectedItem.ToString();
                Console.WriteLine("file: " + name);

                if (false) //TODO: if it is a playlist 
                {
                   
                }
                else
                {
                    try
                    {
                        Music music = list.SelectedItem as Music;
                        //get path to file
                        string path = music.getFullPath();
                        
                        TagLib.File tagFile = TagLib.File.Create(path);
                        string songName = tagFile.Tag.Title;
                        var length = tagFile.Properties.Duration;


                        //get BPM
                        // instantiate the Application object

                        dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));

                        // get the folder and the child
                        var folder = shell.NameSpace(System.IO.Path.GetDirectoryName(path));
                        var item = folder.ParseName(System.IO.Path.GetFileName(path));

                        // get the item's property by it's canonical name. doc says it's a string
                        string bpm = item.ExtendedProperty("System.Music.BeatsPerMinute");
                        Console.WriteLine(bpm);
                        //get BPM

                        label.Content = length;

                        text.Text = path + "\n\n" + "Title: " + songName + "\nBPM: " + bpm;

                        player.Source = new Uri(path, UriKind.RelativeOrAbsolute);


                        player.Play();
                        isPlaying = true;
                    }
                    catch
                    {
                    }
                }
                // Denna funkar bara efter att man har lagt till  en ny låt - därför utkommenterad
                //textBox.Text = "Path: " + songs[0].path + " Title: " + songs[0].name;
            }


        }



        public void Button_Click(object sender, RoutedEventArgs e)
        {

            pausePlay();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void open_song_click(object sender, RoutedEventArgs e)
        {
            //TODO: redirect to Playlist
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".mp3";
            dlg.Filter = "MP3 Files (*.mp3)|*.mp3|M4A Files (*.m4a)|*.m4a|FLAC Files (*.flac)|*.flac";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {

                // Open document 
                string filename = dlg.FileName;
                list.Items.Add(new Music(filename));
                textBox1.Text = filename;

                //songs[0] = new Music(filename);

                //TODO: 

            }
        }

        private void progresBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            /*// TODO: make automatic
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

        private void load_Click(object sender, RoutedEventArgs e)
        {
            //List<string> musicTracks = new List<string>();
            if (list.SelectedItem.ToString() != null)
            {
                curentPlaylist = list.SelectedItem as Playlist;
                LoadCurentPlaylist(list);
                curPlaylistBox.Text = curentPlaylist.ToString();
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

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Playlist selected = list.SelectedItem as Playlist;
                if (list.SelectedItem.ToString() == "back")
                {
                    goToPlaylists(list);
                    curPlaylistBox.Text = "";

                }
                else if (selected != null)  //TODO: check if of type playlist
                {
                    curentPlaylist = list.Items[list.SelectedIndex] as Playlist;
                    LoadCurentPlaylist(list);
                    curPlaylistBox.Text = curentPlaylist.ToString();


                }
            }
            catch {
                Console.WriteLine("please click on a object");
            }

        }

        public void RightClickBox()
        {
            rightClick.Visibility = Visibility.Visible;

            Point p = Mouse.GetPosition(Application.Current.MainWindow);
            rightClick.Margin = new Thickness(p.X, p.Y, 0, 0);
            rightClick.Items.Clear();
            rightClick.Items.Add("X");
            rightClick.Items.Add("remove");
            rightClick.Items.Add("add song to...");

        }

        private void RightClickBoxHandler(object sender, SelectionChangedEventArgs e)
        {
            rightClick.Visibility = Visibility.Hidden;

           
        }

        private void ListBox_mouseLeft(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("mouse down");

            rightClick.Visibility = Visibility.Hidden;

        }

        private void ListBox_mouseRight(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("mouse down");
           
            RightClickBox();
            
        }

        private void newPlaylist(object sender, RoutedEventArgs e)
        {
           
            string path = getPlaylistsPath();

            Console.WriteLine(path +"\\" + playlistName.Text + ".txt");
            System.IO.File.WriteAllText(@path + "\\" +  playlistName.Text + ".txt", "[]");
        }

        private void searchButton(object sender, RoutedEventArgs e)
        {
            searchList(list, 1, searchField.Text);
        }
    }
}
