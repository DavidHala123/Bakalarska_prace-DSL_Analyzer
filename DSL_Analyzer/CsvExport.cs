using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Windows;

namespace DSL_Analyzer
{
    //EXPORTS DATA TO CSV
    internal class CsvExport
    {
        public CsvExport(List<ChartValues> chartV, List<bool> graphSelector)
        {
            ConsoleUC.ConsoleText = "4";
            int chartIndex = 0;
            for (int i = 0; i < graphSelector.Count; i++) //ITERATES THROUGH EVERY CHART THAT IS TRUE IN GRAPHSELECTOR AND CREATES CSV FILE
            {
                if (graphSelector[i])
                {
                    SaveFileDialog sf = new SaveFileDialog()
                    {
                        Title = $"Save File ({chartV[chartIndex].name.Replace("-up", "").Replace("-down", "").Replace("-dn", "")})",
                        Filter = "CSV FILE (*.csv) | *.csv"
                    };
                    if (sf.ShowDialog() == true)
                    {
                        var csv = new StringBuilder();
                        csv.AppendLine(chartV[chartIndex].name);
                        csv.AppendLine("UPSTREAM;;;;DOWNSTREAM");
                        csv.AppendLine("Y values;X values;;;Y values;X values");
                        int maxIndex = 0;
                        if (chartV[chartIndex].Xvals.Count > chartV[chartIndex + 1].Xvals.Count)
                            maxIndex = chartV[chartIndex].Xvals.Count;
                        else
                            maxIndex = chartV[chartIndex + 1].Xvals.Count;
                        for (int j = 0; j < maxIndex; j++)
                        {
                            var newline = "";
                            if (j < chartV[chartIndex].Xvals.Count)
                            {
                                newline += ($"{chartV[chartIndex].Yvals[j].ToString()};{chartV[chartIndex].Xvals[j].ToString()}");
                            }
                            if (j < chartV[chartIndex + 1].Xvals.Count)
                            {
                                newline += ($";;;{chartV[chartIndex + 1].Yvals[j].ToString()};{chartV[chartIndex + 1].Xvals[j].ToString()}");
                            }
                            csv.AppendLine(newline);
                        }
                        File.WriteAllText(sf.FileName, csv.ToString());
                        chartIndex += 2;
                    }
                }
            }
            ConsoleUC.ConsoleText = "0";
        }
    }
}
