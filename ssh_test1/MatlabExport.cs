using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics;
using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Win32;

namespace ssh_test1
{
    internal class MatlabExport
    {
        public MatlabExport(List<ChartValues> values, List<bool> graphSelector) 
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Title = "Save File",
                Filter = "Matlab File (*.mat) | *.mat"
            };
            if (sf.ShowDialog() == true) 
            {
                ConsoleUC.ConsoleText = "4";
                string appednString = "";
                var matrices = new List<MatlabMatrix>();
                int valuesIndex = 0;
                int nameIndex = 0;
                for (int i = 0; i < graphSelector.Count; i++) 
                {
                    if (graphSelector[i]) 
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if(values[valuesIndex].Xvals.Count > 0) 
                            {
                                var m = Matrix<double>.Build.Dense(values[valuesIndex].Xvals.Count, 2);
                                for (int k = 0; k < values[valuesIndex].Xvals.Count; k++)
                                {
                                    m[k, 0] = Convert.ToDouble(values[valuesIndex].Yvals[k]);
                                    m[k, 1] = Convert.ToDouble(values[valuesIndex].Xvals[k]);
                                }
                                if (valuesIndex % 2 == 0)
                                    appednString = "_down";
                                else
                                    appednString = "_up";
                                matrices.Add(MatlabWriter.Pack(m, values[nameIndex].name.Replace("-up", "").Replace("-down", "").Replace("-dn", "").Replace("-", "_").Trim() + appednString));
                            }
                            valuesIndex++;
                        }
                        nameIndex += 2;
                    }
                }
                MatlabWriter.Store(sf.FileName, matrices);
                ConsoleUC.ConsoleText = "0";
            }
        }
    }
}
