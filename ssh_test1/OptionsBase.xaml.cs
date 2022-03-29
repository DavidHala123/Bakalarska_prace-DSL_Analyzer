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
using System.Windows.Shapes;

namespace ssh_test1
{
    /// <summary>
    /// Interaction logic for OptionsBase.xaml
    /// </summary>
    public partial class OptionsBase : Window
    {
        private List<bool> charts;
        public OptionsBase(UIElement UC, List<bool> listofCharts)
        {
            InitializeComponent();
            this.charts = listofCharts;
            openUC(UC);
        }
        private UIElement UC;
        private void SelChart_Click(object sender, RoutedEventArgs e)
        {
            UC = new SelectChartsUC(charts);
            openUC(UC);
        }

        private void openUC(UIElement inputUC) 
        {
            optCont.Children.Clear();
            optCont.Children.Add(inputUC);
        }

        private void CharAppe_Click(object sender, RoutedEventArgs e)
        {
            UC = new ChartAppearenceUC();
            openUC(UC);
        }
    }
}
