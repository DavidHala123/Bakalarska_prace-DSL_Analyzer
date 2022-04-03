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
    /// Interaction logic for SelectChartsUC.xaml
    /// </summary>
    public partial class SelectChartsUC : UserControl
    {
        String[] outputString = new String[7];
        object window;
        public SelectChartsUC(List<bool> listOfChecks)
        {
            InitializeComponent();
            LoadDistribution.IsChecked = listOfChecks[0];
            GainAllocation.IsChecked = listOfChecks[1];
            Snr.IsChecked = listOfChecks[2];
            Qln.IsChecked = listOfChecks[3];
            FuncComplex.IsChecked = listOfChecks[4];
            FuncReal.IsChecked = listOfChecks[5];
            TxPsd.IsChecked = listOfChecks[6];
            this.window = window;
        }

        private void LoadDistribution_Checked(object sender, RoutedEventArgs e)
        {
            outputString[0] = "1";
        }

        private void GainAllocation_Checked(object sender, RoutedEventArgs e)
        {
            outputString[1] = "1";
        }

        private void Snr_Checked(object sender, RoutedEventArgs e)
        {
            outputString[2] = "1";
        }
        private void Qln_Checked(object sender, RoutedEventArgs e)
        {
            outputString[3] = "1";
        }

        private void FuncComplex_Checked(object sender, RoutedEventArgs e)
        {
            outputString[4] = "1";
        }

        private void FuncReal_Checked(object sender, RoutedEventArgs e)
        {
            outputString[5] = "1";
        }

        private void TxPsd_Checked(object sender, RoutedEventArgs e)
        {
            outputString[6] = "1";
        }

        private void LoadDistribution_Unchecked(object sender, RoutedEventArgs e)
        {
            outputString[0] = "0";
        }

        private void GainAllocation_Unchecked(object sender, RoutedEventArgs e)
        {
            outputString[1] = "0";
        }

        private void Snr_Unchecked(object sender, RoutedEventArgs e)
        {
            outputString[2] = "0";
        }
        private void Qln_Unchecked(object sender, RoutedEventArgs e)
        {
            outputString[3] = "0";
        }

        private void FuncComplex_Unchecked(object sender, RoutedEventArgs e)
        {
            outputString[4] = "0";
        }

        private void FuncReal_Unchecked(object sender, RoutedEventArgs e)
        {
            outputString[5] = "0";
        }

        private void TxPsd_Unchecked(object sender, RoutedEventArgs e)
        {
            outputString[6] = "0";
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            List<bool> output = new List<bool>();
            for (int i = 0; i <= 6; i++)
            {
                if (outputString[i] != "1")
                    output.Add(false);
                else
                    output.Add(true);
            }
            Window1.graphSelector = output;
            OptionsBase.OptionsChanged = true;
        }
    }
}
