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
        public bool colorChangeBool = false;
        public InfoTableUC()
        {
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
        private SolidColorBrush _up = new SolidColorBrush(Colors.OrangeRed);
        public SolidColorBrush up 
        {
            get { return _up; }
            set
            {
                _up = value;
                NotifyPropertyChanged();
                if (actBitrateUP > 0)
                {
                    actBitrateUP = actBitrateUP - 1;
                    actBitrateUP = actBitrateUP + 1;
                }
            }
        }

        private SolidColorBrush _down = new SolidColorBrush(Colors.RoyalBlue);
        public SolidColorBrush down
        {
            get { return _down; }
            set
            {
                if (_down != value)
                {
                    _down = value;
                    NotifyPropertyChanged();
                    if (actBitrateDOWN > 0)
                    {
                        actBitrateDOWN = actBitrateDOWN - 1;
                        actBitrateDOWN = actBitrateDOWN + 1;
                    }
                }
            }
        }

        private double _attaBitrateUP;
        public double attaBitrateUP 
        {
            get { return _attaBitrateUP; }
            set 
            {
                _attaBitrateUP = value;
                NotifyPropertyChanged();
            }
        }

        private double _actBitrateUP;
        public double actBitrateUP 
        {
            get { return _actBitrateUP; }
            set
            {
                _actBitrateUP = value;
                NotifyPropertyChanged();
            }
        }

        private double _attaBitrateDOWN;
        public double attaBitrateDOWN
        {
            get { return _attaBitrateDOWN; }
            set 
            {
                _attaBitrateDOWN = value;
                NotifyPropertyChanged();
            }
        }

        private double _actBitrateDOWN;
        public double actBitrateDOWN
        {
            get { return _actBitrateDOWN; }
            set
            {
                _actBitrateDOWN = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _portIndex;
        public string portIndex
        {
            get { return _portIndex; }
            set 
            {
                _portIndex = value;
                populateGeneralInfo();
                NotifyPropertyChanged();
            }
        }
        private string _index;
        public string index
        {
            get { return _index; }
            set
            {
                _index = value;
                NotifyPropertyChanged();
            }
        }
        private string _adm_state;
        public string adm_state
        {
            get { return _adm_state; }
            set
            {
                _adm_state = value;
                NotifyPropertyChanged();
            }
        }
        private string _current_init;
        public string current_init
        {
            get { return _current_init; }
            set
            {
                _current_init = value;
                NotifyPropertyChanged();
            }
        }
        private string _current_mode;
        public string current_mode
        {
            get { return _current_mode; }
            set
            {
                _current_mode = value;
                NotifyPropertyChanged();
            }
        }
        private string _supported_mode;

        public string supported_mode
        {
            get { return _supported_mode; }
            set
            {
                _supported_mode = value;
                NotifyPropertyChanged();
            }
        }

        private int _chartValuesUP;
        public int chartValuesUP 
        {
            get { return _chartValuesUP; }
            set 
            {
                _chartValuesUP = value;
                NotifyPropertyChanged();
            }
        }

        public int chartValuesDown 
        {
            get { return _chartValuesCount - _chartValuesUP; }
        }

        private int _chartValuesCount;
        public int chartValuesCount
        {
            get { return _chartValuesCount; }
            set
            {
                _chartValuesCount = value;
                NotifyPropertyChanged();
            }
        }

        private string _noiseMUP;
        public string noiseMUP 
        {
            get { return _noiseMUP; }
            set 
            {
                _noiseMUP = value;
                NotifyPropertyChanged();
            }
        }

        private string _noiseMDOWN;
        public string noiseMDOWN
        {
            get { return _noiseMDOWN; }
            set
            {
                _noiseMDOWN = value;
                NotifyPropertyChanged();
            }
        }

        private string _outputPUP;
        public string outputPUP
        {
            get { return _outputPUP; }
            set
            {
                _outputPUP = value;
                NotifyPropertyChanged();
            }
        }

        private string _outputPDOWN;
        public string outputPDOWN
        {
            get { return _outputPDOWN; }
            set
            {
                _outputPDOWN = value;
                NotifyPropertyChanged();
            }
        }

        private string _signalAtUP;
        public string signalAtUP
        {
            get { return _signalAtUP; }
            set
            {
                _signalAtUP = value;
                NotifyPropertyChanged();
            }
        }

        private string _signalAtDOWN;
        public string signalAtDOWN
        {
            get { return _signalAtDOWN; }
            set
            {
                _signalAtDOWN = value;
                NotifyPropertyChanged();
            }
        }

        private string _txPsdUP;
        public string txPsdUP
        {
            get { return _txPsdUP; }
            set
            {
                _txPsdUP = value;
                NotifyPropertyChanged();
            }
        }

        private string _txPsdDOWN;
        public string txPsdDOWN
        {
            get { return _txPsdDOWN; }
            set
            {
                _txPsdDOWN = value;
                NotifyPropertyChanged();
            }
        }

        private async Task populateGeneralInfo()
        {
            suppm_value.Items.Clear();
            string supmodes = "";
            string generalInfo1 = await Task.Run(() => (new SendData("show xdsl operational-data line " + portIndex + " detail").getResponse()) + new SendData("show xdsl operational-data near-end line " + portIndex + " detail").getResponse());
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
                    case string generaldata when generaldata.Replace("-", " ").Contains("g992") || generaldata.Replace("-", " ").Contains("g993"):
                            string[] suppmoOutput = generaldata.Split(':');;
                        if (!suppmoOutput[1].Contains("dis") && !suppmoOutput[0].Contains("actual-opmode"))
                            suppm_value.Items.Add(suppmoOutput[1].Replace(" ", "-"));
                        break;
                }
            }
            supported_mode = supmodes;
            //getMaxBitrate();
        }


        private async Task populateRealTimeInfo()
        {
            string actbrUP = "";
            string attbrUP = "";
            string actbrDOWN = "";
            string attbrDOWN = "";
            string rtInfo1 = await Task.Run(() => new SendData("show xdsl operational-data near-end channel " + portIndex + " detail").getResponse());
            string rtInfo2 = await Task.Run(() => new SendData("show xdsl operational-data far-end channel " + portIndex + " detail").getResponse());
            string rtInfo3 = await Task.Run(() => new SendData("show xdsl operational-data near-end line " + portIndex + " detail").getResponse());
            string rtInfo4 = await Task.Run(() => new SendData("show xdsl operational-data far-end line " + portIndex + " detail").getResponse());
            string[] output1 = rtInfo1.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            string[] output2 = rtInfo2.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            string[] output3 = rtInfo3.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            string[] output4 = rtInfo4.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            string[] outputCombined1 = output1.Concat(output2).ToArray();
            string[] outputCombined2 = output3.Concat(output4).ToArray();
            string[] outputCombined = outputCombined1.Concat(outputCombined2).ToArray();
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
                    case string generaldata when generaldata.Contains("noise-margin-up"):
                        string[] nmUP = generaldata.Split(':');
                        noiseMUP = nmUP[1];
                        break;
                    case string generaldata when generaldata.Contains("noise-margin-down"):
                        string[] nmDOWN = generaldata.Split(':');
                        noiseMDOWN = nmDOWN[1];
                        break;
                    case string generaldata when generaldata.Contains("output-power-up"):
                        string[] opUP = generaldata.Split(':');
                        outputPUP = opUP[1];
                        break;
                    case string generaldata when generaldata.Contains("output-power-down"):
                        string[] opDOWN = generaldata.Split(':');
                        outputPDOWN = opDOWN[1];
                        break;
                    case string generaldata when generaldata.Contains("sig-attenuation-up"):
                        string[] saUP = generaldata.Split(':');
                        signalAtUP = saUP[1];
                        break;
                    case string generaldata when generaldata.Contains("sig-attenuation-down"):
                        string[] saDOWN = generaldata.Split(':');
                        signalAtDOWN = saDOWN[1];
                        break;
                    case string generaldata when generaldata.Contains("actual-psd-up"):
                        string[] txpUP = generaldata.Split(':');
                        txPsdUP = txpUP[1];
                        break;
                    case string generaldata when generaldata.Contains("actual-psd-down"):
                        string[] txpDOWN = generaldata.Split(':');
                        txPsdDOWN = txpDOWN[1];
                        break;
                }
            }
            getMaxBitrate(current_mode);
            actBitrateUP = Convert.ToDouble(actbrUP.Trim()) / 1000;
            actBitrateDOWN = Convert.ToDouble(actbrDOWN.Trim()) / 1000;
        }

        private void getMaxBitrate(string currMode)
        {
            switch (currMode)
            {
                case string mode when mode.Contains("g992-1-a") || mode.Contains("g992-1-b"):
                    attaBitrateUP = 1.5;
                    attaBitrateDOWN = 8;
                    break;
                case string mode when mode.Contains("g992-2-a"):
                    attaBitrateUP = 0.5;
                    attaBitrateDOWN = 1.5;
                    break;
                case string mode when mode.Contains("g992-3-a") || mode.Contains("g922-3-b"):
                    attaBitrateUP = 1;
                    attaBitrateDOWN = 12;
                    break;
                case string mode when mode.Contains("g992-3-aj") || mode.Contains("g992-3-am"):
                    attaBitrateUP = 3;
                    attaBitrateDOWN = 12;
                    break;
                case string mode when mode.Contains("g992-3-l1") || mode.Contains("g992-3-l2"):
                    attaBitrateUP = 0.8;
                    attaBitrateDOWN = 5;
                    break;
                case string mode when mode.Contains("g992-5-a") || mode.Contains("g992-5-b"):
                    attaBitrateUP = 1;
                    attaBitrateDOWN = 25;
                    break;
                case string mode when mode.Contains("g992-5-am"):
                    attaBitrateUP = 3;
                    attaBitrateDOWN = 25;
                    break;
                case string mode when mode.Contains("g992-5-aj"):
                    attaBitrateUP = 3;
                    attaBitrateDOWN = 25;
                    break;
                case string mode when mode.Contains("g993-2-8a") || mode.Contains("g993-2-8b") || mode.Contains("g993-2-8c") || mode.Contains("g993-2-8d"):
                    attaBitrateUP = 12;
                    attaBitrateDOWN = 24;
                    break;
                case string mode when mode.Contains("g993-2-12a") || mode.Contains("g993-2-12b"):
                    attaBitrateUP = 24;
                    attaBitrateDOWN = 24;
                    break;

                case string mode when mode.Contains("g993-2-17a"):
                    attaBitrateUP = 24;
                    attaBitrateDOWN = 48;
                    break;
                case string mode when mode.Contains("g993-2-30a"):
                    attaBitrateUP = 28;
                    attaBitrateDOWN = 28;
                    break;
                case string mode when mode.Contains("g993-2-35b"):
                    attaBitrateUP = 100;
                    attaBitrateDOWN = 300;
                    break;

            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
