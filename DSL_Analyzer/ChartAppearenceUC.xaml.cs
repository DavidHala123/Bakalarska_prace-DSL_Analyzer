using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for ChartAppearenceUC.xaml
    /// </summary>
    public partial class ChartAppearenceUC : INotifyPropertyChanged
    {
        public ChartAppearenceUC(Window1 wind, OptionsBase optb)
        {
            this.optb = optb;
            this.wind = wind;
            DataContext = this;
            InitializeComponent();
        }
        OptionsBase optb;
        Window1 wind;
        private bool setUpb = false;
        private bool setDownb = false;
        private int _Rval;
        public byte Rval 
        {
            get { return System.Convert.ToByte(_Rval); }
            set 
            {
                if (_Rval != value)
                {
                    _Rval = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _Gval;
        public byte Gval
        {
            get { return System.Convert.ToByte(_Gval); }
            set
            {
                if (_Gval != value)
                {
                    _Gval = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _Bval;
        public byte Bval
        {
            get { return System.Convert.ToByte(_Bval); }
            set
            {
                if (_Bval != value)
                {
                    _Bval = value;
                    OnPropertyChanged();
                }
            }
        }

        private void blueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            colorRect.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)System.Convert.ToByte(Rval), (byte)System.Convert.ToByte(Gval), (byte)System.Convert.ToByte(Bval)));
            OnPropertyChanged();
        }

        private void greenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            colorRect.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)System.Convert.ToByte(Rval), (byte)System.Convert.ToByte(Gval), (byte)System.Convert.ToByte(Bval)));
            OnPropertyChanged();
        }

        private void blueSlider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            colorRect.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)System.Convert.ToByte(Rval), (byte)System.Convert.ToByte(Gval), (byte)System.Convert.ToByte(Bval)));
            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void setUpload_Click(object sender, RoutedEventArgs e)
        {
            wind.BrushUpload = new SolidColorBrush(Color.FromArgb(255, Rval, Gval, Bval));
            setUpb = true;
            close();
        }

        private void setDownload_Click(object sender, RoutedEventArgs e)
        {
            wind.BrushDownload = new SolidColorBrush(Color.FromArgb(255, Rval, Gval, Bval));
            setDownb = true;
            close();
        }

        private void close() 
        {
            if(setUpb && setDownb)
                optb.Close();
        }
    }
}
