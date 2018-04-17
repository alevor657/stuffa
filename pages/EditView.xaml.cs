﻿using Stuffa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using WpfApp2.feedback;

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
           
            //SnackBar.Content = new MsgWin("dublets spoted", "add", "skipp", leftMsbWinButton, rightMsbWinButton);


            // Bara för testing
            //List<Songs> songsInPlaylist = new List<Songs>();
            //songsInPlaylist.Add(new Songs() { Bpm = 132, Title = "Way Back", Artist = "Vicetone" });
            //songsInPlaylist.Add(new Songs() { Bpm = 110, Title = "Fire", Artist = "Burning" });


            //currentPlaylist.ItemsSource = songsInPlaylist;

        }
        public void LoadPlaylist(List<Music> playlistSongs)
        {
            List<Tuple<Music, Visibility>> list = new List<Tuple<Music, Visibility>>();
            foreach (Music i in playlistSongs)
            {
                list.Add(new Tuple<Music, Visibility>(i, Visibility.Collapsed));
            }
            currentPlaylist.ItemsSource = null;
            currentPlaylist.ItemsSource = list;
            this.SnackBar.Content = null;
            //currentPlaylist.ItemsSource = musicInPlaylist;
        }

        public void snackBarActivate()
        {
            SnackBar.Content = null;
            
            SnackBar.Content = new MsgWin("dublets spoted", "add", "skipp", leftMsbWinButton, rightMsbWinButton);
            
            //pop.IsEnabled = true;
            //var messageQueue = SnackBarDialog.MessageQueue;

            //the message queue can be called from any thread
            //Task.Factory.StartNew(() => messageQueue.Enqueue("some music allready existed in this playlist", "continue adding", () => { }));
        }

        public void snackBarActivate(string message)
        {
            var messageQueue = SnackBarDialog.MessageQueue;

            //the message queue can be called from any thread
            Task.Factory.StartNew(() => messageQueue.Enqueue(message, "OKAY", () => { }));
        }

        public void LoadSearch(List<Music> playlistSongs)
        {

            searchRes.ItemsSource = null;
            searchRes.ItemsSource = playlistSongs;

        }

        public void setMarked(List<Music> music, List<int> marked)
        {
            List<Tuple<Music, Visibility>> list = new List<Tuple<Music, Visibility>>();
            
            for(int i = 0; i < music.Count; i++)
            {
                list.Add(new Tuple<Music, Visibility>(music[i], marked.Contains(i) ? Visibility.Visible : Visibility.Collapsed));
            }
            this.currentPlaylist.ItemsSource = null;
            this.currentPlaylist.ItemsSource = list;

        }

        private void CurrentPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentPlaylist.SelectedIndex != -1)
            {
                container.PlaySelectedSong();
            }            
        }

        public bool IsSearchBarActive()
        {
            return SearchTermTextBox.IsSelectionActive;
        }


        private void DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(ListBox))
                {
                    Music m = ((e.Source as ListBox).SelectedItem as Tuple<Music, System.Windows.Visibility>).Item1;
                    Console.WriteLine(e.GetPosition(e.Source as ListBox));
                    
                    Console.WriteLine(m.getTitle() + " : " + m.getArtist());
                }
                else
                {
                    /*foreach(string s in e.Data.GetFormats())
                    {
                        Console.WriteLine(s);
                    }
                    Console.WriteLine("------");*/

                    List<string> paths = ((string[])e.Data.GetData(DataFormats.FileDrop, false)).ToList<string>();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        if (!paths[i].EndsWith(".mp3") && !paths[i].EndsWith(".m4a"))
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
                
            }
            catch { }
        }

        public void leftMsbWinButton(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("left");
            container.AddDupletts();
            SnackBar.Content = null;

            //pop.IsOpen = false;

        }

        public void rightMsbWinButton(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("right");
            SnackBar.Content = null;
            //pop.IsOpen = false;
        }

        private void removeSongOnIndex(object sender, RoutedEventArgs e)
        {
            container.removeMusic(currentPlaylist.SelectedIndex);

        }

        private void search(object sender, KeyEventArgs e)
        {

            container.searchAllMusic(SearchTermTextBox.Text);
        }

        private void AddMusic(object sender, RoutedEventArgs e)
        {
            container.LoadNewMusic();
        }
        public void setHighlight(int selectedIndex)
        {
            this.currentPlaylist.SelectedIndex = selectedIndex;
            this.currentPlaylist.ScrollIntoView(this.currentPlaylist.SelectedItem);
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



