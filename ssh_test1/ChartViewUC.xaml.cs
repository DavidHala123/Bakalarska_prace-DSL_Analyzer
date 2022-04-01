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
        public ChartViewUC(ChartValues dataFarEnd, ChartValues dataNearEnd, int i)
        {
            InitializeComponent();
            ChartGraph(dataFarEnd, dataNearEnd, i);
        }
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
        private string _name = "";
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                    _name = value;
            }
        }
        List<string> YaxisName = new List<string>() { "number of bits [-]", "gain [-]", "snr [Hz]", "qln [dBmHz]", "HLIN [db]", "HLOG [dB]", "Tx-PSD [dbmHz]" };
        private async void ChartGraph(ChartValues dataFarEnd, ChartValues dataNearEnd, int i)
        {
            List<int> outputList = new List<int>();
            try
            {

                if (Window1.graphSelector[i] == true)
                {

                    Chart chart = new Chart()
                    {
                        LegendVisibility = Visibility.Hidden,
                        LeftTitle = YaxisName[i],
                        BottomTitle = "carrier [i]"
                    };
                    chart.Content = getChart(dataFarEnd, dataNearEnd);
                    chartViewGrid.Children.Add(chart);
                    chartViewGrid.Children.Add(setLegend());
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            ConsoleLogic.ConsoleText = "0";
        }

        private Grid getChart(ChartValues chartVnear, ChartValues chartVfar)
        {
            List<List<int>> BandFar = new List<List<int>>();
            List<List<int>> BandNear = new List<List<int>>();
            LegendItemsPanel legendItemsPanel = new LegendItemsPanel();
            Legend legend = new Legend()
            {
                Content = legendItemsPanel
            };
            Grid grid = new Grid();
            Chart chart = new Chart()
            {
                LegendVisibility = Visibility.Hidden,
            };
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
            for (int j = 0; j < BandFar.Count; j += 2)
            {
                LineGraph lineGraphFar = new LineGraph()
                {
                    Stroke = Window1.BrushUpload,
                    Padding = new System.Windows.Thickness(0, 30, 0, 0),
                };
                lineGraphFar.Plot(BandFar[j], BandFar[j + 1]);
                grid.Children.Add(lineGraphFar);
            }
            for (int j = 0; j < BandNear.Count; j += 2)
            {
                LineGraph lineGraphNear = new LineGraph()
                {
                    Stroke = Window1.BrushDownload,
                    Padding = new System.Windows.Thickness(0, 30, 0, 0),
                };
                lineGraphNear.Plot(BandNear[j], BandNear[j + 1]);
                grid.Children.Add(lineGraphNear);
            }
            return grid;
        }
        private Legend setLegend()
        {
            LegendItemsPanel legendItemsPanel = new LegendItemsPanel();
            Legend output = new Legend()
            {
                Content = legendItemsPanel
            };
            Rectangle rectUP = new Rectangle()
            {
                Width = 10,
                Height = 10,
                Fill = Window1.BrushUpload,
                Stroke = Window1.BrushUpload,
            };

            Rectangle rectDOWN = new Rectangle()
            {
                Width = 10,
                Height = 10,
                Fill = Window1.BrushDownload,
                Stroke = Window1.BrushDownload,
            };
            TextBox textBoxUP = new TextBox()
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                Background = new SolidColorBrush(Colors.Transparent),
                Text = " Upstream"
            };
            TextBox textBoxDOWN = new TextBox()
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                Background = new SolidColorBrush(Colors.Transparent),
                Text = " Downstream"
            };
            DockPanel dockUP = new DockPanel();
            DockPanel dockDOWN = new DockPanel();
            dockUP.Children.Add(rectUP);
            dockUP.Children.Add(textBoxUP);
            dockDOWN.Children.Add(rectDOWN);
            dockDOWN.Children.Add(textBoxDOWN);
            legendItemsPanel.Children.Add(dockUP);
            legendItemsPanel.Children.Add(dockDOWN);
            return output;
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
                    if (graphValuesX[j + 1] != graphValuesX[j] + 1)
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
