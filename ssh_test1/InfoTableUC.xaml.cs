using LiveCharts;
using LiveCharts.Configurations;
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
    /// Interaction logic for InfoTableUC.xaml
    /// </summary>
    public partial class InfoTableUC : UserControl, INotifyPropertyChanged
    {
        public InfoTableUC()
        {
            chartValuesDOWN = new ChartValues<int>(new[] { 0 });
            chartValuesUP = new ChartValues<int>(new[] { 0 });
            DataContext = this;
            InitializeComponent();
        }

        private string attaBitrateUP;

        private string actBitrateUP;

        private string attaBitrateDOWN;

        private string actBitrateDOWN;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _portIndex;
        public string portIndex
        {
            get { return _portIndex; }
            set 
            {
                if (_portIndex != value)
                {
                    _portIndex = value;
                    populateGeneralInfo();
                    NotifyPropertyChanged();
                }
            }
        }
        private string _index;
        public string index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _adm_state;
        public string adm_state
        {
            get { return _adm_state; }
            set
            {
                if (_adm_state != value)
                {
                    _adm_state = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _current_init;
        public string current_init
        {
            get { return _current_init; }
            set
            {
                if (_current_init != value)
                {
                    _current_init = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _current_mode;
        public string current_mode
        {
            get { return _current_mode; }
            set
            {
                if (_current_mode != value)
                {
                    _current_mode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _supported_mode;

        public string supported_mode
        {
            get { return _supported_mode; }
            set
            {
                if (_supported_mode != value)
                {
                    _supported_mode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ChartValues<int> _chartValuesUP;
        public ChartValues<int> chartValuesUP 
        {
            get { return _chartValuesUP; }
            set 
            {
                if (_chartValuesUP != value)
                {
                    _chartValuesUP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ChartValues<int> _chartValuesDOWN;
        public ChartValues<int> chartValuesDOWN
        {
            get { return _chartValuesDOWN; }
            set
            {
                if (_chartValuesDOWN != value)
                {
                    _chartValuesDOWN = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private async Task populateGeneralInfo()
        {
            string supmodes = "";
            string generalInfo1 = await Task.Run(() => (new SendData("show xdsl operational-data line " + portIndex + " detail").getResponse()));
            string[] output = generalInfo1.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < output.Length; i++)
            {
                switch (output[i])
                {
                    case string generaldata when generaldata.Contains("if-index"):
                        string[] indexOutput = generaldata.Split(':');
                        index = indexOutput[1];
                        break;
                    case string generaldata when generaldata.Contains("adm-state"):
                        string[] admsOutput = generaldata.Split(':');
                        adm_state = admsOutput[1];
                        break;
                    case string generaldata when generaldata.Contains("cur-init-state"):
                        string[] currinOutput = generaldata.Split(':');
                        current_init = currinOutput[1];
                        break;
                    case string generaldata when generaldata.Contains("cur-op-mode"):
                        string[] curopOutput = generaldata.Split(':');
                        current_mode = curopOutput[1];
                        break;
                        //case string generaldata when generaldata.Contains("g992") || generaldata.Contains("g993") || generaldata.Contains("ansi") || generaldata.Contains("etsi") || generaldata.Contains("802"):
                        //    string[] suppmoOutput = generaldata.Split(':');
                        //    if (!suppmoOutput[1].Contains("dis-"))
                        //        supmodes += suppmoOutput[1] + ", ";
                        //    break;
                }
            }
            supported_mode = supmodes;
        }


        private async Task realTimeInfo()
        {
            string generalInfo1 = new SendData("show xdsl operational-data near-end channel " + portIndex + " detail").getResponse();
            string generalInfo2 = new SendData("show xdsl operational-data far-end channel " + portIndex + " detail").getResponse();
            string[] output1 = generalInfo1.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            string[] output2 = generalInfo2.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            string[] outputCombined = output1.Concat(output2).ToArray();
            for (int i = 0; i < outputCombined.Length; i++)
            {
                switch (outputCombined[i])
                {
                    case string generaldata when generaldata.Contains("attain-bitrate-up"):
                        string[] attBUP = generaldata.Split(':');
                        attaBitrateUP = attBUP[1];
                        break;
                    case string generaldata when generaldata.Contains("actual-bitrate-up"):
                        string[] actBUP = generaldata.Split(':');
                        actBitrateUP = actBUP[1];
                        break;
                    case string generaldata when generaldata.Contains("attain-bitrate-down"):
                        string[] attBDOWN = generaldata.Split(':');
                        attaBitrateDOWN = attBDOWN[1];
                        break;
                    case string generaldata when generaldata.Contains("actual-bitrate-dow"):
                        string[] actBDOWN = generaldata.Split(':');
                        actBitrateUP = actBDOWN[1];
                        break;
                }
            }
            //int actBUPi = Int32.Parse(actBitrateUP.Trim());
            //int actBDOWNi = Int32.Parse(actBitrateDOWN.Trim());
            //realtimeBUP.Add(actBUPi);
            //realtimeBDOWN.Add(actBDOWNi);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
