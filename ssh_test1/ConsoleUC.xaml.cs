using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for ConsoleUC.xaml
    /// </summary>
    public partial class ConsoleUC : UserControl, INotifyPropertyChanged
    {
        public ConsoleUC()
        {
            DataContext = this;
            InitializeComponent();
        }

        private string _XDSLStandartS;
        public string XDSLStandartS
        {
            get { return _XDSLStandartS; }
            set
            {
                if (value != _XDSLStandartS)
                {
                    _XDSLStandartS = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private static string consoleText;
        public static string ConsoleText
        {
            get
            {
                return consoleText;
            }
            set
            {
                string output = "";
                switch (value)
                {
                    case "0":
                        output = "";
                        break;
                    case "1":
                        output = "Sending request to DSLAM";
                        break;
                    case "2":
                        output = "Reading response";
                        break;
                    case "3":
                        output = "Processing data";
                        break;
                    case "4":
                        output = "Exporting data";
                        break;

                }
                consoleText = output;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(ConsoleText)));
                //NotifyPropertyChanged();
            }
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
