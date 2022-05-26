using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DSL_Analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        //SETTER OF CONNECT WINDOW - SETS COLORS OF INPUT TEXT
        public ConnectionWindow()
        {
            InitializeComponent();
            ipv4.Foreground = new SolidColorBrush(Colors.Gray);
            Name.Foreground = new SolidColorBrush(Colors.Gray);
            password.Foreground = new SolidColorBrush(Colors.Gray);
        }
        //CHECKS IF MAIN WINDOW SHOULD BE OPENED AFTER CONNECTING
        private bool _openMain = true;
        public bool openMain
        {
            get { return _openMain; }
            set { _openMain = value; }
        }

        private bool connected = false;
        public bool _connected
        {
            get { return connected; }
            set 
            {
                connected = value;
            }
        }

        private void ipv4_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //RETRIEVE ALL INFORMATION FROM INPUT BOX AND SENDS IT TO CONNECT FUNCTION/CLASS, WHICH RETURN BOOL VALUE THAT
        //DETERMINES WHETER OR NOT SHOULD BE MAIN WINDOW (Window1) OPENED
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            ConData.ipv4 = ipv4.Text.ToString();
            ConData.name = Name.Text.ToString();
            ConData.password = password.Password.ToString();
            Connect connect = new Connect();
            if (connect.con(false))
            {
                connected = true;
                if (openMain)
                {
                    MainWindow wind = new MainWindow();
                    wind.Show();
                }
                this.Hide();
            }
        }

        private void pass_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //ON FOCUS CHANGING COLORS OF INPUT TEXT
        private void ipv4_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ipv4.Text == "IP Address")
            {
                ipv4.Foreground = new SolidColorBrush(Colors.Black);
                ipv4.Text = "";
            }
        }

        private void ipv4_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ipv4.Text == "")
            {
                ipv4.Foreground = new SolidColorBrush(Colors.Gray);
                ipv4.Text = "IP Address";
            }
        }

        private void Name_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Name.Text == "Name of User")
            {
                Name.Foreground = new SolidColorBrush(Colors.Black);
                Name.Text = "";
            }
        }

        private void Name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Name.Text == "")
            {
                Name.Foreground = new SolidColorBrush(Colors.Gray);
                Name.Text = "Name of User";
            }
        }

        //CLOSES THE WINDOW
        protected override void OnClosed(EventArgs e)
        {
            if (openMain)
            {
                base.OnClosed(e);
                Application.Current.Shutdown();
            }
        }
    }
}
