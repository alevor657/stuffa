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
using System.Windows.Shapes;

namespace WpfApp2.feedback
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Edit : Window
    {
        public Edit(int curBpm, string curTitle, string curArtist, MouseButtonEventHandler edit)
        {
            InitializeComponent();
            this.BpmOrginal.Text = curBpm.ToString();
            this.TitleOrginal.Text = curTitle;
            this.ArtistOrginal.Text = curArtist;
            //this.edit.MouseLeftButtonUp += edit;
        }


    }
}
