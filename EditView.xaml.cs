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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class EditView : Page
    {
        public EditView()
        {
            InitializeComponent();
            List<Music> songsInPlaylist = new List<Music>();
            //songsInPlaylist.Add(new Music{ title = "Felix", artist= "Alexey", = "180"});
            
        }
    }
}
