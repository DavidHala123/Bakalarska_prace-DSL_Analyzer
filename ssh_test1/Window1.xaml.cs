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
        public Window1()
        {
            try 
            {
                graphSelector = new List<bool>() { true, true, true, true, true, true, true };
                async Task Initialize() 
                {
                    List<PortData> listofobj = await Task.Run(() => sendit.FillComboBox(sendit.sendCommandGetResponse
                       ("show xdsl operational-data line") ?? throw new ArgumentNullException()));
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

                for (int i = 0; i <= 6; i++)
                {
                    if (graphSelector[i] == true)
                    {
                        if (graphSelector[i] == true)
                        {
                            Chart chart = new Chart();
                            LineGraph graph = new LineGraph();
                            var y = (await Task.Run(() => sendit.getGraph(sendit.SelectData(sendit.indexes[i],
                                                    sendit.indexes[i + 1], sendCommandOutput))));
                            var x = Enumerable.Range(0, y.Count()).Select(i => i).ToArray();
                            graph.Plot(x, y);
                            graph.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString("#76EB7E");
                            graph.StrokeThickness = 2;
                            chart.Content = graph;
                            GraphField.Items.Add(new TabItem
                            {
                                Header = selectedPort,
                                Content = chart
                            });
                        }

                    }
                }
            }
            catch { }
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
                    List<PortData> infoListData = await Task.Run(() => sendit.FillInfoTable(selectedPort,
                    sendit.sendCommandGetResponse("show xdsl operational-data far-end line " + selectedPort + " detail")
                    ?? throw new ArgumentNullException(),
                        sendit.sendCommandGetResponse("show xdsl operational-data near-end line " + selectedPort + " detail")
                        ?? throw new ArgumentNullException(),
                        sendit.sendCommandGetResponse("show xdsl operational-data line " + selectedPort + " detail")
                        ?? throw new ArgumentNullException()));
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
            string output = "";
            while (true)
            {
                output = sendit.setConsoleText();

                if (output != "") 
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

                if (output != outputbefore)
                {
                    await setAppCons(output);
                    outputbefore = output;
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
