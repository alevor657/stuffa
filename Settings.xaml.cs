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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {

        bool hej = false;
        public Settings()
        {
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
    }
}
