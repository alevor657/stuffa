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
    /// Interaction logic for PlayerControl.xaml
    /// </summary>

    public partial class PlayerControl : Page
    {

        bool isPlaying = true;

        public PlayerControl()
        {
            InitializeComponent();
        }

        private void trackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Position = TimeSpan.FromSeconds(trackSlider.Value);
        }
        private void playButtonUp(object sender, MouseButtonEventArgs e)
        {

            //if (mediaFileIsOpen)
            //{



            if (isPlaying)
            {

                BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/WpfApp2;component/play-white.png", UriKind.RelativeOrAbsolute));
                playButton.Source = image;
                isPlaying = false;
                //player.Pause();
                //songTimer.Stop();

            }
            else
            {

                BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/WpfApp2;component/pause-white.png", UriKind.RelativeOrAbsolute));
                playButton.Source = image;
                isPlaying = true;
                //player.Play();
                //songTimer.Start();
            }
            //}
        }
    }
}
