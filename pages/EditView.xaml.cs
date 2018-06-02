using Stuffa;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        Music toEdit;
        Container container;
        bool dragFromSearchList;


        //public List<Music> currentMusic;
        public EditView(Container container)
        {
            this.container = container;
            InitializeComponent();
            playlists.Visibility = Visibility.Hidden;


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
            if(index == 0)
            {
                //no music in pl instruct user on how to add to playllist
                DragHere.Content = new DragHereInstruction(testtest, AddMusic);
            }
            else
            {
                DragHere.Content = null;
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
            
            SnackBar.Content = new MsgWin("Some music already exist in playlist", "ADD", "SKIP", leftMsbWinButton, rightMsbWinButton);
            
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

        public void snackBarActivateDatabase(string message)
        {
            var messageQueue = SnackBarDialogdatabase.MessageQueue;
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

        /*private void DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(ListBox))
                {

                }
                else
                {
                    //foreach(string s in e.Data.GetFormats())
                    //{
                    //    Console.WriteLine(s);
                    //}
                    //Console.WriteLine("------");

                    List<string> paths = ((string[])e.Data.GetData(DataFormats.FileDrop, false)).ToList<string>();

                    for (int i = 0; i < paths.Count; i++)
                    {
                        CultureInfo ci;
                        ci = new CultureInfo("en-US");
                        if (!paths[i].EndsWith(".mp3", true, ci) && !paths[i].EndsWith(".m4a", true, ci) && !paths[i].EndsWith(".flac", true, ci))
                        {
                            if (paths[i].LastIndexOf('.') > paths[i].LastIndexOf('\\')) //files have . after the last \
                            {
                                Console.WriteLine(paths[i] + " <--wrong filetype");

                                paths.RemoveAt(i);
                                i--;
                            }
                            else //directory
                            {
                                List<string> files = new List<string>();
                                // Process the list of files found in the directory.
                                try
                                {
                                    //get all files in directory
                                    string[] fileEntries = Directory.GetFiles(paths[i]);

                                    paths.AddRange(fileEntries);


                                }
                                catch { }
                                paths.RemoveAt(i);
                                i--;


                            }
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
        }*/

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

        internal void SnackBarErr(string message)
        {
            var messageQueue = SnackBarDialogErr.MessageQueue;

            //the message queue can be called from any thread
            Task.Factory.StartNew(() => messageQueue.Enqueue(message, "OKAY", () => { }));

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

        private void search(object sender = null, KeyEventArgs e = null)
        {
            bool show = true;
            if (SearchTermTextBox.Text.StartsWith("pl:"))
            {

                string plName = SearchTermTextBox.Text;
                List<string> pl = container.GetPlaylists();


                for (int i = 0; i < pl.Count(); i++)
                {
                    if(plName.Contains(pl[i]) && plName.Length >= 3 + pl[i].Length + 1)
                    {
                        container.SearchPlaylist(i, plName.Substring(3 + pl[i].Length +1 /*mellanslag*/));
                        i = plName.Count();
                        playlists.Visibility = Visibility.Hidden;

                        show = false;
                    }

                }

                if(show)
                {
                    if(playlists.ItemsSource == null)
                    {
                        playlists.ItemsSource = container.GetPlaylists();
                    }
                    playlists.Visibility = Visibility.Visible;
                }

            }
            else
            {
                playlists.Visibility = Visibility.Hidden;
            }
            if(show)
            {
                container.searchAllMusic(SearchTermTextBox.Text);
            }
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
            bool wrongFileType = false;
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

                        CultureInfo ci;
                        ci = new CultureInfo("en-US");
                        if (!paths[i].EndsWith(".mp3", true, ci)  && !paths[i].EndsWith(".m4a", true, ci) && !paths[i].EndsWith(".wav", true, ci) && !paths[i].EndsWith(".flac", true, ci))
                        {
                            if (paths[i].LastIndexOf('.') > paths[i].LastIndexOf('\\')) //files have . after the last \
                            {
                                Console.WriteLine(paths[i] + " <--wrong filetype");
                                wrongFileType = true;
                                paths.RemoveAt(i);
                                i--;
                            }
                            else //directory
                            {
                                List<string> files = new List<string>();
                                // Process the list of files found in the directory.
                                try
                                {
                                    //get all files in directory
                                    string[] fileEntries = Directory.GetFiles(paths[i]);

                                    paths.AddRange(fileEntries);


                                }
                                catch { }
                                paths.RemoveAt(i);
                                i--;


                            }
                        }
                        else
                        {
                            Console.WriteLine(paths[i]);

                        }
                    }

                    if(container.LoadNewMusic(paths))
                    {
                        currentPlaylist.ScrollIntoView(currentPlaylist.Items.GetItemAt(currentPlaylist.Items.Count - 1));
                        this.snackBarActivate("Music added");
                        currentPlaylist.SelectedIndex = currentPlaylist.Items.Count - 1;
                    }
                }

                if(wrongFileType)
                {
                    SnackBarErr("only folders, mp3, flac, m4a or .wav files");
                }

            }
            catch
            {
            }
        }

        private void ImportToDatabase(object sender, DragEventArgs e)
        {
            try
            {
                List<string> paths = ((string[])e.Data.GetData(DataFormats.FileDrop, false)).ToList<string>();


                for (int i = 0; i < paths.Count; i++)
                {
                    CultureInfo ci;
                    ci = new CultureInfo("en-US");
                    if (!paths[i].EndsWith(".mp3", true, ci) && !paths[i].EndsWith(".m4a", true, ci) && !paths[i].EndsWith(".flac", true, ci))
                    {
                        if (paths[i].LastIndexOf('.') > paths[i].LastIndexOf('\\')) //files have . after the last \
                        {
                            Console.WriteLine(paths[i] + " <--wrong filetype");

                            paths.RemoveAt(i);
                            i--;
                        }
                        else //directory
                        {
                            List<string> files = new List<string>();
                            // Process the list of files found in the directory.
                            try
                            {
                                //get all files in directory
                                string[] fileEntries = Directory.GetFiles(paths[i]);

                                paths.AddRange(fileEntries);


                            }
                            catch { }
                            paths.RemoveAt(i);
                            i--;


                        }
                    }
                    else
                    {
                        Console.WriteLine(paths[i]);

                    }
                }

                container.LoadNewMusicDatabase(paths);
                this.snackBarActivateDatabase("Music added");
            }
            catch
            {
                Console.WriteLine("could not import music to database");
            }
        }

        private void searchRes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void dragFromSearch(object sender, MouseButtonEventArgs e)
        {
            this.dragFromSearchList = true;
            this.currentPlaylist.AllowDrop = true;

        }

        private void dragFromCur(object sender, MouseButtonEventArgs e)
        {
            this.dragFromSearchList = false;

        }

        private void AddMusicLibrary(object sender, RoutedEventArgs e)
        {
            container.LoadNewMusicDatabse();

        }

        private void removeSongLibrary(object sender, RoutedEventArgs e)
        {
            container.RemoveMusicLibrary(searchRes.SelectedItem as Music);
            search();
            //snackBarActivate("To be implemented...");
        }

        private void editMusic(object sender, RoutedEventArgs e)
        {
            editMusicWin((this.currentPlaylist.SelectedItem as Tuple<Music, Visibility, int>).Item1);
            //container.edit((currentPlaylist.Items.GetItemAt(currentPlaylist.SelectedIndex) as Tuple<Music, Visibility, int>).Item1);
        }
        public void editMusicWin(Music m)
        {
            if (m != null)
            {
                this.BpmBox.Text = m.getBpm().ToString();
                this.TitleBox.Text = m.getTitle();
                this.ArtistBox.Text = m.getArtist();
                toEdit = m;
            }
        }

        public void editMusicWinDb(Music m)
        {
            if (m != null)
            {
                this.BpmBox2.Text = m.getBpm().ToString();
                this.TitleBox2.Text = m.getTitle();
                this.ArtistBox2.Text = m.getArtist();
                toEdit = m;
            }
        }



        private void EditMusicCurPlaylist_OnDialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true)) return;
            this.container.EditMusic(toEdit, BpmBox.Text, TitleBox.Text, ArtistBox.Text);
            this.container.showSelectedPlaylist();

        }

        private void DragHere_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void DragOverShow(object sender, DragEventArgs e)
        {

            bool show = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == false)
            {
                if (!dragFromSearchList)
                {
                    show = false;
                    this.currentPlaylist.AllowDrop = false;
                }
            }
            if (show)
            {
                DragHere.Content = null;
                DragHere.Content = new DragHere(testtest);
            }

        }

        private void dragFromCur(object sender, MouseEventArgs e)
        {
            this.dragFromSearchList = false;

        }

        private void DragOverShowMaster(object sender, DragEventArgs e)
        {
            object paths = ((string[])e.Data.GetData(DataFormats.FileDrop, false));
            if (paths != null)
            {
                this.DragHereMaster.Content = new DragHere(ImportToDatabase);

            }
            else
            {
                this.searchRes.AllowDrop = false;
            }

        }

        private void AllowDropSearch(object sender, MouseEventArgs e)
        {
            this.dragFromSearchList = true;
            this.searchRes.AllowDrop = true;
        }

        private void editMusicDatabase(object sender, RoutedEventArgs e)
        {
            editMusicWinDb(this.searchRes.SelectedItem as Music);

        }

        private void EditMusicDatabase_OnDialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            //if canse return
            if (!Equals(eventArgs.Parameter, true)) return;

            this.container.EditMusic(toEdit, BpmBox2.Text, TitleBox2.Text, ArtistBox2.Text);
            container.searchAllMusic(SearchTermTextBox.Text);

        }





        private void CurrentPlaylist_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            if (currentPlaylist.SelectedIndex != -1)
            {
                container.PlaySelectedSong();
            }
        }

        private void PlayDatabaseMusic(object sender, MouseButtonEventArgs e)
        {
            //play selected music in Database/search
            container.PlaySelectedSongDb();
        }

        private void AddDatabaseMusic(object sender, MouseButtonEventArgs e)
        {
            List<string> paths = new List<string>
            {
                (this.searchRes.SelectedItem as Music).getFullPath()
            };
            if(container.LoadNewMusic(paths))
            {
                snackBarActivate("Music added");
            }
        }



        

        int isGridInCurPresed = 0;
        private void CurMenuFocus(object sender, MouseButtonEventArgs e)
        {
            //if a grid in CurrentPlaylist is clicked this will first be called
            isGridInCurPresed = 1;
        }

        private void CurMenuFocusList(object sender, MouseButtonEventArgs e)
        {
            // when the currentPlaylist is clicked this will ve caled after the "CurMenuFocus" method. And secure that if the user presses outside any grid it is a valid plase to pres
            isGridInCurPresed++;

        }

        private void ContextMenuOpen(object sender, RoutedEventArgs e)
        {
            if(isGridInCurPresed == 2)
            {
                EditCur.IsEnabled = true;
                RemoveCur.IsEnabled = true;
            }
            else
            {
                EditCur.IsEnabled = false;
                RemoveCur.IsEnabled = false ;

            }
        }




        int isGridInSearchPresed = 0;
        private void SearchMenuFocus(object sender, MouseButtonEventArgs e)
        {
            //if a grid in CurrentPlaylist is clicked this will first be called
            isGridInSearchPresed = 1;
        }

        private void SearchMenuFocusList(object sender, MouseButtonEventArgs e)
        {
            // when the currentPlaylist is clicked this will ve caled after the "CurMenuFocus" method. And secure that if the user presses outside any grid it is a valid plase to pres
            isGridInSearchPresed++;

        }

        private void ContextMenuOpenSearch(object sender, RoutedEventArgs e)
        {
            if (isGridInSearchPresed == 2)
            {
                EditSearch.IsEnabled = true;
                RemoveSearch.IsEnabled = true;
            }
            else
            {
                EditSearch.IsEnabled = false;
                RemoveSearch.IsEnabled = false;

            }
        }

        private void BPMLabelUp(object sender, MouseButtonEventArgs e)
        {
            container.SortOnChoice(0);
        }

        private void TitleLabelUp(object sender, MouseButtonEventArgs e)
        {
            container.SortOnChoice(2);
        }

        private void ArtistLabelUp(object sender, MouseButtonEventArgs e)
        {
            container.SortOnChoice(1);
        }


        private void AddPl(object sender, RoutedEventArgs e)
        {
            this.SearchTermTextBox.Text = "pl:";
            Console.WriteLine("click!");
            this.playlists.ItemsSource = container.GetPlaylists();
            playlists.Visibility = Visibility.Visible;

        }

        private void PlaylistSearchSelected(object sender, MouseButtonEventArgs e)
        {
            SearchTermTextBox.Text = "pl:" + (sender as ListBox).SelectedItem.ToString() + " ";
            search();
            playlists.Visibility = Visibility.Hidden;
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



