
using System.Windows;
using System.Windows.Input;

namespace WpfApp2.SQLite
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class loadingWindow : Window
    {
        public loadingWindow(string TextMsg)
        {
            InitializeComponent();
            textMsg.Text = TextMsg;
            this.Visibility = Visibility.Visible;
        }

        public void setMax(int max)
        {
            this.Dispatcher.Invoke(new System.Action (() => this.Progress.Maximum = max));
        }

        public void increasePos()
        {
            this.Dispatcher.Invoke(new System.Action (() => this.Progress.Value++));

            this.Dispatcher.Invoke(new System.Action(() => checkIfFull()
            ));
            

        }

        private void checkIfFull()
        {
            if (this.Progress.Value == this.Progress.Maximum)
            {
                this.Close();
                System.Console.WriteLine("progress bar finnished");
            }
        }

        private void dragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
