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
using WpfApp2.pages;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Windows.Threading;
using SocketServer;

namespace Stuffa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //PlaylistView playlistPage = new PlaylistView();
        Settings settingsPage = new Settings();

        //deaclear variables
        int settingsCounter = 0;
        bool isMaximized = false;
        static bool isPlaying = false;
        static bool mediaFileIsOpen = false;
        static Playlist curentPlaylist;

        DispatcherTimer songTimer = new DispatcherTimer();


       
        //main function
        public MainWindow()
        {
            // give this thread a name 
            Thread.CurrentThread.Name = "parent";
            InitializeComponent();
            //PlaylistView playlistPage = new PlaylistView();
            Settings settingsPage = new Settings();
            ContainerView.Content = new Container();

            //DynamicView.Content = playlistPage;

            // show all playlists
            // goToPlaylists(playlistList);

            //start a temporary server until better is developed TODO: Update/remove
            //Thread serverThread = new Thread(startServer);
            //serverThread.IsBackground = true;
            //serverThread.Start();

            //songTimer.Interval = new TimeSpan(500);
            //songTimer.Tick += TimerTicker;

            // Init server
            Server.Init();
        }

      
        private void closeUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void closeEnter(object sender, MouseEventArgs e)
        {
            closeButton.Source = new BitmapImage(new Uri("/img/close-hover.png", UriKind.Relative));
        }

        private void closeLeave(object sender, MouseEventArgs e)
        {
            closeButton.Source = new BitmapImage(new Uri("/img/close.png.png", UriKind.Relative));
        }

        private void closeDown(object sender, MouseButtonEventArgs e)
        {
            closeButton.Source = new BitmapImage(new Uri("/img/close-clicked.png", UriKind.Relative));
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

        //    private void SettingsClick(object sender, RoutedEventArgs e)
        //    {
        //        settingsCounter++;
        //        if (settingsCounter % 2 == 1)
        //        {
        //            DynamicView.Content = settingsPage;
        //        }

        //        else
        //        {
        //            DynamicView.Content = playlistPage;
        //        }
        //    }



        //    private void playSettingEnter(object sender, MouseEventArgs e)
        //    {
        //        playSettingButton.Opacity = 1;
        //    }

        //    private void playSettingLeave(object sender, MouseEventArgs e)
        //    {
        //        playSettingButton.Opacity = 0.5;
        //    }

        //    private void volumeFadeIn(object sender, RoutedEventArgs e)
        //    {
        //        // TODO --------------------------------------------------------------------------------------

        //    }

        //    private void volumeFadeOut(object sender, RoutedEventArgs e)
        //    {
        //        // TODO --------------------------------------------------------------------------------------
        //    }
    }
}