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



namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isPlaying = false;

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
            list.Items.Add(System.IO.Path.GetFileNameWithoutExtension(path));
            // Console.WriteLine("Processed file '{0}'.", path);
        }

        public MainWindow()
        {
            InitializeComponent();

            ProcessDirectory("E:\\grupp.proj\\YTExampleMusik", list);


        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string path = list.SelectedItem.ToString();
            path = "E:\\grupp.proj\\YTExampleMusik\\" + path + ".mp3";

            TagLib.File tagFile = TagLib.File.Create(path);
            string artist = tagFile.Tag.JoinedPerformers;

            text.Text = path + "\n\nmusik grupp: " + artist;

            player.Source = new Uri(path, UriKind.RelativeOrAbsolute);


            player.Play();
            isPlaying = true;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
    }
}
