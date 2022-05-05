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

namespace ssh_test1
{
    /// <summary>
    /// Interaction logic for ConnectionUC.xaml
    /// </summary>
    public partial class ConnectionUC : UserControl
    {


        private bool _IsOn = true;
        public bool IsOn
        {
            get
            {
                return _IsOn;
            }
            set
            {
                _IsOn = value;
                passText.Text = _IsOn ? "*****" : _pass;
            }
        }

        private string _ipAddr = ConData.ipv4;
        public string ipAddr
        {
            get { return _ipAddr; }
            set
            {
                if (_ipAddr != value)
                {
                    _ipAddr = value;
                }
            }
        }
        private string _name = ConData.name;
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }
        private string _pass = ConData.password;
        public string pass
        {
            get { return _pass; }
            set
            {
                if (_pass != value)
                {
                    _pass = value;
                }
            }
        }

        Window1 wind;
        OptionsBase optb;
        public ConnectionUC(Window1 wind, OptionsBase optb)
        {
            this.optb = optb;
            DataContext = this;
            InitializeComponent();
            RectangeFill();
            this.wind = wind;
            passText.Text = "*****";
        }

        private async Task RectangeFill()
        {
            connRec.Visibility = Visibility.Collapsed;
            bool connection = await Task.Run(() => new Connect().con(true));
            if (connection)
                connRec.Fill = new SolidColorBrush(Colors.Green);
            else
                connRec.Fill = new SolidColorBrush(Colors.Red);
            connRec.Visibility = Visibility.Visible;
        }

        private void ShowHideButton_Click(object sender, RoutedEventArgs e)
        {
            IsOn = !IsOn;
        }

        private void checkConn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void changeCon_Click(object sender, RoutedEventArgs e)
        {
            string name = ConData.name;
            string ipv4 = ConData.ipv4;
            MainWindow main = new MainWindow();
            main.openMain = false;
            main.ShowDialog();
            optb.Close();
            if (ConData.name != name || ConData.ipv4 != ipv4)
                wind.conChanged = true;
        }

        private void checkCon_Click(object sender, RoutedEventArgs e)
        {
            RectangeFill();
        }
    }
}
