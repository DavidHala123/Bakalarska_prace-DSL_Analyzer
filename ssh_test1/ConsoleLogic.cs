using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ssh_test1
{
    internal class ConsoleLogic
    {
        private static string consoleText = "0";
        private string progressionInfo;
        private int length;
        public ConsoleLogic(int progressionInfo, int length)
        {
            this.progressionInfo = progressionInfo.ToString();
            this.length = length;
            ConsoleText = this.progressionInfo;
        }

        public static string ConsoleText
        {
            get
            {
                return consoleText;
            }
            set
            {
                string output = "";
                switch (value)
                {
                    case "0":
                        output = "";
                        break;
                    case "1":
                        output = "Sending request to DSLAM";
                        break;
                    case "2":
                        output = "Reading response";
                        break;
                    case "3":
                        output = "Processing data";
                        break;
                    case "4":
                        output = "Exporting data";
                        break;

                }
                consoleText = output;
            }
        }
    }
}
