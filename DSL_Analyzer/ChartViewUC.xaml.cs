﻿using InteractiveDataDisplay.WPF;
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
    /// Interaction logic for ChartViewUC.xaml
    /// </summary>
    public partial class ChartViewUC : UserControl
    {
        //SHOWS CERTAIN CHARTS
        public ChartViewUC(ChartValues dataFarEnd, ChartValues dataNearEnd, int i, SolidColorBrush up, SolidColorBrush down, string currm, double hz)
        {
            DataContext = this;
            this.up = up;
            this.down = down;
            InitializeComponent();
            this.offset = hz;
            ChartGraph(dataFarEnd, dataNearEnd, i);
        }
        private SolidColorBrush _up;
        public SolidColorBrush up 
        {
            get { return _up; }
            set 
            {
                if(_up != value)
                    _up = value;
            }
        }
        private SolidColorBrush _down;
        public SolidColorBrush down
        {
            get { return _down; }
            set
            {
                if (_down != value)
                    _down = value;
            }
        }
        private List<bool> graphSelector = new List<bool>();
        private int _numCarrUP;
        public int numCarrUP
        {
            get { return _numCarrUP; }
            set
            {
                if (_numCarrUP != value)
                    _numCarrUP = value;
            }
        }
        private int _numCarrDOWN;
        public int numCarrDOWN
        {
            get { return _numCarrDOWN; }
            set
            {
                if (_numCarrDOWN != value)
                    _numCarrDOWN = value;
            }
        }
        private string _YaxisName;
        public string YaxisName
        {
            get { return _YaxisName; }
            set
            {
                if (_YaxisName != value)
                    _YaxisName = value;
            }
        }
        private double offset;

        List<string> YaxisNames = new List<string>() { "number of bits [bit]", "gain [dB]", "snr [dB]", "qln [dBm/Hz]", "HLIN [dB]", "HLOG [dB]", "Tx-PSD [dbm/Hz]", "", "qln [dbm/Hz]", "aln [dBm/Hz]" };
        
        //CREATES FINAL CHARTS
        private async void ChartGraph(ChartValues dataFarEnd, ChartValues dataNearEnd, int i)
        {
            try
            {
                getChart(dataFarEnd, dataNearEnd);
                chart.LeftTitle = YaxisNames[i];
                if(offset > 1)
                    chart.BottomTitle = "f [kHz]";
            }
            catch 
            {
            }
        }

        //CREATING AND FILLINF CHART OBJECTS
        private void getChart(ChartValues chartVnear, ChartValues chartVfar)
        {
            List<List<double>> BandFar = new List<List<double>>();
            List<List<double>> BandNear = new List<List<double>>();
            if (chartVnear.name.Contains("down") || chartVnear.name.Contains("char-func-real"))
            {
                BandFar = splitListForBands(chartVnear.Xvals, chartVnear.Yvals);
                BandNear = splitListForBands(chartVfar.Xvals, chartVfar.Yvals);
            }
            else
            {
                BandFar = splitListForBands(chartVfar.Xvals, chartVfar.Yvals);
                BandNear = splitListForBands(chartVnear.Xvals, chartVnear.Yvals);
            }
            for (int j = 0; j < BandFar.Count - 1; j += 2)
            {
                LineGraph lineGraphFar = new LineGraph()
                {
                    Stroke = down,
                    Padding = new System.Windows.Thickness(0, 30, 0, 1),
                };
                lineGraphFar.Plot(BandFar[j], BandFar[j + 1]);
                chartGrid.Children.Add(lineGraphFar);
            }
            for (int j = 0; j < BandNear.Count - 1; j += 2)
            {
                LineGraph lineGraphNear = new LineGraph()
                {
                    Stroke = up,
                    Padding = new System.Windows.Thickness(0, 30, 0, 0),
                };
                lineGraphNear.Plot(BandNear[j], BandNear[j + 1]);
                chartGrid.Children.Add(lineGraphNear);
            }
        }

        //SPLITTING DATA TO DISCONTINUOUS PLOTS IF NEEDED
        private List<List<double>> splitListForBands(List<double> graphValuesX, List<double> graphValuesY)
        {
            List<double> valsX = new List<double>();
            List<double> valsY = new List<double>();
            List<List<double>> output = new List<List<double>>();
            int xInd = 0;
            for (int j = 0; j < graphValuesX.Count; j++)
            {
                try
                {
                    if (graphValuesX[j + 1] > graphValuesX[j] + Math.Ceiling(offset))
                    {
                        output.Add(graphValuesX.GetRange(xInd, j + 1 - xInd));
                        output.Add(graphValuesY.GetRange(xInd, j + 1 - xInd));
                        xInd = j + 1;
                    }
                }
                catch
                {
                    output.Add(graphValuesX.GetRange(xInd, j + 1 - xInd));
                    output.Add(graphValuesY.GetRange(xInd, j + 1 - xInd));
                    break;
                }
            }
            return output;
        }
    }
}
