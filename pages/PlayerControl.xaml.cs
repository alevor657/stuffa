using Stuffa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        int timer = -1;

        bool isLoaded = false;
        bool mediaFileIsOpen;
        bool isPlaying = false;

        bool fadingOutForNextSong;
        int pauseBetween = -1;


        double volumeLevel;

        DispatcherTimer songTimer = new DispatcherTimer();
        DispatcherTimer labelTimer = new DispatcherTimer();

        DispatcherTimer outFadeTimer = new DispatcherTimer();
        DispatcherTimer inFadeTimer = new DispatcherTimer();

        int lengthcounter = 0;
        int maxLengthOfTitle = 284;
        int DelayCounter = 0;

        int shuffleLoop = 1;

        bool draging;


        public PlayerControl(Container container)
        {
            this.container = container;

            mediaFileIsOpen = true;
            InitializeComponent();
            TitleScrollView.ScrollToVerticalOffset(288);

            labelTimer.Interval = new TimeSpan(25000);
            labelTimer.Tick += TimerTickerSlideText;
            labelTimer.Start();

            songTimer.Interval = new TimeSpan(50000);
            songTimer.Tick += SongTimerTicker;

            outFadeTimer.Tick += OutFadeTicker;
            inFadeTimer.Tick += InFadeTicker;

            volumeLevel = 0.75;
            Player.Volume = 0;
            fadingOutForNextSong = false;

            //Player.Source = new Uri("D:\\Nedladdningar\\Vicetone - Way Back (feat. Cozi Zuehlsdorff).mp3", UriKind.RelativeOrAbsolute);

            isLoaded = true;
            draging = false;
            isPlaying = false;


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
                if (fadingOutForNextSong)
                {
                    outFadeTimer.Stop();
                    NextSong();
                }
                stopFadeOut();
            }
        }

        private void SliderTicker(object sender, EventArgs e)
        {
            
        }

        private void SongTimerTicker(object sender, EventArgs e)
        {
            
              TimeSpan currentTime = new TimeSpan(0, Player.Position.Duration().Minutes, Player.Position.Duration().Seconds);
            SongCurrentTime.Content = currentTime.ToString().Substring(3);

            if (!draging)
            {
                trackSlider.Value = Player.Position.TotalSeconds;
            }

            
            if (currentTime.TotalSeconds > trackSliderTimer.Value - 3 && !fadingOutForNextSong)
            {
                fadeOutForNext();
                fadingOutForNextSong = true;
            }



            if (Player.NaturalDuration.HasTimeSpan)
            {
                if ((Player.NaturalDuration.TimeSpan.TotalSeconds - currentTime.TotalSeconds) <= 3 && !fadingOutForNextSong)
                {
                    fadeOut(currentTime.TotalSeconds * 100000);
                    fadingOutForNextSong = true;
                }


            }


        }

        void TimerTickerSlideText(object sender, EventArgs e)
        {
            if (DelayCounter < 50)
                DelayCounter++;


            if (DelayCounter == 50)
            {
                container.PreventSleep();
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




        }

        public void unloadMusicImmediately()
        {
            this.Player.Source = null;
        }

        public string getMusicTitle()
        {
            return TitleLabel.Text;
        }

        public void TogglePlay() => playButtonUp();

        private void playButtonUp(object sender = null, MouseButtonEventArgs e = null)
        {

            if (mediaFileIsOpen)
            {
                if (isPlaying)
                {
                    BitmapImage image = new BitmapImage(new Uri("../img/play-white.png", UriKind.Relative));
                    playButton.Source = image;
                    fadeOut();
                    isPlaying = false;
                    inFadeTimer.Stop();
                    fadingOutForNextSong = false;
                }
                else
                {
                    stopFadeOut();
                    BitmapImage image = new BitmapImage(new Uri("../img/pause-white.png", UriKind.Relative));
                    playButton.Source = image;
                    isPlaying = true;
                    Player.Play();
                    songTimer.Start();
                    fadeIn();
                    outFadeTimer.Stop();
                }
                container.SendStateToServerOnUpdate();

            }
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {

            mediaFileIsOpen = true;
            if (Player.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = Player.NaturalDuration.TimeSpan;
                trackSliderTimer.Maximum = ts.TotalSeconds;
                if(timer != -1)
                {
                    trackSliderTimer.Value = timer;
                    BrushConverter conv = new BrushConverter();
                    SolidColorBrush brush = conv.ConvertFromString("#d3d3d3") as SolidColorBrush;
                    trackSliderTimer.Foreground = brush;
                }
                else
                {
                    trackSliderTimer.Value = 99999;
                    BrushConverter conv = new BrushConverter();
                    SolidColorBrush brush = conv.ConvertFromString("#00FFFFFF") as SolidColorBrush;
                    trackSliderTimer.Foreground = brush;
                }

                trackSlider.Maximum = ts.TotalSeconds;
                var openDuration = Player.NaturalDuration.TimeSpan;
                var theDuration = new TimeSpan(0, openDuration.Minutes, openDuration.Seconds);
                SongDurationLabel.Content = theDuration.ToString().Substring(3);
            }
        }

        public void SetTimer(int t)
        {
            timer = t;
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
            volumeLevel = VolumeSlider.Value;
            Player.Volume = VolumeSlider.Value;
            SetVolumeIcon();


        }

        private void SetVolumeIcon()
        {
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
                container.SendStateToServerOnUpdate();

            }
        }

        public void SetVolume(float k)
        {
            volumeLevel = k; 
            VolumeSlider.Value = k;
            Player.Volume = k;
            SetVolumeIcon();
        }

        public void PlaySong(Music m)
        {
            if (m != null)
            {
                TitleLabel.Text = m.getTitle();
                ArtistLabel.Content = m.getArtist();
                BpmLabel.Content = m.getBpm() + " BPM";

                isPlaying = true;

                Player.Source = null;

                Player.Play();
                
                Player.Source = new Uri(m.getFullPath());
                trackSlider.Value = 0;

                BitmapImage image = new BitmapImage(new Uri("../img/pause-white.png", UriKind.Relative));
                playButton.Source = image;
                songTimer.Start();
                fadeIn();
                fadingOutForNextSong = false;


                container.SendStateToServerOnUpdate();

            }

        }



        //update Player with the new position
        private void Draging(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

            //Player.Position = TimeSpan.FromSeconds(trackSlider.Value);
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (!draging)
            {
                NextSong();
            }
        }

        public void NextSong()
        {
            if(pauseBetween > 0)
            {
                fadeOut();
                System.Threading.Thread myThread;

                myThread = new System.Threading.Thread(new
   System.Threading.ThreadStart(NextSongThread));
                myThread.Start();
            }
            else
            {
                container.getRandomSong();
                container.SendStateToServerOnUpdate();
            }

        }

        private void NextSongThread()
        {
            System.Threading.Thread.Sleep(pauseBetween*1000+3000);
            container.Dispatcher.Invoke(container.getRandomSong);
            container.Dispatcher.Invoke(container.SendStateToServerOnUpdate);
        }
        public void PauseBetweenMusic(int pause)
        {
            pauseBetween = pause;
        }

        private void EndDraging(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            draging = false;
            Player.Position = TimeSpan.FromSeconds(trackSlider.Value);
            if (trackSlider.Maximum == trackSlider.Value)
            {
                NextSong();
            }
        }

        public Dictionary<string, object> getPlayerState()
        {
            int bpm = 0;
            Int32.TryParse((BpmLabel.Content as string).Split(' ')[0], out bpm);

            Dictionary<string, object> d = new Dictionary<string, object>();
            d.Add("song", TitleLabel.Text);
            d.Add("artist", ArtistLabel.Content as string);
            // Always 0, fix needed? --------------------------------------------------------------------------------------- Is this still true?
            d.Add("bpm", bpm);
            d.Add("isPlaying", isPlaying);
            d.Add("soundLevel", Convert.ToInt32(volumeLevel * 100));

            return d;
        }

        private void StartDraging(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            draging = true;
        }

        private void fadeOut(double secondsLeft = 300000)
        {
            if ((VolumeSlider.Value != 0 || Player.Volume != 0) && fadingOutForNextSong != true)
            {
                volumeLevel = VolumeSlider.Value;
                long timeInterval = Convert.ToInt64(300000 / Player.Volume);
                outFadeTimer.Interval = new TimeSpan(timeInterval);
                outFadeTimer.Start();
            }
            else
            {
                stopFadeOut();

            }
        }
        bool startNewSong = false;
        private void fadeOutForNext(double secondsLeft = 300000)
        {
            if ((Player.Volume > 0) && fadingOutForNextSong != true)
            {
                volumeLevel = VolumeSlider.Value;
                long timeInterval = Convert.ToInt64(300000 / Player.Volume);
                outFadeTimer.Interval = new TimeSpan(timeInterval);
                outFadeTimer.Start();
                startNewSong = true;
            }
            else
            {
                stopFadeOut();



            }
        }

        private void fadeIn()
        {
            if (volumeLevel != 0)
            {
                long timeInterval = Convert.ToInt64(300000 / volumeLevel);
                inFadeTimer.Interval = new TimeSpan(timeInterval);
                inFadeTimer.Start();
            }
        }

        private void stopFadeOut()
        {
            outFadeTimer.Stop();
            Player.Pause();
            songTimer.Stop();
            if (!startNewSong)
            {
                isPlaying = false;
                fadingOutForNextSong = false;
            }
            else
            {
                startNewSong = false;
                Player.Play();
                fadingOutForNextSong = false;
                isPlaying = true;
                songTimer.Start();
            }
        }

        private void stopFadeIn()
        {
            inFadeTimer.Stop();
        }

        private void NextSong(object sender, MouseButtonEventArgs e)
        {
            this.NextSong();
        }
        public int getShuffleState()
        {
            return this.shuffleLoop;
        }
          
    private void ShuffleButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Bara lägga in funktionell kod under eller över bildbytet, den räknar i slutet.
            if (shuffleLoop == 0)
            {
                // INGEN SHUFFLE
                BitmapImage image = new BitmapImage(new Uri("../img/shuffle.png", UriKind.Relative));
                ShuffleButton.Source = image;
                //do not show markings
                container.showSelectedPlaylist();

            }
            else if (shuffleLoop == 1)
            {
                // VANLIG SHUFFLE
                BitmapImage image = new BitmapImage(new Uri("../img/shuffle_active.png", UriKind.Relative));
                ShuffleButton.Source = image;
            }
            else if (shuffleLoop == 2)
            {
                // BPM SHUFFLE
                BitmapImage image = new BitmapImage(new Uri("../img/shuffle_bpm_active2.png", UriKind.Relative));
                ShuffleButton.Source = image;
                container.ShowBpmMarking();
            }
            shuffleLoop = (shuffleLoop + 1) % 3;
            container.SendStateToServerOnUpdate();
            

        }
        public void SetToBPMShuffle()
        {
            shuffleLoop = 0;
            // BPM SHUFFLE
            BitmapImage image = new BitmapImage(new Uri("../img/shuffle_bpm_active2.png", UriKind.Relative));
            ShuffleButton.Source = image;
        }

        public void RestartSong(object sender = null, MouseButtonEventArgs e = null)
        {
            this.Player.Stop();
            if(isPlaying)
            {
                this.Player.Play();
                fadeIn();

            }
            //container.SendStateToServerOnUpdate();
        }

        public int GetCurrentVolumeAsInt()
        {
            return (int)(VolumeSlider.Value * 100);
        }

        private void getIp(object sender, ToolTipEventArgs e)
        {
            //clear previsus mesage
                MobileToolTip.Items.Clear();

                //get name of host
                string hostName = Dns.GetHostName();
                Console.WriteLine(hostName);

                //get the Ip address
                string localIp = Dns.GetHostByName(hostName).AddressList[0].ToString();
                Console.WriteLine("Ip address is : " + localIp);
                MobileToolTip.Items.Add("mobile key: " + localIp);
            
            
        }

        public void setMobileActive()
        {
            BrushConverter conv = new BrushConverter();
            SolidColorBrush brush = conv.ConvertFromString("#00796b") as SolidColorBrush;


            mobile.Fill = brush;
        }
    }
}