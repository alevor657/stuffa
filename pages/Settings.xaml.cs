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
            hej = true;
        }

        private void coolUnchecked(object sender, RoutedEventArgs e)
        {
            hej = false;
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Console.WriteLine("enter key");
                int givenBpm = Int32.Parse(BpmInput.Text);
                this.container.playBpm(givenBpm, 0);
                this.BPM = givenBpm;
            }
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
        }

        private void IntervalSub_Click(object sender, RoutedEventArgs e)
        {
            CurrentInterval--;
            this.container.setInterval(this.CurrentInterval);
            IntervalInput.Text = CurrentInterval.ToString();
        }
    }
    
}
