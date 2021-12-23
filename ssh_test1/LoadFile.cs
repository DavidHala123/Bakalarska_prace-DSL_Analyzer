﻿using Microsoft.Win32;
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
        public LoadFile() 
        {
            OpenFileDialog opf = new OpenFileDialog()
            {
                Title = "Open File",
                Filter = "Text File (*.txt) | *.txt"
            };
            if(opf.ShowDialog() == true) 
            {
                textOfFile = File.ReadAllText(opf.FileName);
                farEnd = textOfFile.Substring(textOfFile.IndexOf("---FAR END---"), textOfFile.IndexOf("---NEAR END---") - textOfFile.IndexOf("---FAR END---"));
                nearEnd = textOfFile.Substring(textOfFile.IndexOf("---NEAR END---"), textOfFile.IndexOf("---END OF FILE---") - textOfFile.IndexOf("---NEAR END---"));
            }
        }

        public string[] getFarNearData() 
        {
            string[] output = {"", "" };
            output[0] = farEnd;
            output[1] = nearEnd;
            return output;
        }
    }
}
