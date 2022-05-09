﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ssh_test1
{
    internal class LoadFile
    {
        private string generalInfo = "";
        private string rtInfo = "";
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
                if (File.GetLastWriteTimeUtc(opf.FileName).ToString() != File.GetCreationTimeUtc(opf.FileName).ToString()) 
                {
                    MessageBox.Show($"File was modified. Shown data may be corrupted.\nDate and time of last change: {File.GetLastWriteTimeUtc(opf.FileName).ToString()}");
                }
                fileName = opf.SafeFileName;
                textOfFile = File.ReadAllText(opf.FileName);
                try 
                {
                    try 
                    {
                        generalInfo = textOfFile.Substring(textOfFile.IndexOf("---GENERAL INFO---"), textOfFile.IndexOf("---RT INFO---") - textOfFile.IndexOf("---GENERAL INFO---"));
                        rtInfo = textOfFile.Substring(textOfFile.IndexOf("---RT INFO---"), textOfFile.IndexOf("---FAR END---") - textOfFile.IndexOf("---RT INFO---"));
                    }
                    catch 
                    {
    
                    }
                    farEnd = textOfFile.Substring(textOfFile.IndexOf("---FAR END---"), textOfFile.IndexOf("---NEAR END---") - textOfFile.IndexOf("---FAR END---"));
                    nearEnd = textOfFile.Substring(textOfFile.IndexOf("---NEAR END---"), textOfFile.IndexOf("---END OF FILE---") - textOfFile.IndexOf("---NEAR END---"));
                    isOk = true;
                }
                catch 
                {
                    MessageBox.Show("Provided file has not the right format. Make sure you selected a file previously generated by this program.");
                    isOk = false;
                }
            }
        }
        public bool getState() 
        {
            return isOk;
        }

        public string[] getFarNearData() 
        {
            string[] output = {"", "", "", "" };
            output[0] = generalInfo;
            output[1] = rtInfo;
            output[2] = farEnd;
            output[3] = nearEnd;
            return output;
        }
        public string getFileName() 
        {
            return fileName;
        }
    }
}
