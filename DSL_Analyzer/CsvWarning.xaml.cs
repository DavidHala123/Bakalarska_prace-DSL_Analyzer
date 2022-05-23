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
    /// Interaction logic for CsvWarning.xaml
    /// </summary>
    public partial class CsvWarning : Window
    {
        public bool showCsvExport = false;
        public CsvWarning()
        {
            InitializeComponent();
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Ok_click(object sender, RoutedEventArgs e)
        {
            showCsvExport = true;
            this.Close();
        }
    }
}
