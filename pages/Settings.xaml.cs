﻿using System;
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
        int BPM;
        bool hej = false;
        bool autoState = false;
        public Settings(Container container)
        {
            this.container = container;
            CurrentInterval = 0;
            InitializeComponent();
            BPM = 0;

            IntervalInput.Text = CurrentInterval.ToString();

        }
        public int getBPM()
        {
            return this.BPM;
        }
        public void setBPM(int BPM)
        {
            this.BPM = BPM;
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
        }

        private void coolUnchecked(object sender, RoutedEventArgs e)
        {
            autoState = false;
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            
                int givenBpm = Int32.Parse(BpmInput.Text);
                this.container.playBpm(givenBpm);
                this.BPM = givenBpm;
            //container.SendStateToServerOnUpdate();
            
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
            d.Add("autoBpm", toggleButton.IsChecked);
            d.Add("bpmJump", Int32.Parse(BPMPerSong.Text));
            d.Add("bpmInterval", Int32.Parse(IntervalInput.Text));
            d.Add("baseBpm", Int32.Parse(BpmInput.Text));


            return d;
        }
    }
    
}
