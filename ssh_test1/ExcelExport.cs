using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace ssh_test1
{
    internal class ExcelExport
    {
        public ExcelExport(List<ChartValues> values, List<bool> graphs)
        {
            var _excel = new Excel.Application();
            try
            {
                SaveFileDialog sf = new SaveFileDialog()
                {
                    Title = "Save File",
                    Filter = "Excel FILE (*.xlsx) | *.xlsx"
                };
                if (sf.ShowDialog() == true)
                {
                    ConsoleUC.ConsoleText = "4";
                    var wb = _excel.Workbooks.Add();
                    double numberOfColD = values.Count / 2;
                    int numOfColI = (int)Math.Ceiling(numberOfColD);
                    var collection = new Excel.Worksheet[numOfColI];
                    int valuesCount = values.Count() - 1;
                    for (int chart = graphs.Count - 1; chart >= 0; chart--)
                    {
                        if (graphs[chart])
                        {
                            int indexOfCol = numOfColI - 1;
                            int PositionUpload = 3;
                            collection[indexOfCol] = (Excel.Worksheet)wb.Worksheets.Add();
                            collection[indexOfCol].Name = values[valuesCount].name.Replace("-up", "").Replace("-down", "").Replace("-dn", "").Trim();
                            collection[indexOfCol].Cells[1, 1] = "UPSTREAM";
                            collection[indexOfCol].Cells[2, 2] = "X values";
                            collection[indexOfCol].Cells[2, 1] = "Y values";
                            for (int y = 0; y <= values[valuesCount].Xvals.Count(); y++)
                            {
                                try
                                {
                                    collection[indexOfCol].Cells[PositionUpload, 2] = values[valuesCount].Xvals[y];
                                    collection[indexOfCol].Cells[PositionUpload, 1] = values[valuesCount].Yvals[y];
                                    PositionUpload++;
                                    if (values[valuesCount].Xvals[y] + 1 != values[valuesCount].Xvals[y + 1])
                                        PositionUpload++;
                                }
                                catch
                                {
                                    PositionUpload++;
                                }
                            }
                            int PositionDownload = 3;
                            collection[indexOfCol].Cells[1, 5] = "DOWNSTREAM";
                            collection[indexOfCol].Cells[2, 6] = "X values";
                            collection[indexOfCol].Cells[2, 5] = "Y values";
                            for (int xparam = 0; xparam <= values[valuesCount - 1].Xvals.Count - 1; xparam++)
                            {
                                try
                                {
                                    collection[indexOfCol].Cells[PositionDownload, 6] = values[valuesCount - 1].Xvals[xparam];
                                    collection[indexOfCol].Cells[PositionDownload, 5] = values[valuesCount - 1].Yvals[xparam];
                                    PositionDownload++;
                                    if (values[valuesCount - 1].Xvals[xparam] + 1 != values[valuesCount - 1].Xvals[xparam + 1])
                                        PositionDownload++;
                                }
                                catch
                                {
                                    PositionDownload++;
                                }
                            }
                            Excel.ChartObjects xlCharts = (Excel.ChartObjects)collection[indexOfCol].ChartObjects(Type.Missing);
                            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(500, 80, 400, 200);
                            Excel.Chart chartPage = myChart.Chart;
                            chartPage.ChartType = Excel.XlChartType.xlXYScatterSmoothNoMarkers;
                            chartPage.HasTitle = true;
                            chartPage.ChartTitle.Text = values[valuesCount].name.Replace("-up", "").Replace("-down", "").Replace("-dn", "").Trim();
                            myChart.Select();
                            Excel.SeriesCollection seriesCollection = (Excel.SeriesCollection)chartPage.SeriesCollection();
                            Excel.Series os1 = seriesCollection.NewSeries();
                            os1.Name = "DOWNLOAD";
                            os1.Values = collection[indexOfCol].get_Range("A3", $"A{PositionUpload}");
                            os1.XValues = collection[indexOfCol].get_Range("B3", $"B{PositionUpload}");
                            Excel.Series os2 = seriesCollection.NewSeries();
                            os2.Name = "UPLOAD";
                            os2.Values = collection[indexOfCol].get_Range("E3", $"E{PositionDownload}");
                            os2.XValues = collection[indexOfCol].get_Range("F3", $"F{PositionDownload}");
                            indexOfCol--;
                            valuesCount -= 2;
                        }
                    }
                    wb.SaveAs(sf.FileName);
                    wb.Close();

                }
            }
            finally
            {
                Marshal.ReleaseComObject(_excel);
            }
        }
    }
}