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
    /// Interaction logic for DynamicView.xaml
    /// </summary>
    public partial class DynamicView : Page
    {
        Container container;

        public DynamicView(Container c)
        {
            container = c;
            InitializeComponent();
        }
    }
}
