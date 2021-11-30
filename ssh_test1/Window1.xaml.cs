using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InteractiveDataDisplay.WPF;
using System.Linq;

namespace ssh_test1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        string selectedPort;
        public static List<bool> graphSelector;
        public static bool isOnline = false;
        public Window1()
        {
            try 
            {
                graphSelector = new List<bool>() { true, true, true, true, true, true, true };
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
            if(PortBox.SelectedItem == null) 
            {
                MessageBox.Show("Please select port you wish to analyze first");
            }
            //try
            //{
            int graphIndex = 0;
                GraphField.Items.Clear();
                GraphLogic graphLog = await Task.Run(() => new GraphLogic(
                    new SendData("show xdsl carrier-data far-end " + selectedPort + " detail").getResponse() 
                    ?? throw new ArgumentNullException(),
                    new SendData("show xdsl carrier-data near-end " + selectedPort + " detail").getResponse() 
                    ?? throw new ArgumentNullException(), graphSelector));
            for (int i = 0; i < graphSelector.Count(); i++)
            {
                if (graphSelector[i] == true)
                {
                    graphLog.SelectGraphNeeeded(i);
                    Chart chart = new Chart();
                    chart.Content = await Task.Run(()=> 
                    getChart(graphIndex, graphLog.getYValues() ?? throw new ArgumentNullException(), 
                    graphLog.getXValues() ?? throw new ArgumentNullException(), 
                    graphLog.getListOfNames() ?? throw new ArgumentNullException()));
                    GraphField.Items.Add(new TabItem
                    {
                        Header = "idk",
                        Content = chart
                    });
                    graphIndex += 2;
                }
            }
            //}
            //catch(Exception ex) { MessageBox.Show(ex.ToString()); }
            ConsoleLogic.ConsoleText = "0";
        }

        private Grid getChart(int i, List<List<int>> graphValuesY, List<List<int>> graphValuesX, List<string> graphName)
        {
            Grid grid = new Grid();
            Chart chart = new Chart();
            LineGraph lineGraphFar = new LineGraph()
            {
                Description = graphName[i],
                Stroke = new SolidColorBrush(Colors.Blue),
            };
            LineGraph lineGraphNear = new LineGraph()
            {
                Description = graphName[i + 1],
                Stroke = new SolidColorBrush(Colors.Red)
            };
            lineGraphFar.Plot(graphValuesX[i], graphValuesY[i]);
            lineGraphNear.Plot(graphValuesX[i + 1], graphValuesY[i + 1]);
            grid.Children.Add(lineGraphFar);
            grid.Children.Add(lineGraphNear);
            return grid;
        }

        private async void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (PortData)PortBox.SelectedItem;
            if (selected.portState.Contains("down.png"))
            {
                MessageBox.Show("Port you are trying to access is currently offline");
            }
            else
            {
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
                if (ConsoleLogic.ConsoleText != "")
                {
                    gif.Visibility = Visibility.Visible;
                    XDSLStandart.Margin = new Thickness(77, 0, 0, 0);
                    send.IsEnabled = false;
                    PortBox.IsEnabled = false;
                    send.Content = "Processing";
                }
                else
                {
                    gif.Visibility = Visibility.Collapsed;
                    XDSLStandart.Margin = new Thickness(15, 0, 0, 0);
                    PortBox.IsEnabled = true;
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

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow opt = new OptionsWindow(graphSelector);
            opt.Show();
        }
    }
}
