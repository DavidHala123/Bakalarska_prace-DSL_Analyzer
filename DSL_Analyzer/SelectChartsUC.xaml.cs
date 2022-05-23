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

namespace DSL_Analyzer
{
    /// <summary>
    /// Interaction logic for SelectChartsUC.xaml
    /// </summary>
    public partial class SelectChartsUC : UserControl
    {
        Window1 wind;
        OptionsBase optb;
        private string mode;
        private List<bool> listOfChecks;
        public SelectChartsUC(Window1 wind, OptionsBase optb, string mode)
        {
            this.optb = optb;
            this.wind = wind;
            this.mode = mode;
            listOfChecks = wind.graphSelector;
            InitializeComponent();
            LoadDistribution.IsChecked = listOfChecks[0];
            GainAllocation.IsChecked = listOfChecks[1];
            Snr.IsChecked = listOfChecks[2];
            if(mode == "G-fast")
                Qln.IsChecked = listOfChecks[8];
            else
                Qln.IsChecked = listOfChecks[3];
            FuncComplex.IsChecked = listOfChecks[4];
            FuncReal.IsChecked = listOfChecks[5];
            TxPsd.IsChecked = listOfChecks[6];
            if(mode != "G-fast") 
            {
                mcr_carr.IsEnabled = false;
                aln.IsEnabled = false;
            }
            else 
            {
                mcr_carr.IsChecked = listOfChecks[7];
                aln.IsChecked = listOfChecks[9];
            }
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if(mode == "G-fast") 
            {
                wind.graphSelector = new List<bool>() { LoadDistribution.IsChecked ?? false, GainAllocation.IsChecked ?? false,
                    Snr.IsChecked ?? false, false, FuncComplex.IsChecked ?? false, FuncReal.IsChecked ?? false, 
                    TxPsd.IsChecked ?? false, mcr_carr.IsChecked ?? false , Qln.IsChecked ?? false, 
                    aln.IsChecked ?? false };
            }
            else 
            {
                wind.graphSelector = new List<bool>() { LoadDistribution.IsChecked ?? false, GainAllocation.IsChecked ?? false,
                    Snr.IsChecked ?? false, Qln.IsChecked ?? false, FuncComplex.IsChecked ?? false,
                    FuncReal.IsChecked ?? false, TxPsd.IsChecked ?? false };
            }
            optb.Close();
        }
    }
}
