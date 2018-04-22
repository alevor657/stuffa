﻿using Stuffa;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        bool dragFromSearchList;


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
            List<Tuple<Music, Visibility, int>> list = new List<Tuple<Music, Visibility, int>>();
            int index = 0;
            foreach (Music i in playlistSongs)
            {
                list.Add(new Tuple<Music, Visibility, int>(i, Visibility.Collapsed, index));
                index++;
            }


            /*
            IList SourceList;
            Array a;
            currentPlaylist.Items.CopyTo(a, 0);
            if(SourceList != null)
            {
                SourceList.Clear();
            }*/


            

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
            List<Tuple<Music, Visibility, int>> list = new List<Tuple<Music, Visibility, int>>();

            for(int i = 0; i < music.Count; i++)
            {
                list.Add(new Tuple<Music, Visibility, int>(music[i], marked.Contains(i) ? Visibility.Visible : Visibility.Collapsed, i));
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

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private void DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(ListBox))
                {

                    /*
                    Music m = (this.currentPlaylist.SelectedItem as Tuple<Music, System.Windows.Visibility, int>).Item1;


                    Console.WriteLine("1 " + m.getTitle() + " : " + m.getArtist());


                    ListBox lb = (sender as ListBox);
                    //Call the imported function with the cursor's current position
                    uint X = (uint)e.GetPosition(lb).X;
                    uint Y = (uint)e.GetPosition(lb).Y;
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                    

                    m = (this.currentPlaylist.SelectedItem as Tuple<Music, System.Windows.Visibility, int>).Item1;

                    
                    Console.WriteLine("2 " + m.getTitle() + " : " + m.getArtist());*/
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

            currentPlaylist.ScrollIntoView(currentPlaylist.Items.GetItemAt(currentPlaylist.Items.Count - 1));
            this.snackBarActivate("Music added");
            currentPlaylist.SelectedIndex = currentPlaylist.Items.Count - 1;

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

        private void testtest(object sender, DragEventArgs e)
        {
            try
            {

                if (e.Data.GetDataPresent(DataFormats.FileDrop) == false)
                {
                    if (!dragFromSearchList)
                    {
                        //Console.WriteLine( "hejå" + ((sender as Grid).DataContext as Tuple<Music, System.Windows.Visibility, int>).Item3);
                        int from = this.currentPlaylist.SelectedIndex;
                        int to = ((sender as Grid).DataContext as Tuple<Music, System.Windows.Visibility, int>).Item3;



                        currentPlaylist.Items.Refresh();

                        //Object o = currentPlaylist.Items.GetItemAt(currentPlaylist.Items.Count-1);
                        //currentPlaylist.Items.Insert(to, o);
                        //currentPlaylist.Items.Refresh();

                        currentPlaylist.SelectedIndex = -1;
                        currentPlaylist.ItemsSource = null;
                        currentPlaylist.Items.Refresh();



                        container.MoveMusic(from, to);
                        currentPlaylist.SelectedIndex = to;


                        //currentPlaylist.Items.Clear();
                        //currentPlaylist.Items.Refresh();

                    }
                    else
                    {
                        Music m = searchRes.SelectedItem as Music;
                        if(container.LoadNewMusic(new List<string> { m.getFullPath() }))
                        {
                            currentPlaylist.ScrollIntoView(currentPlaylist.Items.GetItemAt(currentPlaylist.Items.Count - 1));
                            this.snackBarActivate("Music added");
                            currentPlaylist.SelectedIndex = currentPlaylist.Items.Count - 1;
                        }


                    }


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
            catch
            {

            }
        }

        private void searchRes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void dragFromSearch(object sender, MouseButtonEventArgs e)
        {
            this.dragFromSearchList = true;
        }

        private void dragFromCur(object sender, MouseButtonEventArgs e)
        {
            this.dragFromSearchList = false;

        }

        private void AddMusicLibrary(object sender, RoutedEventArgs e)
        {
            container.LoadNewMusicLibrary();

        }

        private void removeSongLibrary(object sender, RoutedEventArgs e)
        {
            snackBarActivate("To be implemented...");
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



