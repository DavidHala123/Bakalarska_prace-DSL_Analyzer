using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssh_test1
{
    internal class LoadFile
    {
        private string farEnd = "";
        private string nearEnd = "";
        private string textOfFile = "";
        private string fileName = "";
        private bool isOk = false;
        public LoadFile() 
        {
            OpenFileDialog opf = new OpenFileDialog()
            {
                Title = "Open File",
                Filter = "Text File (*.txt) | *.txt"
            };
            if(opf.ShowDialog() == true) 
            {
                isOk = true;
                fileName = opf.SafeFileName;
                textOfFile = File.ReadAllText(opf.FileName);
                farEnd = textOfFile.Substring(textOfFile.IndexOf("---FAR END---"), textOfFile.IndexOf("---NEAR END---") - textOfFile.IndexOf("---FAR END---"));
                nearEnd = textOfFile.Substring(textOfFile.IndexOf("---NEAR END---"), textOfFile.IndexOf("---END OF FILE---") - textOfFile.IndexOf("---NEAR END---"));
            }
        }
        public bool getState() 
        {
            return isOk;
        }

        public string[] getFarNearData() 
        {
            string[] output = {"", "" };
            output[0] = farEnd;
            output[1] = nearEnd;
            return output;
        }
        public string getFileName() 
        {
            return fileName;
        }
    }
}
