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



namespace Stuffa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool isPlaying = false;
        Music[] songs = new Music[3];
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.

        public static void loadPlaylist(string path, ListBox list)
        {
            using (StreamReader r = new StreamReader(@path))
            {
                string json = r.ReadToEnd();
                List<string> musicTracks = JsonConvert.DeserializeObject<List<string>>(json);

                list.Items.Clear();
                list.Items.Add("back");
                
                

                foreach (string i in musicTracks.ToArray())
                {
                    list.Items.Add(i);

                }
            }
        }

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
        public static void goToPlaylists(ListBox list)
        {

            ProcessDirectoryJson(getPlaylistsPath(), list);
        }
        public static void ProcessDirectory(string targetDirectory, ListBox list, string[] fileTypes, int cap)
        {
            // Process the list of files found in the directory.
            try
            {
                string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                {
                    for(int i = 0; i < cap; i++)
                    {
                        if(fileName.EndsWith(fileTypes[i]))
                        {
                            ProcessFile(fileName, list);

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
        }


        public static void ProcessDirectoryJson(string targetDirectory, ListBox list)
        {
            string[] fileTypes = new string[1];
            fileTypes[0] = ".txt";

            list.Items.Clear();

            ProcessDirectory(targetDirectory, list, fileTypes, 1);
        }

        public static void ProcessDirectoryMusic(string targetDirectory, ListBox list)
        {
            list.Items.Clear();

            string[] fileTypes = new string[2];
            fileTypes[0] = ".mp3";
            fileTypes[1] = ".m4a";

            ProcessDirectory(targetDirectory, list, fileTypes, 2);

        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path, ListBox list)
        {
            //gets the file name
            list.Items.Add(path);
            // Console.WriteLine("Processed file '{0}'.", path);
        }

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
                int errNr = 0;
                string path = list.SelectedItem.ToString();
                Console.WriteLine("file: " + path);
                
                if (path.EndsWith(".txt"))
                {
                   
                }
                else
                {
                    try
                    {
                        errNr++;//1
                        TagLib.File tagFile = TagLib.File.Create(path);
                        errNr++;//2
                        string songName = tagFile.Tag.Title;
                        errNr++;//3
                        var length = tagFile.Properties.Duration;
                        errNr++;//4


                        //get BPM
                        // instantiate the Application object

                        dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));
                        errNr++;//5

                        // get the folder and the child
                        var folder = shell.NameSpace(System.IO.Path.GetDirectoryName(path));
                        errNr++;//6
                        var item = folder.ParseName(System.IO.Path.GetFileName(path));
                        errNr++;

                        // get the item's property by it's canonical name. doc says it's a string
                        string bpm = item.ExtendedProperty("System.Music.BeatsPerMinute");
                        errNr++;
                        Console.WriteLine(bpm);
                        errNr++;
                        //get BPM

                        label.Content = length;
                        errNr++;

                        text.Text = path + "\n\n" + "Title: " + songName + "\nBPM: " + bpm;
                        errNr++;

                        player.Source = new Uri(path, UriKind.RelativeOrAbsolute);
                        errNr++;


                        player.Play();
                        errNr++;
                        isPlaying = true;
                        errNr++;
                    }
                    catch
                    {
                        Console.WriteLine(errNr);
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
                list.Items.Add(filename);
                textBox1.Text = filename;

                songs[0] = new Music(filename, System.IO.Path.GetFileNameWithoutExtension(filename));

            }
        }

        private void progresBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            List<string> musicTracks = new List<string>();
            foreach (var i in list.Items)
            {
                musicTracks.Add(i.ToString());
            }
            // package Newtonsoft.Json (Json.Net) need to be installed. 
            // to install go to "project" > "manage NuGet packages..." > "Brows" > type "Newtonsoft.Json" / "Json.Net" > "install"
            string json = JsonConvert.SerializeObject(musicTracks.ToArray());
            System.IO.File.WriteAllText(@"D:\path8ihbjhgnbbv.txt", json);


        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            //List<string> musicTracks = new List<string>();
            if (list.SelectedItem.ToString() != null)
            {
                loadPlaylist(list.SelectedItem.ToString(), list);
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
            if(list.SelectedItem.ToString() == "back")
            {
                goToPlaylists(list);
            }
            else if (list.SelectedItem.ToString().EndsWith(".txt"))
            {
                loadPlaylist(list.SelectedItem.ToString(), list);

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

       
    }
}
