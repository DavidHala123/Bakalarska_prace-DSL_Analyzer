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
        ConsoleLogic consoleLog;
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
            try
            {
                GraphLogic dataFarEnd = new GraphLogic();
                GraphLogic dataNearEnd = new GraphLogic();
                List<List<int>> dataFarEndList = await Task.Run(() => dataFarEnd.getGraphDecValues(new SendData("show xdsl carrier-data far-end " + selectedPort + " detail").getResponse()
                        ?? throw new ArgumentNullException(), graphSelector));
                List<List<int>> dataNearEndList = await Task.Run(() => dataNearEnd.getGraphDecValues(new SendData("show xdsl carrier-data near-end " + selectedPort + " detail").getResponse()
                        ?? throw new ArgumentNullException(), graphSelector));
                List<string> namesFarEnd = dataFarEnd.getName();
                List<string> namesNearEnd = dataNearEnd.getName();
                GraphField.Items.Clear();
                for (int i = 0; i <= 6; i++)
                {
                    Grid graphGrid = new Grid();
                    var yFar = dataFarEndList[i];
                    var xFar = Enumerable.Range(0, yFar.Count()).Select(i => i).ToArray();
                    var yNear = dataNearEndList[i];
                    var xNear = Enumerable.Range(xFar.Count(), yNear.Count()).Select(i => i).ToArray();
                    Chart chart = new Chart();
                    LineGraph graphFar = new LineGraph()
                    {
                        Stroke = new SolidColorBrush(Colors.Blue),
                        Description = namesFarEnd[i],
                        StrokeThickness = 1,
                    };
                    LineGraph graphNear = new LineGraph()
                    {
                        Stroke = new SolidColorBrush(Colors.Red),
                        Description = namesNearEnd[i],
                        StrokeThickness = 1,
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
                            Header = namesFarEnd[i].Replace("-up", "").Replace("-down", "") + " [" + selectedPort + "]",
                            Content = chart
                        });
                    }
                    catch { continue; }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.ToString()); }
            send.IsEnabled = true;
            PortBox.IsEnabled = true;
            send.Content = "Analyze";
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
                    send.IsEnabled = true;
                    PortBox.IsEnabled = true;
                    send.Content = "Analyze";
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
