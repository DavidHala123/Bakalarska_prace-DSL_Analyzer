using InteractiveDataDisplay.WPF;
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
    /// Interaction logic for ChartViewUC.xaml
    /// </summary>
    public partial class ChartViewUC : UserControl
    {
        public ChartViewUC(ChartValues dataFarEnd, ChartValues dataNearEnd, int i, SolidColorBrush up, SolidColorBrush down, string currm, bool hz)
        {
            DataContext = this;
            this.up = up;
            this.down = down;
            InitializeComponent();
            if (hz)
                offset = gethzOffset(currm);
            ChartGraph(dataFarEnd, dataNearEnd, i);
        }
        private SolidColorBrush _up;
        public SolidColorBrush up 
        {
            get { return _up; }
            set 
            {
                if(_up != value)
                    _up = value;
            }
        }
        private SolidColorBrush _down;
        public SolidColorBrush down
        {
            get { return _down; }
            set
            {
                if (_down != value)
                    _down = value;
            }
        }
        private List<bool> graphSelector = new List<bool>();
        private int _numCarrUP;
        public int numCarrUP
        {
            get { return _numCarrUP; }
            set
            {
                if (_numCarrUP != value)
                    _numCarrUP = value;
            }
        }
        private int _numCarrDOWN;
        public int numCarrDOWN
        {
            get { return _numCarrDOWN; }
            set
            {
                if (_numCarrDOWN != value)
                    _numCarrDOWN = value;
            }
        }
        private string _YaxisName;
        public string YaxisName
        {
            get { return _YaxisName; }
            set
            {
                if (_YaxisName != value)
                    _YaxisName = value;
            }
        }
        private int offset = 1;

        List<string> YaxisNames = new List<string>() { "number of bits [-]", "gain [-]", "snr [Hz]", "qln [dBmHz]", "HLIN [db]", "HLOG [dB]", "Tx-PSD [dbmHz]" };
        private async void ChartGraph(ChartValues dataFarEnd, ChartValues dataNearEnd, int i)
        {
            try
            {
                getChart(dataFarEnd, dataNearEnd);
                chart.LeftTitle = YaxisNames[i];
                if(offset > 1)
                    chart.BottomTitle = "Hertz [kHz]";
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private int gethzOffset(string mode) 
        {
            int output = 0;
            switch (mode) 
            {
                case string modeis when modeis.Contains("g992-2-30a"):
                    output = 5;
                    break;
                case string modeis when modeis.Contains("gfast"):
                    output = 52;
                    break;
                default:
                    output = 9;
                    break;
            }
            return output;
        }

        private void getChart(ChartValues chartVnear, ChartValues chartVfar)
        {
            List<List<int>> BandFar = new List<List<int>>();
            List<List<int>> BandNear = new List<List<int>>();
            if (chartVnear.name.Contains("down") || chartVnear.name.Contains("char-func-real"))
            {
                BandFar = splitListForBands(chartVnear.Xvals, chartVnear.Yvals);
                BandNear = splitListForBands(chartVfar.Xvals, chartVfar.Yvals);
            }
            else
            {
                BandFar = splitListForBands(chartVfar.Xvals, chartVfar.Yvals);
                BandNear = splitListForBands(chartVnear.Xvals, chartVnear.Yvals);
            }
            for (int j = 0; j < BandFar.Count - 1; j += 2)
            {
                LineGraph lineGraphFar = new LineGraph()
                {
                    Stroke = down,
                    Padding = new System.Windows.Thickness(0, 30, 0, 0),
                };
                lineGraphFar.Plot(BandFar[j], BandFar[j + 1]);
                chartGrid.Children.Add(lineGraphFar);
            }
            for (int j = 0; j < BandNear.Count - 1; j += 2)
            {
                LineGraph lineGraphNear = new LineGraph()
                {
                    Stroke = up,
                    Padding = new System.Windows.Thickness(0, 30, 0, 0),
                };
                lineGraphNear.Plot(BandNear[j], BandNear[j + 1]);
                chartGrid.Children.Add(lineGraphNear);
            }
        }


        private List<List<int>> splitListForBands(List<int> graphValuesX, List<int> graphValuesY)
        {
            List<int> valsX = new List<int>();
            List<int> valsY = new List<int>();
            List<List<int>> output = new List<List<int>>();
            int xInd = 0;
            for (int j = 0; j < graphValuesX.Count; j++)
            {
                try
                {
                    if (graphValuesX[j + 1] > graphValuesX[j] + offset)
                    {
                        output.Add(graphValuesX.GetRange(xInd, j + 1 - xInd));
                        output.Add(graphValuesY.GetRange(xInd, j + 1 - xInd));
                        xInd = j + 1;
                    }
                }
                catch
                {
                    output.Add(graphValuesX.GetRange(xInd, j + 1 - xInd));
                    output.Add(graphValuesY.GetRange(xInd, j + 1 - xInd));
                    break;
                }
            }
            return output;
        }
    }
}
