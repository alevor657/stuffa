using MaterialDesignThemes.Wpf;
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
using Stuffa;

namespace WpfApp2.pages
{
    /// <summary>
    /// Interaction logic for Container.xaml
    /// </summary>
    public partial class Container : Page
    {

        bool inSettings = false;

        EditView ev = new EditView();
        PlayerControl pc = new PlayerControl();
        Settings settings = new Settings();
        public Container()
        {
            Stuffa.MediaPlayer mp = new Stuffa.MediaPlayer(ev, pc);

            InitializeComponent();
            playerControl.Content = pc;
            DynamicView.Content = ev;
            PlaylistView.Content = new PlaylistView(this);
        }

        private void settingsButtonUp(object sender, MouseButtonEventArgs e)
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

        public void snackBarActivate(string message)
        {
            var messageQueue = SnackBarDialog.MessageQueue;
            
            //the message queue can be called from any thread
            Task.Factory.StartNew(() => messageQueue.Enqueue(message, "OKAY", () => { }));
        }
    }
}
