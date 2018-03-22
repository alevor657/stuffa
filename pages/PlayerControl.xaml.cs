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
using System.Windows.Threading;

namespace WpfApp2.pages
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>

    public partial class PlayerControl : Page
    {
        bool mediaFileIsOpen;
        bool isPlaying = false;

        DispatcherTimer slideTimer = new DispatcherTimer();
        DispatcherTimer songTimer = new DispatcherTimer();
        DispatcherTimer labelTimer = new DispatcherTimer();

        int lengthcounter = 0;
        int maxLengthOfTitle = 284;
        int DelayCounter = 0;
        

        public PlayerControl()
        {
            mediaFileIsOpen = true;
            InitializeComponent();
            TitleScrollView.ScrollToVerticalOffset(288);
            

            labelTimer.Interval = new TimeSpan(25000);
            labelTimer.Tick += TimerTickerSlideText;
            labelTimer.Start();

            songTimer.Interval = new TimeSpan(500);
            songTimer.Tick += SongTimerTicker;

            slideTimer.Interval = new TimeSpan(500);
            slideTimer.Tick += SliderTicker;

            //Player.Source = new Uri("D:\\Nedladdningar\\Vicetone - Way Back (feat. Cozi Zuehlsdorff).mp3", UriKind.RelativeOrAbsolute);



        }

        private void SliderTicker(object sender, EventArgs e)
        {
            trackSlider.Value = Player.Position.TotalSeconds;
        }

        private void SongTimerTicker(object sender, EventArgs e)
        {
            TimeSpan currentTime = new TimeSpan(0, Player.Position.Duration().Minutes, Player.Position.Duration().Seconds);
            SongCurrentTime.Content = currentTime.ToString().Substring(3);
        }

        void TimerTickerSlideText(object sender, EventArgs e)
    {
            if (DelayCounter < 50)
                DelayCounter++;
            

            if (DelayCounter == 50)
            {
                
                if (lengthcounter == maxLengthOfTitle)
                {
                    lengthcounter = 0;
                    DelayCounter = 0;
                }
                lengthcounter++;
                TitleScrollView.ScrollToHorizontalOffset(lengthcounter);
            }

            TimeSpan currentTime = new TimeSpan(0, Player.Position.Duration().Minutes, Player.Position.Duration().Seconds);
            SongCurrentTime.Content = currentTime.ToString().Substring(3);
            trackSlider.Value = Player.Position.TotalSeconds;


        }
        private void trackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        { 
            Player.Position = TimeSpan.FromSeconds(trackSlider.Value);
            slideTimer.Start();
        }
        private void playButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (mediaFileIsOpen)
            {
                if (isPlaying)
                {

                    BitmapImage image = new BitmapImage(new Uri("../img/play-white.png", UriKind.Relative));
                    playButton.Source = image;
                    isPlaying = false;
                    Player.Pause();
                    songTimer.Stop();
                    slideTimer.Stop();

                }
                else
                {

                    BitmapImage image = new BitmapImage(new Uri("../img/pause-white.png", UriKind.Relative));
                    playButton.Source = image;
                    isPlaying = true;
                    Player.Play();
                    songTimer.Start();
                    slideTimer.Start();

                }
            }
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
           
            mediaFileIsOpen = true;
            TimeSpan ts = Player.NaturalDuration.TimeSpan;
            trackSlider.Maximum = ts.TotalSeconds;
            var openDuration = Player.NaturalDuration.TimeSpan;
            var theDuration = new TimeSpan(0, openDuration.Minutes, openDuration.Seconds);
            SongDurationLabel.Content = theDuration.ToString().Substring(3);

        }

        private void trackSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            slideTimer.Stop();
        }

        private void loadSong(object sender, MouseButtonEventArgs e)
        {
            
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".mp3";
                dlg.Filter = "MP3 Files (*.mp3)|*.mp3|M4A Files (*.m4a)|*.m4a|FLAC Files (*.flac)|*.flac";

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();


                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    Player.Source = new Uri(dlg.FileName);
                }
            

        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Volume = VolumeSlider.Value;
        }

        public void PlaySong(string path)
        {
            Player.Source = new Uri(path);
            BitmapImage image = new BitmapImage(new Uri("../img/pause-white.png", UriKind.Relative));
            playButton.Source = image;
            isPlaying = true;
            Player.Play();
            songTimer.Start();
        }
    }
}
