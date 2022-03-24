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
                    var collection = new Excel.Worksheet[valuesY.Count / 2];
                    for (int x = valuesY.Count - 1; x >= 1; x-=2)
                    {
                        if(graphs[x/2] == true) 
                        {
                            int PositionUpload = 3;
                            collection[x / 2] = (Excel.Worksheet)wb.Worksheets.Add();
                            collection[x / 2].Name = names[x];
                            collection[x / 2].Cells[1, 1] = "UPSTREAM";
                            collection[x / 2].Cells[2, 2] = "X values";
                            collection[x / 2].Cells[2, 1] = "Y values";
                            for (int y = 0; y <= valuesX[x].Count - 1; y++)
                            {
                                try
                                {
                                    collection[x / 2].Cells[PositionUpload, 2] = valuesX[x][y];
                                    collection[x / 2].Cells[PositionUpload, 1] = valuesY[x][y];
                                    PositionUpload++;
                                    if (valuesX[x][y] + 1 != valuesX[x][y + 1])
                                        PositionUpload++;
                                }
                                catch
                                {
                                    collection[x / 2].Cells[PositionUpload, 2] = valuesX[x][y];
                                    collection[x / 2].Cells[PositionUpload, 1] = valuesY[x][y];
                                }
                            }
                            int PositionDownload = 3;
                            collection[x / 2].Cells[1, 5] = "DOWNSTREAM";
                            collection[x / 2].Cells[2, 6] = "X values";
                            collection[x / 2].Cells[2, 5] = "Y values";
                            for (int y = 0; y <= valuesX[x - 1].Count - 1; y++)
                            {
                                try
                                {
                                    collection[x / 2].Cells[PositionDownload, 6] = valuesX[x - 1][y];
                                    collection[x / 2].Cells[PositionDownload, 5] = valuesY[x - 1][y];
                                    PositionDownload++;
                                    if (valuesX[x - 1][y] + 1 != valuesX[x - 1][y + 1])
                                        PositionDownload++;
                                }
                                catch
                                {
                                    collection[x / 2].Cells[PositionDownload, 6] = valuesX[x - 1][y];
                                    collection[x / 2].Cells[PositionDownload, 5] = valuesY[x - 1][y];
                                }
                            }
                            Excel.ChartObjects xlCharts = (Excel.ChartObjects)collection[x / 2].ChartObjects(Type.Missing);
                            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(500, 80, 400, 200);
                            Excel.Chart chartPage = myChart.Chart;
                            chartPage.ChartType = Excel.XlChartType.xlXYScatterSmoothNoMarkers;
                            myChart.Select();
                            Excel.SeriesCollection seriesCollection = (Excel.SeriesCollection)chartPage.SeriesCollection();
                            Excel.Series os1 = seriesCollection.NewSeries();
                            os1.Name = "UPLOAD";
                            os1.Values = collection[x / 2].get_Range("A3", $"A{PositionUpload}");
                            os1.XValues = collection[x / 2].get_Range("B3", $"B{PositionUpload}");
                            Excel.Series os2 = seriesCollection.NewSeries();
                            os2.Name = "DOWNLOAD";
                            os2.Values = collection[x / 2].get_Range("E3", $"E{PositionDownload}");
                            os2.XValues = collection[x / 2].get_Range("F3", $"F{PositionDownload}");
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
