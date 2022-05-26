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

namespace DSL_Analyzer
{
    //EXPORTS DATA TO MATLAB
    internal class MatlabExport
    {
        public MatlabExport(List<ChartValues> values, List<bool> graphSelector, double hzConstant)
        {
            int matriceIndexer = 0;
            int row = -1;
            SaveFileDialog sf = new SaveFileDialog
            {
                Title = "Save File",
                Filter = "Matlab File (*.mat) | *.mat"
            };
            if (sf.ShowDialog() == true)
            {
                ConsoleUC.ConsoleText = "4";
                List<double> xVals = new List<double>();
                List<double> yVals = new List<double>();
                var matrices = new List<MatlabMatrix>();
                int valuesIndex = 0;
                int nameIndex = 0;
                string appednString;
                for (int i = 0; i < graphSelector.Count; i++)
                {
                    if (graphSelector[i])
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (j % 2 == 0)
                                appednString = "_up";
                            else
                                appednString = "_down";
                            row = -1;
                            if (values[valuesIndex].Xvals.Count > 0)
                            {
                                xVals.Clear();
                                yVals.Clear();
                                matriceIndexer = 0;
                                for (int k = 0; k < values[valuesIndex].Xvals.Count; k++)
                                {
                                    row++;
                                    if (k == 0) 
                                    {
                                        yVals.Add(Convert.ToDouble(values[valuesIndex].Yvals[k]));
                                        xVals.Add(Convert.ToDouble(values[valuesIndex].Xvals[k]));
                                        continue;
                                    }
                                    if(values[valuesIndex].Xvals[k] - values[valuesIndex].Xvals[k-1] <= Math.Ceiling(hzConstant)) 
                                    {
                                        yVals.Add(Convert.ToDouble(values[valuesIndex].Yvals[k]));
                                        xVals.Add(Convert.ToDouble(values[valuesIndex].Xvals[k]));
                                    }
                                    else 
                                    {
                                        var ma = Matrix<double>.Build.Dense(xVals.Count, 2);
                                        populateMatrix(ma, xVals, yVals);
                                        matrices.Add(MatlabWriter.Pack(ma, values[valuesIndex].name.Replace("-", "_").Trim() + matriceIndexer.ToString()));
                                        xVals.Clear();
                                        yVals.Clear();
                                        yVals.Add(Convert.ToDouble(values[valuesIndex].Yvals[k]));
                                        xVals.Add(Convert.ToDouble(values[valuesIndex].Xvals[k]));
                                        row = -1;
                                        matriceIndexer++;
                                    }
                                }
                                var m = Matrix<double>.Build.Dense(xVals.Count, 2);
                                populateMatrix(m, xVals, yVals);
                                matrices.Add(MatlabWriter.Pack(m, values[valuesIndex].name.Replace("-", "_").Trim() + matriceIndexer.ToString()));
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

        private void populateMatrix(Matrix<double> mat, List<double> x, List<double> y) 
        {
            for(int i = 0; i < x.Count; i++) 
            {
                mat[i,0] = x[i];
                mat[i,1] = y[i];
            }
        }
    }
}