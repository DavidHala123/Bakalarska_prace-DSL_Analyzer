using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InteractiveDataDisplay.WPF;

namespace ssh_test1
{
    internal class GraphLogic
    {
        private List<bool> graphSelector = new List<bool>();
        private List<Chart> outputListOfCharts = new List<Chart>();
        private List<string> name = new List<string>();
        private string outputOfDSLAMFar;
        private string outputOfDSLAMNear;
        private List<List<int>> listOfChartVals = new List<List<int>>();

        public GraphLogic(string outputOfDSLAFar, string outputOfDSLAMNear, List<bool> graphSelector)
        {
            ConsoleLogic.ConsoleText = "3";
            this.outputOfDSLAMFar = outputOfDSLAFar;
            this.outputOfDSLAMNear = outputOfDSLAMNear;
            this.graphSelector = graphSelector;
            SelectGraphNeeeded();
        }

        public List<List<int>> getListChartDecValues() 
        {
            return listOfChartVals;
        }

        public List<string> getListOfNames() 
        {
            return name;
        }

        private List<int> setGraphDecValuesAsync(string inputString)
        {
            List<int> listOfDecValues = new List<int>();
            string[] outputSplit = inputString.Split(':');
            name.Add(outputSplit[0].Trim());
            for (int i = 5; i <= outputSplit.Count(); i++)
            {
                try
                {
                    foreach (char c in outputSplit[i])
                    {
                        int testValue = Int32.Parse(c.ToString(), System.Globalization.NumberStyles.HexNumber);
                        listOfDecValues.Add(testValue);
                    }
                }
                catch
                {
                    continue;
                }
            }
            return listOfDecValues;
        }

        private void SelectGraphNeeeded() 
        {
            string[] substringIndexes = { "load-distribution", "gain-allocation", "snr", "qln", "char-func-complex", "char-func-real", "tx-psd", "tx-psd-carr-grop" };
            for(int i = 0; i < graphSelector.Count; i++) 
            {
                if(graphSelector[i] == true)
                {
                    string substringFarEnd = outputOfDSLAMFar.Substring(outputOfDSLAMFar.IndexOf(substringIndexes[i]), outputOfDSLAMFar.IndexOf(substringIndexes[i + 1]) - outputOfDSLAMFar.IndexOf(substringIndexes[i]));
                    string substringNearEnd = outputOfDSLAMNear.Substring(outputOfDSLAMNear.IndexOf(substringIndexes[i]), outputOfDSLAMNear.IndexOf(substringIndexes[i + 1]) - outputOfDSLAMNear.IndexOf(substringIndexes[i]));
                    listOfChartVals.Add(setGraphDecValuesAsync(substringFarEnd));
                    listOfChartVals.Add(setGraphDecValuesAsync(substringNearEnd));
                }
            }
        }
    }
}

