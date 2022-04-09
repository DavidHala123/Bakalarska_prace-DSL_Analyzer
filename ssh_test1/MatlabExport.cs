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
        public MatlabExport(List<List<int>> X, List<List<int>> Y, List<bool> graphSelector, List<string> names) 
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Title = "Save File",
                Filter = "Matlab File (*.mat) | *.mat"
            };
            if (sf.ShowDialog() == true) 
            {
                string appednString = "";
                var matrices = new List<MatlabMatrix>();
                int valuesIndex = 0;
                for (int i = 0; i < graphSelector.Count; i++) 
                {
                    if (graphSelector[i]) 
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if(X[valuesIndex].Count > 0) 
                            {
                                var m = Matrix<double>.Build.Dense(X[valuesIndex].Count, 2);
                                for (int k = 0; k < X[valuesIndex].Count; k++)
                                {
                                    m[k, 0] = Convert.ToDouble(Y[valuesIndex][k]);
                                    m[k, 1] = Convert.ToDouble(X[valuesIndex][k]);
                                }
                                if (valuesIndex % 2 == 0)
                                    appednString = "_down";
                                else
                                    appednString = "_up";
                                matrices.Add(MatlabWriter.Pack(m, names[i].Replace("-up", "").Replace("-down", "").Replace("-dn", "").Replace("-", "_").Trim() + appednString));
                            }
                            valuesIndex++;
                        }
                    }
                }
                MessageBox.Show(valuesIndex.ToString());
                MatlabWriter.Store(sf.FileName, matrices);
            }
        }
    }
}
