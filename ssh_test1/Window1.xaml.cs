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
        SendData sendit = new SendData();
        ConsoleLogic consoleLog;
        public Window1()
        {
            try 
            {
                graphSelector = new List<bool>() { true, true, true, true, true, true, true };
                async Task Initialize() 
                {
                    List<PortData> listofobj = await Task.Run(() => new PortBoxLogic(sendit.sendCommandGetResponse
                       ("show xdsl operational-data line") ?? throw new ArgumentNullException()).getPortDataCombo());
                    foreach (PortData obj in listofobj)
                    {
                        PortBox.Items.Add(obj);
                    }
                    string xdslStandartStr = await Task.Run(() => sendit.sendCommandGetResponse("show xdsl operational-data line | match match exact:gfast"));
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
            try
            {
                sendit.graphRequired = true;
                GraphField.Items.Clear();
                string sendCommandOutput = await Task.Run(() =>
                                        sendit.sendCommandGetResponse("show xdsl carrier-data far-end " + selectedPort + " detail")
                                        ?? throw new ArgumentNullException());
                string sendCommandOutput2 = await Task.Run(() => sendit.sendCommandGetResponse("show xdsl carrier-data near-end " + selectedPort + " detail") 
                                        ?? throw new ArgumentException());

                for (int i = 0; i <= 6; i++)
                {
                    if (graphSelector[i] == true)
                    {
                        if (graphSelector[i] == true)
                        {
                            GraphLogic dataFarEnd  = await Task.Run(() => new GraphLogic(sendCommandOutput, i));
                            GraphLogic dataNearEnd = await Task.Run(() => new GraphLogic(sendCommandOutput2, i));
                            var yFar = dataFarEnd.getGraphDecValues();
                            var xFar = Enumerable.Range(0, yFar.Count()).Select(i => i).ToArray();
                            var yNear = dataNearEnd.getGraphDecValues();
                            var xNear = Enumerable.Range(xFar.Count(), yNear.Count()).Select(i => i).ToArray();
                            Chart chart = new Chart();
                            Grid graphGrid = new Grid();
                            LineGraph graphFar = new LineGraph() 
                            {
                                Stroke = new SolidColorBrush(Colors.Blue),
                                Description = dataFarEnd.getName(),
                                StrokeThickness = 2,
                            };
                            LineGraph graphNear = new LineGraph() 
                            {
                                Stroke = new SolidColorBrush(Colors.Red),
                                Description = dataNearEnd.getName(),
                                StrokeThickness = 2,
                            };
                            graphFar.Plot(xFar, yFar);
                            graphNear.Plot(xNear, yNear);
                            graphGrid.Children.Add(graphFar);
                            graphGrid.Children.Add(graphNear);
                            chart.Content = graphGrid;
                            try
                            {
                                GraphField.Items.Add(new TabItem
                                {
                                    Header = dataFarEnd.getName().Replace("-up", "").Replace("-down", "") + " [" + selectedPort + "]",
                                    Content = chart
                                });
                            }
                            catch { continue; }
                        }

                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.ToString()); }
            send.IsEnabled = true;
            PortBox.IsEnabled = true;
            send.Content = "Analyze";
            sendit.graphRequired = false;
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
                    sendit.sendCommandGetResponse("show xdsl operational-data far-end line " + selectedPort + " detail")
                    ?? throw new ArgumentNullException(),
                        sendit.sendCommandGetResponse("show xdsl operational-data near-end line " + selectedPort + " detail")
                        ?? throw new ArgumentNullException(),
                        sendit.sendCommandGetResponse("show xdsl operational-data line " + selectedPort + " detail")
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
            AppConsole.Text = input;
        }
        private async Task con()
        {
            string outputbefore = "";
            //string output = "";
            while (true)
            {
                consoleLog = new ConsoleLogic(sendit.progressionInfo, sendit.lengthNow);

                if (consoleLog.ConsoleText != "") 
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
                    send.IsEnabled = true;
                    PortBox.IsEnabled = true;
                    send.Content = "Analyze";
                }

                if (consoleLog.ConsoleText != outputbefore)
                {
                    await setAppCons(consoleLog.ConsoleText);
                    outputbefore = consoleLog.ConsoleText;
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
