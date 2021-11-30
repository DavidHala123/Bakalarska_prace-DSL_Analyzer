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
        private List<List<int>> listOfChartYVals = new List<List<int>>();
        private List<List<int>> listOfChartXVals = new List<List<int>>();

        public GraphLogic(string outputOfDSLAFar, string outputOfDSLAMNear, List<bool> graphSelector)
        {
            ConsoleLogic.ConsoleText = "3";
            this.outputOfDSLAMFar = outputOfDSLAFar;
            this.outputOfDSLAMNear = outputOfDSLAMNear;
            this.graphSelector = graphSelector;
        }

        public List<List<int>> getYValues() 
        {
            return listOfChartYVals;
        }
        public List<List<int>> getXValues()
        {
            return listOfChartXVals;
        }

        public List<string> getListOfNames() 
        {
            return name;
        }

        private int GetDecValues(string c)
        {
            return Int32.Parse(c.ToString(), System.Globalization.NumberStyles.HexNumber);
        }

        private void SetGraphValues(string inputString, int startIndex, int stopIndex, int charIndexOfStart, int NumberOfNibble, int divider)
        {
            List<int> outputListY = new List<int>();
            List<int> outputListX = new List<int>();
            int charIndex = charIndexOfStart;
            for (int i = startIndex; i <= stopIndex; i += NumberOfNibble)
            {
                string input = "";
                for(int j = 0; j < NumberOfNibble; j++)
                    input += inputString[charIndex+j];
                outputListY.Add(GetDecValues(input)/divider);
                outputListX.Add(i);
                charIndex++;
            }
            listOfChartYVals.Add(outputListY);
            listOfChartXVals.Add(outputListX);
        }

        private void getGraphLogic(string inputString)
        {
            string startIndex = "";
            string stopIndex = "";
            int i = 0;
            string[] inputSplit = inputString.Split(new[] { ':' }, 2);
            name.Add(inputSplit[0]);
            string bitload = String.Concat(inputSplit[1].Replace(":", "").Where(c => !Char.IsWhiteSpace(c)));
            if (inputSplit[0].Contains("load-distribution"))
            {
                for (i = 0; i < 8; i++)
                {
                    if (i < 4)
                        startIndex += bitload[i];
                    else
                        stopIndex += bitload[i];
                }
                SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i+1, 1, 1);
            }
            if (inputSplit[0].Contains("gain-allocation"))
            {
                for (i = 0; i < 8; i++)
                {
                    if (i < 4)
                        startIndex += bitload[i];
                    else
                        stopIndex += bitload[i];
                }
                SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i + 1, 3, 512);
            }
        }

        public void SelectGraphNeeeded(int i) 
        {
            string[] substringIndexes = { "load-distribution", "gain-allocation", "snr", "qln", "char-func-complex", "char-func-real", "tx-psd", "tx-psd-carr-grop" };
            string substringFarEnd = outputOfDSLAMFar.Substring(outputOfDSLAMFar.IndexOf(substringIndexes[i]), outputOfDSLAMFar.IndexOf(substringIndexes[i + 1]) - outputOfDSLAMFar.IndexOf(substringIndexes[i]));
            string substringNearEnd = outputOfDSLAMNear.Substring(outputOfDSLAMNear.IndexOf(substringIndexes[i]), outputOfDSLAMNear.IndexOf(substringIndexes[i + 1]) - outputOfDSLAMNear.IndexOf(substringIndexes[i]));
            getGraphLogic(substringFarEnd);
            getGraphLogic(substringNearEnd);
        }
    }
}

