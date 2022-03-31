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
                if(_ipAddr != value) 
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
                if(_name != value) 
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
                if(_pass != value)
                {
                    _pass = value;
                }
            }
        }
        public ConnectionUC()
        {
            DataContext = this;
            InitializeComponent();
            passText.Text = "*****";
        }

        private void ShowHideButton_Click(object sender, RoutedEventArgs e)
        {
            IsOn = !IsOn;
        }

        private void checkConn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void changeConn_Click(object sender, RoutedEventArgs e)
        {
            ConData.ipv4 = "192.168.10.18";
            ConData.name = "isuser";
            ConData.password = "USER#10";
            //MessageBox.Show(window);
            Connect con = new Connect();
        }
    }
}
