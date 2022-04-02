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
        public bool realtime 
        {
            set 
            {
                populateRealTimeInfo();
                NotifyPropertyChanged();
            }
        }

        private int _attaBitrateUP;
        public int attaBitrateUP 
        {
            get { return _attaBitrateUP; }
            set 
            {
                if(_attaBitrateUP != value) 
                {
                    _attaBitrateUP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _actBitrateUP;
        public int actBitrateUP 
        {
            get { return _actBitrateUP; }
            set
            {
                if (_actBitrateUP != value)
                {
                    _actBitrateUP = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _attaBitrateDOWN;
        public int attaBitrateDOWN
        {
            get { return _attaBitrateDOWN; }
            set 
            {
                if(_attaBitrateDOWN != value) 
                {
                    _attaBitrateDOWN = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _actBitrateDOWN;
        public int actBitrateDOWN
        {
            get { return _actBitrateDOWN; }
            set
            {
                if (_actBitrateDOWN != value)
                {
                    _actBitrateDOWN = value;
                    NotifyPropertyChanged();
                }
            }
        }

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


        private async Task populateRealTimeInfo()
        {
            string actbrUP = "";
            string attbrUP = "";
            string actbrDOWN = "";
            string attbrDOWN = "";
            string generalInfo1 = await Task.Run(() => new SendData("show xdsl operational-data near-end channel " + portIndex + " detail").getResponse());
            string generalInfo2 = await Task.Run(() => new SendData("show xdsl operational-data far-end channel " + portIndex + " detail").getResponse());
            string[] output1 = generalInfo1.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            string[] output2 = generalInfo2.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            string[] outputCombined = output1.Concat(output2).ToArray();
            for (int i = 0; i < outputCombined.Length; i++)
            {
                switch (outputCombined[i])
                {
                    case string generaldata when generaldata.Contains("attain-bitrate-up"):
                        string[] attBUP = generaldata.Split(':');
                        attbrUP = attBUP[1];
                        break;
                    case string generaldata when generaldata.Contains("actual-bitrate-up"):
                        string[] actBUP = generaldata.Split(':');
                        actbrUP = actBUP[1];
                        break;
                    case string generaldata when generaldata.Contains("attain-bitrate-down"):
                        string[] attBDOWN = generaldata.Split(':');
                        attbrDOWN = attBDOWN[1];
                        break;
                    case string generaldata when generaldata.Contains("actual-bitrate-down"):
                        string[] actBDOWN = generaldata.Split(':');
                        actbrDOWN = actBDOWN[1];
                        break;
                }
            }
            attaBitrateUP = Int32.Parse(attbrUP.Trim());
            attaBitrateDOWN = Int32.Parse(attbrDOWN.Trim());
            actBitrateUP = Int32.Parse(actbrUP.Trim());
            actBitrateDOWN = Int32.Parse(actbrDOWN.Trim());
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
