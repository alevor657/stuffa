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
using WpfApp2.pages;

namespace WpfApp2.feedback
{
    /// <summary>
    /// Interaction logic for MsgWin.xaml
    /// </summary>
    public partial class MsgWin : Page
    {
        Container Con;
        public MsgWin(string msg, string B0, string B1, Container container)
        {
            InitializeComponent();
            this.Msg.Text = msg;
            this.Button0.Text = B0;
            this.Button1.Text = B1;
            Con = container;
        }

        private void B0MouseOver(object sender, MouseEventArgs e)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = (Color)ColorConverter.ConvertFromString("#781714");

            B0Background.Fill = mySolidColorBrush;
        }

        private void B1MouseOver(object sender, MouseEventArgs e)
        {
            Console.WriteLine("over");
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = (Color)ColorConverter.ConvertFromString("#781714");
            //Mouse.SetCursor(Cursors.Hand);

            B1Background.Fill = mySolidColorBrush;
        }

        private void B0MouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = (Color)ColorConverter.ConvertFromString("#781714");
            mySolidColorBrush.Opacity = 0;

            B0Background.Fill = mySolidColorBrush;
        }

        private void B1MouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = (Color)ColorConverter.ConvertFromString("#781714");
            mySolidColorBrush.Opacity = 0;

          

            B1Background.Fill = mySolidColorBrush;
        }
    }
}
