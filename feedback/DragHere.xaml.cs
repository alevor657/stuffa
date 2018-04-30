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

namespace WpfApp2.feedback
{
    /// <summary>
    /// Interaction logic for DragHere.xaml
    /// </summary>
    public partial class DragHere : Page
    {
        public DragHere(DragEventHandler dropEvent)
        {
            InitializeComponent();
            Console.WriteLine(this.Grid1.AllowDrop.ToString());
            Console.WriteLine(this.Grid1.AllowDrop ? "this grid allows drop" : "this grid does not allow drop");
            Grid1.Drop += dropEvent;
        }

        private void DragLeave(object sender, DragEventArgs e)
        {
            Console.WriteLine("Drag leave");
            
            this.Grid1.Height = 0;
        }

        private void MouseLeav(object sender, MouseEventArgs e)
        {
            Console.WriteLine("gg");
        }

        private void DragDrop(object sender, DragEventArgs e)
        {
            this.Grid1.Height = 0;
        }
    }
}
