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
            MessageBox.Show(valuesY.Count.ToString() + ", " + names.Count.ToString());
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
                        int i = 1;
                        collection[x/2] = (Excel.Worksheet)wb.Worksheets.Add();
                        collection[x/2].Name = names[x];
                        foreach (int y in valuesY[x])
                        {
                            collection[x/2].Cells[i, 1] = y;
                            i++;
                        }
                        foreach (int y in valuesY[x-1])
                        {
                            collection[x/2].Cells[i, 1] = y;
                            i++;
                        }
                        i = 1;
                        foreach (int y in valuesX[x])
                        {
                            collection[x / 2].Cells[i, 2] = y;
                            i++;
                        }
                        foreach (int y in valuesX[x - 1])
                        {
                            collection[x / 2].Cells[i, 2] = y;
                            i++;
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
