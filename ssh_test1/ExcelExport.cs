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
        public ExcelExport(List<List<int>> valuesY, List<List<int>> valuesX, List<string> names) 
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
                    var wb = _excel.Workbooks.Add();
                    var collection = new Excel.Worksheet[valuesY.Count / 2];
                    for (int x = valuesY.Count - 1; x >= 1; x-=2)
                    {
                        int PositionUpload = 3;
                        collection[x/2] = (Excel.Worksheet)wb.Worksheets.Add();
                        collection[x/2].Name = names[x];
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
                        for (int y = 0; y <= valuesX[x-1].Count - 1; y++)
                        {
                            try
                            {
                                collection[x / 2].Cells[PositionDownload, 6] = valuesX[x-1][y];
                                collection[x / 2].Cells[PositionDownload, 5] = valuesY[x-1][y];
                                PositionDownload++;
                                if (valuesX[x - 1][y] + 1 != valuesX[x - 1][y + 1])
                                    PositionDownload++;
                            }
                            catch
                            {
                                collection[x / 2].Cells[PositionDownload, 6] = valuesX[x-1][y];
                                collection[x / 2].Cells[PositionDownload, 5] = valuesY[x-1][y];
                            }
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
