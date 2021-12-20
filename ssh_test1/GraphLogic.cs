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

        public void SelectGraphNeeeded(int i)
        {
            string[] substringIndexes = { "load-distribution", "gain-allocation", "snr", "qln", "char-func-complex", "char-func-real", "tx-psd", "tx-psd-carr-grop" };
            string substringFarEnd = outputOfDSLAMFar.Substring(outputOfDSLAMFar.IndexOf(substringIndexes[i]), outputOfDSLAMFar.IndexOf(substringIndexes[i + 1]) - outputOfDSLAMFar.IndexOf(substringIndexes[i]));
            string substringNearEnd = outputOfDSLAMNear.Substring(outputOfDSLAMNear.IndexOf(substringIndexes[i]), outputOfDSLAMNear.IndexOf(substringIndexes[i + 1]) - outputOfDSLAMNear.IndexOf(substringIndexes[i]));
            getGraphLogic(substringFarEnd);
            getGraphLogic(substringNearEnd);
        }

        private void getGraphLogic(string inputString)
        {
            List<int> outputListY = new List<int>();
            List<int> outputListX = new List<int>();
            List<List<int>> outputVals = new List<List<int>>();
            int i = 0;
            int adder = 0;
            string[] inputSplit = inputString.Split(new[] { ':' }, 2);
            name.Add(inputSplit[0]);
            string bitload = String.Concat(inputSplit[1].Replace(":", "").Where(c => !Char.IsWhiteSpace(c)));
            switch (inputSplit[0]) 
            {
                case string name when inputSplit[0].Contains("load-distribution"):
                    while(outputListY.Count + adder < bitload.Length)
                    {
                        string startIndex = "";
                        string stopIndex = "";
                        for (i = outputListY.Count + adder; i < outputListY.Count + adder + 8; i++)
                        {
                            if (i < outputListY.Count + adder + 4)
                                startIndex += bitload[i];
                            else
                                stopIndex += bitload[i];
                        }
                        int DecValStart = GetDecValues(startIndex);
                        int DecValStop = GetDecValues(stopIndex);
                        if ((DecValStop - DecValStart) % 2 == 0)
                        {
                            adder += 9;
                        }
                        else
                            adder += 8;
                        outputVals = SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i, 1, 1, 1);
                        outputListY.AddRange(outputVals[0]);
                        outputListX.AddRange(outputVals[1]);
                    }
                    break;
                case string name when inputSplit[0].Contains("gain-allocation"):
                    while(outputListY.Count * 4 + adder < bitload.Length) 
                    {
                        string startIndex = "";
                        string stopIndex = "";
                        for (i = outputListY.Count * 4 + adder; i < outputListY.Count * 4 + adder + 8; i++)
                        {
                            if (i < outputListY.Count * 4 + adder + 4)
                                startIndex += bitload[i];
                            else
                                stopIndex += bitload[i];
                        }
                        adder += 8;
                        outputVals = SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i, 4, 4, 512);
                        outputListY.AddRange(outputVals[0]);
                        outputListX.AddRange(outputVals[1]);
                    }
                    break;
            }
            listOfChartYVals.Add(outputListY);
            listOfChartXVals.Add(outputListX);
        }

        //    private List<List<int>> SetGraphValues(string inputString, int startIndex, int stopIndex, int charIndexOfStart, int NumberOfNibble, int iterationIndex, int divider)
        //    {
        //        List<int> yVals = new List<int>();
        //        List<int> xVals = new List<int>();
        //        List<List<int>> outputList = new List<List<int>>();
        //        int charIndex = charIndexOfStart;
        //        for (int i = startIndex; i <= stopIndex; i += iterationIndex)
        //        {
        //            string input = "";
        //            for(int j = 0; j < NumberOfNibble; j++)
        //                input += inputString[charIndex+j];
        //            yVals.Add(GetDecValues(input)/divider);
        //            xVals.Add(i);
        //            charIndex += iterationIndex;
        //        }
        //        outputList.Add(yVals);
        //        outputList.Add(xVals);
        //        return outputList;
        //    }
        //}
        private List<List<int>> SetGraphValues(string inputString, int startIndex, int stopIndex, int charIndexOfStart, int NumberOfNibble, int iterationIndex, int divider)
        {
            List<int> yVals = new List<int>();
            List<int> xVals = new List<int>();
            List<List<int>> outputList = new List<List<int>>();
            int charIndex = charIndexOfStart;
            for (int i = 0; i <= stopIndex - startIndex; i++)
            {
                string input = "";
                for (int j = 0; j < NumberOfNibble; j++)
                    input += inputString[charIndex + j];
                yVals.Add(GetDecValues(input) / divider);
                xVals.Add(startIndex + i);
                charIndex += NumberOfNibble;
            }
            outputList.Add(yVals);
            outputList.Add(xVals);
            return outputList;
        }
    }
}

