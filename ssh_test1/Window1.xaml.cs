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
using LiveCharts;
using System.ComponentModel;

namespace ssh_test1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            try 
            {
                async Task Initialize() 
                {
                    List<PortData> listofobj = await Task.Run(() => new PortBoxLogic(new SendData
                       ("show xdsl operational-data line").getResponse() ?? throw new ArgumentNullException()).getPortDataCombo());
                    foreach (PortData obj in listofobj)
                    {
                        PortBox.Items.Add(obj);
                    }
                    string xdslStandartStr = await Task.Run(() => new SendData("show xdsl operational-data line | match match exact:gfast").getResponse());
                    if (!xdslStandartStr.Contains("up"))
                        Console.XDSLStandartS = "ADSL/VDSL";
                    else
                        Console.XDSLStandartS = "G-fast";
                }
                InitializeComponent();
                Initialize();
            }
            catch { }
        }

        List<List<int>> GraphYvalues = new List<List<int>>();
        List<List<int>> GraphXvalues = new List<List<int>>();
        List<string> GraphListOfNames = new List<string>();
        string dataFarEnd = "";
        string dataNearEnd = "";
        string selectedPort;

        private SolidColorBrush _BrushUpload = new SolidColorBrush(Colors.Red);
        public SolidColorBrush BrushUpload
        {
            get { return _BrushUpload; }
            set
            {
                _BrushUpload = value;
                infoTable.up = value;
                if (GraphField.HasItems)
                    send.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
        private SolidColorBrush _BrushDownload = new SolidColorBrush(Colors.Blue);
        public SolidColorBrush BrushDownload
        {
            get { return _BrushDownload; }
            set
            {
                _BrushDownload = value;
                infoTable.down = value;
                if (GraphField.HasItems)
                    send.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
        private List<bool> _graphSelector = new List<bool>() { true, true, true, true, true, true, true };
        public List<bool> graphSelector
        {
            get { return _graphSelector; }
            set
            {
                if (_graphSelector != value)
                {
                    _graphSelector = value;
                    if(GraphField.HasItems)
                        send.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }

        bool fromFile = false;

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
            ConsoleUC.ConsoleText = "3";
            GraphListOfNames.Clear();
            GraphField.Items.Clear();
            GraphYvalues.Clear();
            GraphXvalues.Clear();
            int realtimeInfo = 0;
            int charVindex = 0;
            if (!fromFile && String.IsNullOrEmpty(dataFarEnd) && String.IsNullOrEmpty(dataNearEnd) && !String.IsNullOrEmpty(infoTable.portIndex))
            {
                dataFarEnd = await Task.Run(() => new SendData("show xdsl carrier-data far-end " + infoTable.portIndex + " detail").getResponse());
                dataNearEnd = await Task.Run(() => new SendData("show xdsl carrier-data near-end " + infoTable.portIndex + " detail").getResponse());
                realtimeInfo = 1;
            }
            GraphLogic graphLog = await Task.Run(() => new GraphLogic(dataFarEnd, dataNearEnd, graphSelector));
            for (int i = 0; i < graphSelector.Count(); i++)
            {
                if (graphSelector[i])
                {
                    graphLog.SelectGraphNeeeded(i);
                    GraphField.Items.Add(new TabItem
                    {
                        Header = graphLog.name.Replace("-up", "").Replace("-down", "").Replace("-dn", ""),
                        Content = new ChartViewUC(graphLog.chartV[charVindex], graphLog.chartV[charVindex + 1], i, BrushUpload, BrushDownload),
                    });
                    GraphXvalues.Add(graphLog.chartV[charVindex].Xvals);
                    GraphXvalues.Add(graphLog.chartV[charVindex + 1].Xvals);
                    GraphYvalues.Add(graphLog.chartV[charVindex].Yvals);
                    GraphYvalues.Add(graphLog.chartV[charVindex + 1].Yvals);
                    GraphListOfNames.Add(graphLog.name);
                    if (realtimeInfo == 1)
                    {
                        try
                        {
                            infoTable.chartValuesCount = GraphXvalues[0].Count + GraphXvalues[1].Count;
                            infoTable.chartValuesUP = GraphXvalues[1].Count;
                            infoTable.realtime = true;
                        }
                        catch { }
                        realtimeInfo += 1;
                    }
                    charVindex += 2;
                }
            }
            ConsoleUC.ConsoleText = "0";
        }

        private void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GraphField.Items.Clear();
            dataFarEnd = "";
            dataNearEnd = "";
            if(GraphYvalues != null && GraphXvalues != null) 
            {
                GraphYvalues.Clear();
                GraphXvalues.Clear();
                GraphListOfNames.Clear();
            }
            if (fromFile && PortBox.SelectedIndex == 0) 
            {
                return;
            }
            var selected = (PortData)PortBox.SelectedItem;
            if (selected.portState.Contains("down.png"))
            {
                MessageBox.Show("Port you are trying to access is currently down. Values that are going to be displayed are values obtained by port's last showtime.");
                return;
            }
            else
            {
                infoTable.current_mode = "";
                infoTable.txPsdDOWN = "";
                if (fromFile) 
                {
                    PortBox.Items.RemoveAt(0);
                }
                fromFile = false;
                infoTable.portIndex = selected.portName.ToString();
            }
        }

        private void InfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
            ExcelExport exc = await Task.Run(() => new ExcelExport(GraphYvalues, GraphXvalues, GraphListOfNames, graphSelector));
            ConsoleUC.ConsoleText = "0";
        }

        private async void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsBase opt = new OptionsBase(0, this);
            opt.Show();
        }

        private void ChartAppearence_Click(object sender, RoutedEventArgs e)
        {
            OptionsBase opt = new OptionsBase(1, this);
            opt.Show();
        }

        private void ExportMatlab_Click(object sender, RoutedEventArgs e)
        {
            MatlabExport me = new MatlabExport(GraphXvalues, GraphYvalues, graphSelector, GraphListOfNames);
        }

        private void ConDetails_Click(object sender, RoutedEventArgs e)
        {
            //OptionsBase opt = new OptionsBase(2, graphSelector, BrushUpload, BrushDownload);
            //opt.ShowDialog();
            //BrushUpload = opt.brushUP;
            //BrushDownload = opt.brushDOWN;
            //graphSelector = opt.charts;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
