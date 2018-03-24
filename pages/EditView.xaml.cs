﻿using Stuffa;
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
    public partial class EditView : Page
    {

        Container container;

        //public List<Music> currentMusic;
        public EditView(Container container)
        {
            this.container = container;
            InitializeComponent();

            // Bara för testing
            //List<Songs> songsInPlaylist = new List<Songs>();
            //songsInPlaylist.Add(new Songs() { Bpm = 132, Title = "Way Back", Artist = "Vicetone" });
            //songsInPlaylist.Add(new Songs() { Bpm = 110, Title = "Fire", Artist = "Burning" });


            //currentPlaylist.ItemsSource = songsInPlaylist;

        }
        public void LoadPlaylist(List<Music> playlistSongs)
        {
            currentPlaylist.ItemsSource = null; ;

            currentPlaylist.ItemsSource = playlistSongs;

            //currentPlaylist.ItemsSource = musicInPlaylist;
        }

        private void currentPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentPlaylist.SelectedIndex != -1)
            {
                container.PlaySelectedSong();
            }            
        }


        private void DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                /*foreach(string s in e.Data.GetFormats())
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine("------");*/

                List<string> paths = ((string[])e.Data.GetData(DataFormats.FileDrop, false)).ToList<string>();

                for (int i = 0; i < paths.Count; i++)
                {
                    if(!paths[i].EndsWith(".mp3") && !paths[i].EndsWith(".m4a"))
                    {
                        Console.WriteLine(paths[i] + " <--wrong filetype");

                        paths.RemoveAt(i);
                    }
                    else
                    {
                        Console.WriteLine(paths[i]);

                    }
                }

                container.LoadNewMusic(paths);
                
            }
            catch { }
        }


        private void removeSongOnIndex(object sender, RoutedEventArgs e)
        {
            container.removeMusic(currentPlaylist.SelectedIndex);

        }
    }
}

public class Songs
{
    public int Bpm { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Path { get; set; }
}



