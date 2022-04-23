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
        GraphLogic graphLog;
        InfoNotAvailable infoNot = new InfoNotAvailable();
        public Window1()
        {
            InitializeComponent();
            try 
            {
                getPortInfo();
            }
            catch 
            {
                MessageBox.Show("Unable to load port information. Please check your connection to the DSLAM");
            }
        }
        string dataFarEnd = "";
        string dataNearEnd = "";
        private bool _conChanged = false;
        public bool conChanged 
        {
            set 
            {
                _conChanged = value;
                if(value == true) 
                {
                    PortBox.SelectedIndex = -1;
                    reload.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    _conChanged = false;
                }

            }
        }

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

        private bool _fromFile = false;

        public bool fromFile 
        {
            get { return _fromFile; }
            set 
            {
                _fromFile = value;
                if(value == true) 
                {
                    infoTable.suppm_value.Items.Clear();
                    infoTable.txPsdDOWN = "";
                    if(infoGrid.Children.Count < 2)
                        infoGrid.Children.Add(infoNot);
                }
                else 
                {
                    infoGrid.Children.Remove(infoNot);
                }
            } 
        }

        private bool _hz = false;
        private List<PortData> listofobj = new List<PortData>();

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

        private async Task getPortInfo()
        {
            int previouslySelected = -1;
            if (PortBox.SelectedIndex >= 0) 
            {
                previouslySelected = PortBox.SelectedIndex;
                PortBox.SelectedIndex = -1;
            }
            if (PortBox.Items.Count > 0)
                PortBox.Items.Clear();
            listofobj = await Task.Run(() => new PortBoxLogic(new SendData
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
            if (previouslySelected != -1)
                PortBox.SelectedIndex = previouslySelected;
        }

        private async void send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConsoleUC.ConsoleText = "3";
                GraphField.Items.Clear();
                int realtimeInfo = 0;
                int charVindex = 0;
                if (!fromFile)
                {
                    dataFarEnd = await Task.Run(() => new SendData("show xdsl carrier-data far-end " + infoTable.portIndex + " detail").getResponse());
                    dataNearEnd = await Task.Run(() => new SendData("show xdsl carrier-data near-end " + infoTable.portIndex + " detail").getResponse());
                    realtimeInfo = 1;
                }
                graphLog = await Task.Run(() => new GraphLogic(dataFarEnd, dataNearEnd, graphSelector, infoTable.current_mode, _hz));
                for (int i = 0; i < graphSelector.Count(); i++)
                {
                    if (graphSelector[i])
                    {
                        graphLog.SelectGraphNeeeded(i);
                        GraphField.Items.Add(new TabItem
                        {
                            Header = graphLog.name.Replace("-up", "").Replace("-down", "").Replace("-dn", ""),
                            Content = new ChartViewUC(graphLog.chartV[charVindex], graphLog.chartV[charVindex + 1], i, BrushUpload, BrushDownload, infoTable.current_mode, _hz),
                        });
                        charVindex += 2;
                    }
                }
                if (realtimeInfo == 1)
                {
                    int maxBitUP = 0;
                    int maxBitDOWN = 0;
                    if (infoTable.current_mode == "gfast")
                    {
                        infoTable.attaBitrateUP = Convert.ToInt32((15 * graphLog.chartV[0].Xvals.Count() * 12 * 4312.5) / 7000000);
                        infoTable.attaBitrateDOWN = Convert.ToInt32((15 * graphLog.chartV[0].Xvals.Count() * 12 * 4312.5 * 6) / 7000000);
                    }
                    else
                    {
                        if (!graphSelector[0])
                        {
                            graphLog.SelectGraphNeeeded(0);
                            foreach (int integer in graphLog.chartV[graphLog.chartV.Count - 2].Yvals)
                            {
                                maxBitUP += integer;
                            }
                            foreach (int integer in graphLog.chartV[graphLog.chartV.Count - 1].Yvals)
                            {
                                maxBitDOWN += integer;
                            }
                        }
                        else
                        {
                            foreach (int integer in graphLog.chartV[0].Yvals)
                            {
                                maxBitUP += integer;
                            }
                            foreach (int integer in graphLog.chartV[1].Yvals)
                            {
                                maxBitDOWN += integer;
                            }
                        }
                        infoTable.attaBitrateUP = (maxBitUP * 4000) / 1000000;
                        infoTable.attaBitrateDOWN = (maxBitDOWN * 4000) / 1000000;
                    }
                    infoTable.chartValuesCount = graphLog.chartV[0].Xvals.Count() + graphLog.chartV[1].Xvals.Count;
                    infoTable.chartValuesUP = graphLog.chartV[0].Xvals.Count;
                    infoTable.realtime = true;

                    realtimeInfo += 1;
                }
            }
            catch
            {
                GraphField.Items.Clear();
                MessageBox.Show("Something went wrong.");
            }
            ConsoleUC.ConsoleText = "0";
            infoTable.realtime = false;
        }

        private void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(PortBox.SelectedIndex >= 0) 
            {
                GraphField.Items.Clear();
                dataFarEnd = "";
                dataNearEnd = "";
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
                    infoTable.suppm_value.Items.Clear();
                    infoTable.txPsdDOWN = "";
                    if (fromFile)
                    {
                        PortBox.Items.RemoveAt(0);
                    }
                    fromFile = false;
                    infoTable.portIndex = selected.portName.ToString();
                }
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
            ExcelExport exc = await Task.Run(() => new ExcelExport(graphLog.chartV, graphSelector));
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
            MatlabExport me = new MatlabExport(graphLog.chartV, graphSelector);
        }

        private void ExportCSV_Click(object sender, RoutedEventArgs e)
        {
            if(graphSelector.Where(c => c).Count() > 1) 
            {
                CsvWarning csvw = new CsvWarning();
                csvw.ShowDialog();
                if (csvw.showCsvExport)
                {
                    CsvExport csve = new CsvExport(graphLog.chartV, graphSelector);
                }
            }
            else 
            {
                CsvExport csve = new CsvExport(graphLog.chartV, graphSelector);
            }
        }

        private void ConDetails_Click(object sender, RoutedEventArgs e)
        {
            OptionsBase optb = new OptionsBase(2, this);
            optb.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void hzCarBt_Click(object sender, RoutedEventArgs e)
        {
            _hz = !_hz;
            hzCarBt.Content = _hz ? "Hertz" : "Carriers";
            if(GraphField.HasItems)
                send.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void reload_Click(object sender, RoutedEventArgs e)
        {
            getPortInfo();
        }
    }
}
