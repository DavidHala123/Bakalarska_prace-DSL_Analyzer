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

namespace DSL_Analyzer
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
        private string mode;
        Window1 wind;

        public OptionsBase(int choice, Window1 wind, string mode)
        {
            this.mode = mode;
            this.wind = wind;
            InitializeComponent();
            switch (choice) 
            {
                case 0:
                    SelChart.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case 1:
                    CharAppe.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case 2:
                    Con.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
                case 3:
                    staticBitr.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    break;
            }
        }
        private void SelChart_Click(object sender, RoutedEventArgs e)
        {
            optCont.Children.Clear();
            SelectChartsUC selc = new SelectChartsUC(wind, this, mode);
            optCont.Children.Add(selc);
        }



        private void CharAppe_Click(object sender, RoutedEventArgs e)
        {
            optCont.Children.Clear();
            ChartAppearenceUC selc = new ChartAppearenceUC(wind, this);
            optCont.Children.Add(selc);
        }

        private void conClick(object sender, RoutedEventArgs e)
        {
            optCont.Children.Clear();
            ConnectionUC conuc = new ConnectionUC(wind, this);
            optCont.Children.Add(conuc);
        }

        private void staticBitrClick(object sender, RoutedEventArgs e)
        {
            optCont.Children.Clear();
            SetStaticBitrate setbr = new SetStaticBitrate(this);
            optCont.Children.Add(setbr);
        }
    }
}
