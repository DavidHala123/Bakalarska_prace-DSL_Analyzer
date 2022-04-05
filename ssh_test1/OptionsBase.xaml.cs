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
        SelectChartsUC selc;
        private List<bool> _charts;
        public  List<bool> charts 
        {
            get { return _charts; }
            set 
            {
                if(charts != value) 
                {
                    charts = value;
                }
            }
        }
        private SolidColorBrush _brushUP;
        public SolidColorBrush brushUP 
        {
            get { return _brushUP; }
            set 
            {
                if(brushUP != value) 
                {
                    brushUP = value;
                }
            }
        }

        private SolidColorBrush _brushDOWN;
        public SolidColorBrush brushDOWN 
        {
            get { return _brushDOWN; }
            set 
            {
                if( brushDOWN != value) 
                {
                    brushDOWN = value;
                }
            }
        }
        Window1 wind;

        public OptionsBase(int choice, Window1 wind)
        {
            this.wind = wind;
            InitializeComponent();
            MessageBox.Show(choice.ToString());
            switch (choice) 
            {
                case 0:
                    SelChart.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case 1:
                    CharAppe.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case 2:
                    break;
            }
        }
        private void SelChart_Click(object sender, RoutedEventArgs e)
        {
            optCont.Children.Clear();
            SelectChartsUC selc = new SelectChartsUC(wind, this);
            optCont.Children.Add(selc);
        }



        private void CharAppe_Click(object sender, RoutedEventArgs e)
        {
            optCont.Children.Clear();
            ChartAppearenceUC selc = new ChartAppearenceUC(wind, this);
            optCont.Children.Add(selc);
        }
    }
}
