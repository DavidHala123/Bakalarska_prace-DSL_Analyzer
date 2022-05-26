using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSL_Analyzer
{
    //PROJECTING PORT NAME AND PORT STATE AS AN IMAGE TO PORTBOX IN MAIN WINDOW
    internal class PortBoxLogic
    {
        private List<PortData> portDatasCombo = new List<PortData>();
        private string commandOutput;
        public PortBoxLogic(string commandOutput) 
        {
            this.commandOutput = commandOutput;
            setPortDataCombo();
        }
        public List<PortData> getPortDataCombo() 
        {
            return portDatasCombo;
        }
        private void setPortDataCombo() 
        {
            ConsoleUC.ConsoleText = "3";
            List<PortData> output = new List<PortData>();
            using var sr = new StringReader(commandOutput);
            {
                string line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        AddPortToList(output, line);
                    }
                    catch
                    {
                        continue;
                    }
                }
                ConsoleUC.ConsoleText = "0";
                portDatasCombo = output;
            }
        }

        private static void AddPortToList(List<PortData> output, string line)
        {
            if (char.IsNumber(line[0]))
            {
                string[] info = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                output.Add(new PortData { portName = info[0], portState = @"Images\" + info[2] + ".png" });
            }
        }
    }
}
