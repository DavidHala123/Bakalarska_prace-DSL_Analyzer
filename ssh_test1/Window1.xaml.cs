using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InteractiveDataDisplay.WPF;
using System.Linq;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace ssh_test1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        List<List<int>> GraphYvalues;
        List<List<int>> GraphXvalues;
        List<string> GraphListOfNames;
        string dataFarEnd = "";
        string dataNearEnd = "";
        string selectedPort;
        private static bool _OptionsChanged = false;
        public static bool OptionsChanged 
        { 
            get { return _OptionsChanged; }
            set 
            {
                if(_OptionsChanged != value) 
                {
                    _OptionsChanged = value;
                }
            }
        }
        private static SolidColorBrush _BrushUpload = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush BrushUpload
        {
            get { return _BrushUpload; }
            set 
            {
                if(_BrushUpload != value) 
                {
                    _BrushUpload = value;
                }
            }
        }
        private static SolidColorBrush _BrushDownload = new SolidColorBrush(Colors.Blue);
        public static SolidColorBrush BrushDownload
        {
            get { return _BrushDownload; }
            set
            {
                if (_BrushDownload != value) 
                {
                    _BrushDownload = value;
                }
            }
        }
        private static List<bool> _graphSelector;
        public static List<bool> graphSelector 
        {
            get { return _graphSelector; }
            set 
            {
                if(_graphSelector != value) 
                {
                    _graphSelector = value;
                }
            }
        }

        bool fromFile = false;
        List<string> YaxisName = new List<string>() { "number of bits [bits]", "gain [Hz]", "snr [Hz]", "qln [Hz]", "idk", "idk", "idk" };
        public Window1()
        {
            try 
            {
                _graphSelector = new List<bool>() { true, true, true, true, true, true, true };
                async Task Initialize() 
                {
                    List<PortData> listofobj = await Task.Run(() => new PortBoxLogic(new SendData
                       ("show xdsl operational-data line").getResponse() ?? throw new ArgumentNullException()).getPortDataCombo());
                    foreach (PortData obj in listofobj)
                    {
                        PortBox.Items.Add(obj);
                    }
                    string xdslStandartStr = await sendToDSLAM("show xdsl operational-data line | match match exact:gfast");
                    if (!xdslStandartStr.Contains("up"))
                    {
                        XDSLStandart.Text = "ADSL/VDSL";
                    }
                    else
                        XDSLStandart.Text = "Gfast";
                }
                InitializeComponent();
                con();
                Initialize();
            }
            catch { }
        }
        public void ErrorMessage(string errorText)
        {
            MessageBox.Show(errorText);
        }

        private void command_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void GraphField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void send_Click(object sender, RoutedEventArgs e)
        {
            ChartGraph();
        }
        private async void ChartGraph()
        {
            if (PortBox.SelectedItem == null && !fromFile)
            {
                MessageBox.Show("Please select port you wish to analyze first");
            }
            try
            {
                if (!fromFile)
                {
                    dataFarEnd = await sendToDSLAM("show xdsl carrier-data far-end " + selectedPort + " detail");
                    dataNearEnd = await sendToDSLAM("show xdsl carrier-data near-end " + selectedPort + " detail");
                }
                int graphIndex = 0;
                GraphField.Items.Clear();
                GraphLogic graphLog = await Task.Run(() => new GraphLogic(dataFarEnd, dataNearEnd, graphSelector));
                for (int i = 0; i < graphSelector.Count(); i++)
                {
                    if (graphSelector[i] == true)
                    {
                        graphLog.SelectGraphNeeeded(i);
                        Grid chartView = new Grid();
                        Chart chart = new Chart()
                        {
                            LegendVisibility = Visibility.Hidden,
                        };
                        chart.Content = getChart(graphIndex, graphLog.chartV[graphIndex], graphLog.chartV[graphIndex + 1]);
                        chartView.Children.Add(chart);
                        chartView.Children.Add(setLegend());
                        GraphField.Items.Add(new TabItem
                        {
                            Header = graphLog.chartV[graphIndex].name.Replace("-up", "").Replace("-down", ""),
                            Content = chartView,
                        });
                        graphIndex += 2;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            ConsoleLogic.ConsoleText = "0";
        }


        private static async Task<string> sendToDSLAM(string input) 
        {
            return await Task.Run(() => new SendData(input).getResponse() ?? throw new ArgumentNullException());
        }

        private Grid getChart(int i, ChartValues chartVnear, ChartValues chartVfar)
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
                    Stroke = BrushUpload,
                    Padding = new System.Windows.Thickness(0,30,0,0),
                };
                lineGraphFar.Plot(BandFar[j], BandFar[j + 1]);
                grid.Children.Add(lineGraphFar);
            }
            for (int j = 0; j < BandNear.Count; j += 2)
            {
                LineGraph lineGraphNear = new LineGraph()
                {
                    Stroke = BrushDownload,
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
                Fill = BrushUpload,
                Stroke = BrushUpload,
            };

            Rectangle rectDOWN = new Rectangle()
            {
                Width = 10,
                Height = 10,
                Fill = BrushDownload,
                Stroke = BrushDownload,
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

        private async void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GraphField.Items.Clear();
            dataFarEnd = "";
            dataNearEnd = "";
            if(GraphYvalues != null && GraphXvalues != null) 
            {
                GraphYvalues.Clear();
                GraphXvalues.Clear();
            }
            if (fromFile && PortBox.SelectedIndex == 0) 
            {
                return;
            }
            var selected = (PortData)PortBox.SelectedItem;
            if (selected.portState.Contains("down.png"))
            {
                MessageBox.Show("Port you are trying to access is currently offline");
                return;
            }
            else
            {
                if (fromFile) 
                {
                    PortBox.Items.RemoveAt(0);
                }
                fromFile = false;
                selectedPort = selected.portName.ToString();
                basinfo.Items.Clear();
                advinfo.Items.Clear();
                try 
                {
                    List<PortData> infoListData = await Task.Run(() => new InfoTableLogic(selectedPort,
                    new SendData("show xdsl operational-data far-end line " + selectedPort + " detail").getResponse()
                    ?? throw new ArgumentNullException(),
                        new SendData("show xdsl operational-data near-end line " + selectedPort + " detail").getResponse()
                        ?? throw new ArgumentNullException(),
                        new SendData("show xdsl operational-data line " + selectedPort + " detail").getResponse()
                        ?? throw new ArgumentNullException()).getPortData());
                    foreach (PortData data in infoListData)
                    {
                        if (data.portInfo != null)
                            basinfo.Items.Add(data);
                        else
                            advinfo.Items.Add(data);
                    }
                }
                catch { }
            }
        }

        private void InfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private async Task setAppCons(string input) 
        {
            AppConsole.Text = ConsoleLogic.ConsoleText;
        }
        public async Task con()
        {
            string outputbefore = "";
            while (true)
            {
                if (_OptionsChanged)
                {
                    if(dataFarEnd != "" && dataNearEnd != "")
                        ChartGraph();
                    _OptionsChanged = false;
                }
                if (ConsoleLogic.ConsoleText != "")
                {
                    gif.Visibility = Visibility.Visible;
                    XDSLStandart.Margin = new Thickness(77, 0, 0, 0);
                    send.IsEnabled = false;
                    LoadButton.IsEnabled = false;
                    SaveButton.IsEnabled = false;
                    PortBox.IsEnabled = false;
                    send.Content = "Processing";
                }
                else
                {
                    gif.Visibility = Visibility.Collapsed;
                    XDSLStandart.Margin = new Thickness(15, 0, 0, 0);
                    PortBox.IsEnabled = true;
                    LoadButton.IsEnabled = true;
                    SaveButton.IsEnabled = true;
                    send.Content = "Analyze";
                    send.IsEnabled = true;
                }

                if (ConsoleLogic.ConsoleText != outputbefore)
                {
                    await setAppCons(ConsoleLogic.ConsoleText);
                    outputbefore = ConsoleLogic.ConsoleText;
                }
                else
                    await Task.Delay(200);
            }
        }

        protected override void OnClosed(EventArgs e) 
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private async void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            SelectChartsUC selc = new SelectChartsUC(_graphSelector);
            OptionsBase opt = new OptionsBase(selc, _graphSelector);
            opt.Show();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFile svfl = new SaveFile(dataFarEnd, dataNearEnd);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            LoadFile lofl = new LoadFile();
            if (lofl.getState())
            {
                fromFile = true;
                string[] dataFile = lofl.getFarNearData();
                PortBox.Items.Insert(0, new PortData { portName = lofl.getFileName(), portState = @"Images\txt_file.png" });
                PortBox.SelectedIndex = 0;
                dataFarEnd = dataFile[0];
                dataNearEnd = dataFile[1];
                var itemAtOne = (PortData)PortBox.Items[1];
                if (itemAtOne.portName.Contains(".txt"))
                {
                    PortBox.Items.RemoveAt(1);
                }
                send.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private async void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelExport exc = await Task.Run(() => new ExcelExport(GraphYvalues, GraphXvalues, GraphListOfNames, _graphSelector));
            ConsoleLogic.ConsoleText = "0";
        }

        private void ChartAppearence_Click(object sender, RoutedEventArgs e)
        {
            ChartAppearenceUC chaUC = new ChartAppearenceUC();
            OptionsBase opt = new OptionsBase(chaUC, _graphSelector);
            opt.Show();
        }

        private void ExportMatlab_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
