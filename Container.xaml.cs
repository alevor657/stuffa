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
    /// Interaction logic for Container.xaml
    /// </summary>
    public partial class Container : Page
    {

        bool inSettings = false;

        EditView ev = new EditView();
        Settings settings = new Settings();
        public Container()
        {
            InitializeComponent();
            playerControl.Content = new PlayerControl();
            DynamicView.Content = ev;
            PlaylistView.Content = new PlaylistView();
        }

        private void settignsButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(!inSettings)
            {
                DynamicView.Content= settings;
                inSettings = true;
            }
            else
            {
                DynamicView.Content = ev;
                inSettings = false;
            }
        }
    }
}
