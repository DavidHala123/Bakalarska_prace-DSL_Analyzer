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
                _graphSelector = new List<bool>() { true, true, true, true, true, true, true };
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

        List<List<int>> GraphYvalues;
        List<List<int>> GraphXvalues;
        List<string> GraphListOfNames;
        string dataFarEnd = "";
        string dataNearEnd = "";
        string selectedPort;

        private static SolidColorBrush _BrushUpload = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush BrushUpload
        {
            get { return _BrushUpload; }
            set
            {
                if (_BrushUpload != value)
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
                if (_graphSelector != value)
                {
                    _graphSelector = value;
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
            GraphField.Items.Clear();
            int charVindex = 0;
            if (!fromFile)
            {
                dataFarEnd = await Task.Run(() => new SendData("show xdsl carrier-data far-end " + infoTable.portIndex + " detail").getResponse());
                dataNearEnd = await Task.Run(() => new SendData("show xdsl carrier-data near-end " + infoTable.portIndex + " detail").getResponse());
            }
            GraphLogic graphLog = await Task.Run(() => new GraphLogic(dataFarEnd, dataNearEnd, Window1.graphSelector));
            for (int i = 0; i < graphSelector.Count(); i++)
            {
                graphLog.SelectGraphNeeeded(i);
                GraphField.Items.Add(new TabItem
                {
                    Header = graphLog.name,
                    Content = new ChartViewUC(graphLog.chartV[charVindex], graphLog.chartV[charVindex + 1], i),
                });
                charVindex += 2;
                if (i==0 && !fromFile)
                {
                    infoTable.chartValuesDOWN = new ChartValues<int>(new[] { graphLog.chartV[0].Xvals.Count() });
                    infoTable.chartValuesUP = new ChartValues<int>(new[] { graphLog.chartV[1].Xvals.Count() });
                    infoTable.realtime = true;
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
                infoTable.portIndex = selected.portName.ToString();
            }
        }

        private void InfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            object window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
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
            ConsoleUC.ConsoleText = "0";
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
        private void ConDetails_Click(object sender, RoutedEventArgs e)
        {
            ConnectionUC conUC = new ConnectionUC();
            OptionsBase opt = new OptionsBase(conUC, _graphSelector);
            opt.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
