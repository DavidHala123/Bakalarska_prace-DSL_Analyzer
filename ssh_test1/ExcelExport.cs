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
        public ExcelExport(List<List<int>> valuesY, List<List<int>> valuesX, List<string> names, List<bool> graphs)
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
                    ConsoleLogic.ConsoleText = "4";
                    var wb = _excel.Workbooks.Add();
                    double numberOfColD = valuesY.Count / 2;
                    int numOfColI = (int)Math.Ceiling(numberOfColD);
                    var collection = new Excel.Worksheet[numOfColI];
                    int nameIndex = names.Count - 1;
                    int valuesCount = valuesY.Count() - 1;
                    for (int chart = graphs.Count - 1; chart >= 0; chart--)
                    {
                        if (graphs[chart])
                        {
                            int indexOfCol = numOfColI - 1;
                            int PositionUpload = 3;
                            collection[indexOfCol] = (Excel.Worksheet)wb.Worksheets.Add();
                            collection[indexOfCol].Name = names[nameIndex];
                            collection[indexOfCol].Cells[1, 1] = "UPSTREAM";
                            collection[indexOfCol].Cells[2, 2] = "X values";
                            collection[indexOfCol].Cells[2, 1] = "Y values";
                            for (int y = 0; y <= valuesX[valuesCount].Count - 1; y++)
                            {
                                try
                                {
                                    collection[indexOfCol].Cells[PositionUpload, 2] = valuesX[valuesCount][y];
                                    collection[indexOfCol].Cells[PositionUpload, 1] = valuesY[valuesCount][y];
                                    PositionUpload++;
                                    if (valuesX[valuesCount][y] + 1 != valuesX[valuesCount][y + 1])
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
                            for (int xparam = 0; xparam <= valuesX[valuesCount - 1].Count - 1; xparam++)
                            {
                                try
                                {
                                    collection[indexOfCol].Cells[PositionDownload, 6] = valuesX[valuesCount - 1][xparam];
                                    collection[indexOfCol].Cells[PositionDownload, 5] = valuesY[valuesCount - 1][xparam];
                                    PositionDownload++;
                                    if (valuesX[valuesCount - 1][xparam] + 1 != valuesX[valuesCount - 1][xparam + 1])
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
                            myChart.Select();
                            Excel.SeriesCollection seriesCollection = (Excel.SeriesCollection)chartPage.SeriesCollection();
                            Excel.Series os1 = seriesCollection.NewSeries();
                            os1.Name = "UPLOAD";
                            os1.Values = collection[indexOfCol].get_Range("A3", $"A{PositionUpload}");
                            os1.XValues = collection[indexOfCol].get_Range("B3", $"B{PositionUpload}");
                            Excel.Series os2 = seriesCollection.NewSeries();
                            os2.Name = "DOWNLOAD";
                            os2.Values = collection[indexOfCol].get_Range("E3", $"E{PositionDownload}");
                            os2.XValues = collection[indexOfCol].get_Range("F3", $"F{PositionDownload}");
                            indexOfCol--;
                            nameIndex -= 2;
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