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
using System.IO;

namespace DSL_Analyzer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GraphLogic graphLog;
        InfoNotAvailable infoNot = new InfoNotAvailable();
        VarData varData = new VarData();
        //ON WINDOW OPEN RETRIEVE INFORMATION ABOUT PORTS
        public MainWindow()
        {
            varData.hzCons = 1;
            varData.conChanged = false;
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

        //CHECKS IF CONNECTION HAS CHANGED. IF YES, IT UPDATES THE WINDOW
        public bool conChanged 
        {
            set 
            {
                varData.conChanged = value;
                if(value == true) 
                {
                    PortBox.SelectedIndex = -1;
                    reload.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    GraphField.Items.Clear();
                    infoTable.suppm_value.Items.Clear();
                    infoTable.txPsdDOWN = "";
                    varData.conChanged = false;
                }

            }
        }

        //HOLDS INFORMATION OF PLOT COLOR
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

        //HOLD INFORMATION OF WHICH CHARTS ARE SELECTED TO BE PROCESSED
        private List<bool> _graphSelector = new List<bool>() { true, true, true, true, true, true, true };
        public List<bool> graphSelector
        {
            get { return _graphSelector; }
            set
            {
                if (_graphSelector != value)
                {
                    _graphSelector = value;
                    selectionChanged = true;
                    if(GraphField.HasItems)
                        send.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }
        private bool selectionChanged = false;
        private bool _fromFile = false;

        //HOLDS INFORMATION OF WHETER WAS DATA RETRIEVED FROM FILE OR NOT
        public bool fromFile 
        {
            get { return _fromFile; }
            set 
            {
                _fromFile = value;
            } 
        }
        private bool isGfast { get; set; }
        private bool _hz = false;
        private List<PortData> listofobj = new List<PortData>();

        private void command_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void GraphField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //RETRIEVE INFORMATION ABOUT PORTS, FINDS OUT WHAT TECHNOLOGIES ARE SUPPORTED BY DSLAM AND HOW MANY CHARTS ARE POSSIBLE,
        //SETS FROMFILE BOOL TO T/F
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
            {
                Console.XDSLStandartS = "ADSL/VDSL";
                if (_graphSelector.Count > 7)
                {
                    _graphSelector.RemoveAt(9);
                    _graphSelector.RemoveAt(8);
                    _graphSelector.RemoveAt(7);
                }
                isGfast = false;
            }
            else 
            {
                Console.XDSLStandartS = "G-fast";
                if (_graphSelector.Count == 7)
                {
                    _graphSelector.Add(true);
                    _graphSelector.Add(true);
                    _graphSelector.Add(true);
                }
                isGfast = true;
            }
            if (previouslySelected != -1)
                PortBox.SelectedIndex = previouslySelected;
        }

        //PROCESSES ALL CHARTS THAT HAS TRUE VALUE IN GRAPHSELECTOR
        private async void send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConsoleUC.ConsoleText = "3";
                infoTable.fromConfig = false;
                GraphField.Items.Clear();
                int charVindex = 0;
                if (!fromFile && !selectionChanged) //IF THERES NO FILE AND GRAPH SELECTOR DOESNT CHANGED --> RETRIEVE DATA FROM DSLAM
                {
                    varData.dataFarEnd = await Task.Run(() => new SendData("show xdsl carrier-data far-end " + infoTable.portIndex + " detail").getResponse());
                    varData.dataNearEnd = await Task.Run(() => new SendData("show xdsl carrier-data near-end " + infoTable.portIndex + " detail").getResponse());
                }
                else  //ELSE GET THE DATA FROM ALREADY SAVED DATA
                {
                    infoTable.generalInfo = varData.generalInfo;
                    infoTable.rtInfo = varData.rtInfo;
                }
                infoTable.fromfile = fromFile;
                graphLog = await Task.Run(() => new GraphLogic(varData.dataFarEnd, varData.dataNearEnd, graphSelector, infoTable.current_mode, _hz));
                varData.hzCons = graphLog.hzConstant;
                for (int i = 0; i < graphSelector.Count(); i++) //ITERATE THROUGH EVERY POSITION IN GRAPH SELECTOR AND PROCESSES ONLY TRUE VALUES
                {
                    if (graphSelector[i])
                    {
                        await Task.Run(() => graphLog.SelectGraphNeeeded(i, isGfast));
                        if (graphLog.chartV[charVindex].Xvals.Count > 0 || graphLog.chartV[charVindex + 1].Xvals.Count > 0)
                        {
                            GraphField.Items.Add(new TabItem
                            {
                                Header = graphLog.name.Replace("-up", "").Replace("-down", "").Replace("-dn", ""),
                                Content = new ChartViewUC(graphLog.chartV[charVindex], graphLog.chartV[charVindex + 1], i, BrushUpload, BrushDownload, infoTable.current_mode, varData.hzCons),
                            });
                        }
                        if (charVindex == 0 && !selectionChanged)
                            await Task.Run(() => getAttainableBitrate());
                        charVindex += 2;
                    }
                }
                selectionChanged = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                GraphField.Items.Clear();
                MessageBox.Show("Something went wrong.");
            }
            ConsoleUC.ConsoleText = "0";
            infoTable.realtime = false;
        }

        //FINDS OUT WHETER IS BITRATE SET AS STATIC, OTHERWISE IT CALCULATES BITRATE AND SETS FLAG TO START RETRIEVING
        //ADDITIONAL INFORMATION FROM DSLAM
        private void getAttainableBitrate() 
        {
            if(!fromFile)
                infoTable.realtime = true;
            if (File.Exists(@"Config\Bitrate.txt"))
            {
                var fileLines = File.ReadAllLines(@"Config\Bitrate.txt");
                foreach (string line in fileLines)
                {
                    string[] values = line.Split(';');
                    if (values[0] == infoTable.current_mode)
                    {
                        infoTable.attaBitrateUP = Int32.Parse(values[1]);
                        infoTable.attaBitrateDOWN = Int32.Parse(values[2]);
                        infoTable.fromConfig = true;
                    }
                }
            }
            if (!infoTable.fromConfig)
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
                        graphLog.SelectGraphNeeeded(0, isGfast);
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
            }
            infoTable.chartValuesCount = graphLog.chartV[0].Xvals.Count() + graphLog.chartV[1].Xvals.Count;
            infoTable.chartValuesUP = graphLog.chartV[0].Xvals.Count;
        }

        //CHECKS IF PORTS ARE ONLINE OR OFFLINE
        private void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (infoGrid.Children.Count >= 2)
                infoGrid.Children.Remove(infoNot);
            if (PortBox.SelectedIndex >= 0) 
            {
                GraphField.Items.Clear();
                varData.dataFarEnd = String.Empty;
                varData.dataNearEnd = String.Empty;
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

        //OPENS SAVE FILE DIALOGUE
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFile svfl = new SaveFile(varData.dataFarEnd, varData.dataNearEnd, infoTable.generalInfo, infoTable.rtInfo);
        }

        //OPENS OPEN FILE DIALOGUE AND RETRIEVES DATA FROM FILE IF IT WAS SELECTED, START PROCESSING OF CHARTS FROM FILE
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            LoadFile lofl = new LoadFile();
            if (lofl.getState())
            {
                fromFile = true;
                string[] dataFile = lofl.getFarNearData();
                PortBox.Items.Insert(0, new PortData { portName = lofl.getFileName(), portState = @"Images\txt_file.png" });
                PortBox.SelectedIndex = 0;
                varData.generalInfo = dataFile[0];
                varData.rtInfo = dataFile[1];
                varData.dataFarEnd = dataFile[2];
                varData.dataNearEnd = dataFile[3];
                var itemAtOne = (PortData)PortBox.Items[1];
                if (itemAtOne.portName.Contains(".txt"))
                {
                    PortBox.Items.RemoveAt(1);
                }
                if (String.IsNullOrEmpty(varData.generalInfo) || String.IsNullOrEmpty(varData.rtInfo))
                {
                    infoTable.suppm_value.Items.Clear();
                    infoTable.txPsdDOWN = "";
                    if (infoGrid.Children.Count < 2)
                        infoGrid.Children.Add(infoNot);
                }
                send.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        //EXPORTS DATA TO EXCEL
        private async void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelExport exc = await Task.Run(() => new ExcelExport(graphLog.chartV, graphSelector, varData.hzCons));
            ConsoleUC.ConsoleText = "0";
        }

        //OPENS OPTIONS BASE AND LET USER TO SELECT CHARTS TO PROCESS
        private async void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsBase opt = new OptionsBase(0, this, Console.XDSLStandartS);
            opt.Show();
        }

        //OPENS OPTIONS BASE AND LETS USER TO SELECT COLORS OF PLOTS
        private void ChartAppearence_Click(object sender, RoutedEventArgs e)
        {
            OptionsBase opt = new OptionsBase(1, this, Console.XDSLStandartS);
            opt.Show();
        }

        //EXPORTS DATA TO MATLAB
        private async void ExportMatlab_Click(object sender, RoutedEventArgs e)
        {
            MatlabExport me = await Task.Run(() => new MatlabExport(graphLog.chartV, graphSelector, varData.hzCons));
        }

        //EXPORTS DATA TO CSV
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

        //OPENS OPTIONS BASE AND LETS USER TO CHECK/CHANGE CONNECTION
        private void ConDetails_Click(object sender, RoutedEventArgs e)
        {
            OptionsBase optb = new OptionsBase(2, this, Console.XDSLStandartS);
            optb.Show();
        }

        //OPENS OPTION BASE AND LETS USER TO SET STATIC VALUES OF BITRATE
        private void setStaticBit_Click(object sender, RoutedEventArgs e)
        {
            OptionsBase optb = new OptionsBase(3, this, Console.XDSLStandartS);
            optb.Show();
        }

        //CLOSES THE APPLICATION
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void hzCarBt_Click(object sender, RoutedEventArgs e)
        {
            _hz = !_hz;
            if (!_hz)
                varData.hzCons = 1;
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
