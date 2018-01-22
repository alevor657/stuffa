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



namespace Stuffa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isPlaying = false;
        Music[] songs = new Music[3];
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory, ListBox list)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName, list);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, list);
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path, ListBox list)
        {
            //gets the file name
            list.Items.Add(path);
           // Console.WriteLine("Processed file '{0}'.", path);
        }

        public MainWindow()
        {
            InitializeComponent();

            ProcessDirectory("C:\\Users\\Fredrik\\source\\repos\\stuffa\\Musik\\", list);

            progresBar.Value = 0.5;
         
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = list.SelectedItem.ToString();
            // Denna funkar bara efter att man har lagt till  en ny låt - därför utkommenterad
            //textBox.Text = "Path: " + songs[0].path + " Title: " + songs[0].name;
            TagLib.File tagFile = TagLib.File.Create(path);
            string artist = tagFile.Tag.JoinedPerformers;
            string album = tagFile.Tag.JoinedAlbumArtists;
            string songName = tagFile.Tag.Title;
            var length = tagFile.Properties.Duration;

            label.Content = length;

            text.Text = path + "\n\nmusik grupp: " + artist + " Album: " + album + " Title: " + songName;

            player.Source = new Uri(path, UriKind.RelativeOrAbsolute);


            player.Play();
            isPlaying = true;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            if(isPlaying)
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
            dlg.Filter = "MP3 Files (*.mp3)|*.mp3| FLAC Files (*.flac)|*.flac";

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
    }
}
