using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssh_test1
{
    internal class SaveFile
    {
        public SaveFile(string dataFarEnd, string dataNearEnd, string generalInfo, string rtInfo) 
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Title = "Save File",
                Filter = "Text File (*.txt) | *.txt"
            };
            if(sf.ShowDialog() == true)
                File.WriteAllText(sf.FileName, "---GENERAL INFO--- \n" + generalInfo + "---RT INFO--- \n" + rtInfo + "---FAR END--- \n" + dataFarEnd + "\n---NEAR END---\n" + dataNearEnd + "\n---END OF FILE---");
        }
    }
}
