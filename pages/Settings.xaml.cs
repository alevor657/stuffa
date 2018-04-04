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

namespace WpfApp2.pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        Container container;

        bool hej = false;
        public Settings(Container container)
        {
            this.container = container;
            InitializeComponent();
        }

        private void coolChecked(object sender, RoutedEventArgs e)
        {
            hej = true;
            coolLabel.Content = "Mattias äter gröna bananer - " + hej;
        }

        private void coolUnchecked(object sender, RoutedEventArgs e)
        {
            hej = false;
            coolLabel.Content = "Mattias äter FAKTISKT RÖDA bananer - " + hej;
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Console.WriteLine("enter key");
                int givenBpm = Int32.Parse(BpmInput.Text);
                this.container.playBpm(givenBpm, 0);
            }
        }
    }
}
