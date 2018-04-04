using Stuffa;
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
        Container container;

        bool isLoaded = false;
        bool mediaFileIsOpen;
        bool isPlaying = false;

        double volumeLevel;

        DispatcherTimer slideTimer = new DispatcherTimer();
        DispatcherTimer songTimer = new DispatcherTimer();
        DispatcherTimer labelTimer = new DispatcherTimer();

        DispatcherTimer outFadeTimer = new DispatcherTimer();
        DispatcherTimer inFadeTimer = new DispatcherTimer();

        int lengthcounter = 0;
        int maxLengthOfTitle = 284;
        int DelayCounter = 0;

        bool draging = false;
        

        public PlayerControl(Container container)
        {
            this.container = container;

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
            
            outFadeTimer.Tick += OutFadeTicker;
            inFadeTimer.Tick += InFadeTicker;

            volumeLevel = 0.75;
            Player.Volume = 0;

            //Player.Source = new Uri("D:\\Nedladdningar\\Vicetone - Way Back (feat. Cozi Zuehlsdorff).mp3", UriKind.RelativeOrAbsolute);

            isLoaded = true;
            draging = false;


        }
        
        private void InFadeTicker(object sender, EventArgs e)
        {
            Player.Volume += 0.01;
            if (Player.Volume >= volumeLevel)
            {
                inFadeTimer.Stop();
            }
        }

        private void OutFadeTicker(object sender, EventArgs e)
        {
            Player.Volume -= 0.01;
            if (Player.Volume < 0.001)
            {
                stopFadeOut();
            }
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

        public void TogglePlay() => playButtonUp();

        private void playButtonUp(object sender = null, MouseButtonEventArgs e = null)
        {

            if (mediaFileIsOpen)
            {
                if (isPlaying)
                {

                    fadeOut();
                }
                else
                {

                    BitmapImage image = new BitmapImage(new Uri("../img/pause-white.png", UriKind.Relative));
                    playButton.Source = image;
                    isPlaying = true;
                    Player.Play();
                    songTimer.Start();
                    slideTimer.Start();
                    fadeIn();
                }
            }
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
           
            mediaFileIsOpen = true;
            if (Player.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = Player.NaturalDuration.TimeSpan;
                trackSlider.Maximum = ts.TotalSeconds;
                var openDuration = Player.NaturalDuration.TimeSpan;
                var theDuration = new TimeSpan(0, openDuration.Minutes, openDuration.Seconds);
                SongDurationLabel.Content = theDuration.ToString().Substring(3);
            }
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

            if (isLoaded)
            {
                if (VolumeSlider.Value >= 0.5)
                {
                    BitmapImage image = new BitmapImage(new Uri("../pages/volume-high.png", UriKind.Relative));
                    VolumeButton.Source = image;
                }
                else if (VolumeSlider.Value == 0)
                {
                    BitmapImage image = new BitmapImage(new Uri("../pages/volume_mute.png", UriKind.Relative));
                    VolumeButton.Source = image;

                }
                else if (VolumeSlider.Value < 0.5)
                {
                    BitmapImage image = new BitmapImage(new Uri("../pages/volume_low.png", UriKind.Relative));
                    VolumeButton.Source = image;

                }
                
            }
        }

        public void PlaySong(Music m)
        {
            if (m != null)
            {
                TitleLabel.Text = m.getTitle();
                ArtistLabel.Content = m.getArtist();
                BpmLabel.Content = m.getBpm() + " BPM";

                Player.Source = new Uri(m.getFullPath());
                BitmapImage image = new BitmapImage(new Uri("../img/pause-white.png", UriKind.Relative));
                playButton.Source = image;
                isPlaying = true;
                Player.Play();
                songTimer.Start();
                fadeIn();
            }

        }



        //update Player with the new position
        private void Draging(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            
            trackSlider.Value += e.VerticalChange;
            Player.Position = TimeSpan.FromSeconds(trackSlider.Value);
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if(!draging)
            {
                NextSong();
            }
        }

        public void NextSong()
        {
            container.getRandomSong();
        }

        private void EndDraging(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            draging = false;
            if(trackSlider.Maximum == trackSlider.Value)
            {
                NextSong();
            }
        }

        public Dictionary<string, object> getPlayerState()
        {
            int bpm;
            bool result = int.TryParse(BpmLabel.Content as string, out bpm);

            Dictionary<string, object> d = new Dictionary<string, object>();
            d.Add("song", TitleLabel.Text);
            d.Add("artist", ArtistLabel.Content as string);
            d.Add("bpm", result ? bpm : 0);
            d.Add("isPlaying", isPlaying);

            return d;
        }

        private void StartDraging(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            draging = true;
        }
        
        private void fadeOut()
        {
            // BitConverter.ToInt64(BitConverter.GetBytes(a), 0);
            volumeLevel = Player.Volume;
            long timeInterval = Convert.ToInt64(300000 / Player.Volume);
            outFadeTimer.Interval = new TimeSpan(timeInterval);
            outFadeTimer.Start();
            
        }

        private void fadeIn()
        {

            long timeInterval = Convert.ToInt64(300000 / volumeLevel);
            inFadeTimer.Interval = new TimeSpan(timeInterval);
            inFadeTimer.Start();
        }

        private void stopFadeOut()
        {
            outFadeTimer.Stop();
            Player.Pause();
            songTimer.Stop();
            slideTimer.Stop();

            BitmapImage image = new BitmapImage(new Uri("../img/play-white.png", UriKind.Relative));
            playButton.Source = image;
            isPlaying = false;
        }

        private void stopFadeIn()
        {
            inFadeTimer.Stop();
        }
    }
}
