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
using System.Text.RegularExpressions;


namespace WpfApp2.pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        Container container;

        int CurrentInterval;
        int CurrentBPMJump = 0;


        int InputBPM= 100;

        bool hej = false;
        bool autoState = false;
        public Settings(Container container)
        {
            this.container = container;
            CurrentInterval = 0;
            InitializeComponent();


            IntervalInput.Text = CurrentInterval.ToString();
            BpmInput.Text = InputBPM.ToString();

        }
        public int getBPM()
        {
            return this.InputBPM;
        }
        public void setBPM(int BPM)
        {
            this.InputBPM = BPM;
            this.BpmInput.Text = this.InputBPM.ToString();
        }
        public int getInterval()
        {
            return this.CurrentInterval;
        }
        public bool GetAutoState()
        {
            return this.autoState;
        }
        private void coolChecked(object sender, RoutedEventArgs e)
        {
            autoState = true;
            container.TurnOnBPMShuffle();
            container.SendStateToServerOnUpdate();
        }

        private void coolUnchecked(object sender, RoutedEventArgs e)
        {
            autoState = false;
            container.SendStateToServerOnUpdate();
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {

            if (!BpmInput.Text.ToString().Contains(" ") && BpmInput.Text.ToString() != "")
            {
                int givenBpm = Int32.Parse(BpmInput.Text);
                this.container.playBpm(givenBpm);
                this.InputBPM = givenBpm;
                //container.SendStateToServerOnUpdate();
            }
        }

        public void ChangeJump(int val)
        {
            BPMPerSong.Text = val.ToString();
            CurrentBPMJump = val;
        }
        

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void IntervalAdd_Click(object sender, RoutedEventArgs e)
        {
            CurrentInterval++;
            this.container.setInterval(this.CurrentInterval);
            IntervalInput.Text = CurrentInterval.ToString();
            container.SendStateToServerOnUpdate();
        }

        private void IntervalSub_Click(object sender, RoutedEventArgs e)
        {
            CurrentInterval--;
            if (CurrentInterval < 0)
            {
                CurrentInterval = 0;
            }
            this.container.setInterval(this.CurrentInterval);
            IntervalInput.Text = CurrentInterval.ToString();
            container.SendStateToServerOnUpdate();
        }

        public void SetInterval(int val)
        {
            CurrentInterval = val;
            this.container.setInterval(val);
            IntervalInput.Text = CurrentInterval.ToString();
            container.SendStateToServerOnUpdate();
        }

        private void IncreaseBPMPerSong_Click(object sender, RoutedEventArgs e)
        {
            CurrentBPMJump++;
            BPMPerSong.Text = CurrentBPMJump.ToString();
            container.SendStateToServerOnUpdate();
        }

        private void DecreaseBPMPerSong_Click(object sender, RoutedEventArgs e)
        {
            CurrentBPMJump--;
            if (CurrentBPMJump < 0)
            {
                CurrentBPMJump = 0;
            }
            BPMPerSong.Text = CurrentBPMJump.ToString();
            container.SendStateToServerOnUpdate();
        }

        private void BPMPerSong_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public int GetRange()
        {
            try
            {
                int i = 0;
                Int32.TryParse(BPMPerSong.Text, out i);
                return i;

            }
            catch
            {
                return -1;
            }
        }

        public Dictionary<string, object> getPlayerState()
        {
            
            Dictionary<string, object> d = new Dictionary<string, object>();
            d.Add("autoBpm", autoState);
            try
            {
                d.Add("bpmJump", Int32.Parse(BPMPerSong.Text));
            }
            catch
            {
                d.Add("bpmJump", 0);
            }
            try
            {
                d.Add("bpmInterval", Int32.Parse(IntervalInput.Text));
            }
            catch
            {
                d.Add("bpmInterval", 0);
            }
            try
            {
                d.Add("baseBpm", Int32.Parse(BpmInput.Text));
            }
            catch
            {
                d.Add("baseBpm", 0);

            }


            return d;
        }

        private void IncreaseBaseBPMPerSong_Click(object sender, RoutedEventArgs e)
        {
            InputBPM++;
            BpmInput.Text = InputBPM.ToString();
            container.SendStateToServerOnUpdate();
        }

        private void DecreaseBaseBPMPerSong_Click(object sender, RoutedEventArgs e)
        {
            InputBPM--;
            BpmInput.Text = InputBPM.ToString();
            container.SendStateToServerOnUpdate();
        }

        public void ToggleAutoplay()
        {
            autoState = !autoState;
            container.SendStateToServerOnUpdate();
            toggleButton.IsChecked = autoState;
        }

        public void SetBaseBpm(int val)
        {
            InputBPM = val;
            BpmInput.Text = InputBPM.ToString();
            container.SendStateToServerOnUpdate();
        }

        private void KeyUpTime(object sender, KeyEventArgs e)
        {
            toggleButton2.IsChecked = true;
            int nr = 0;
            try
            {
                nr = Int32.Parse(TimeInput.Text);
                }
            catch
            {
                nr = 0;
            }
            container.SetTimeToPlay(nr);
        }

        private void TimerActive(object sender, RoutedEventArgs e)
        {
            int nr = 0;
            try
            {
                nr = Int32.Parse(TimeInput.Text);
            }
            catch
            {
                nr = 0;
            }
            container.SetTimeToPlay(nr);
        }

        private void TimerUnactive(object sender, RoutedEventArgs e)
        {

            container.SetTimeToPlay(-1);
        }

        private void DonotPauseBetween(object sender, RoutedEventArgs e)
        {

            container.PauseBetweenMusic(-1);
        }

        private void PauseBetween(object sender, RoutedEventArgs e)
        {
            int nr;
            try
            {
                nr = Int32.Parse(PauseInput.Text);
            }
            catch
            {
                nr = 0;
            }
            container.PauseBetweenMusic(nr);

        }

        private void KeyUpPause(object sender, KeyEventArgs e)
        {
            toggleButton3.IsChecked = true;
            int nr;
            try
            {
                nr = Int32.Parse(PauseInput.Text);
            }
            catch
            {
                nr = 0;
            }
            container.PauseBetweenMusic(nr);
        }
    }
    
}
