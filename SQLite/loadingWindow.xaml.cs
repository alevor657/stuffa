
using System.Windows;
using System.Windows.Input;

namespace WpfApp2.SQLite
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class loadingWindow : Window
    {
        int max = 0;
        public loadingWindow(string TextMsg)
        {
            InitializeComponent();
            textMsg.Text = TextMsg;
            this.Visibility = Visibility.Visible;
        }

        public void setMax(int max)
        {
            this.max = max;
        }

        public void setPos(int pos)
        {
            pos = (pos / max) * 100;
            if(pos >= 100)
            {
                //close window
            }
            else
            {
                this.Progress.Value = pos;
            }
        }

        public void increasePos()
        {
            this.Dispatcher.Invoke(new System.Action (() => this.Progress.Value++));

            this.Dispatcher.Invoke(new System.Action(() => checkIfFull()
            ));
            

        }

        private void checkIfFull()
        {
            if (this.Progress.Value >= 100)
            {
                this.Close();
            }
        }

        private void dragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
