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
        private string outputOfDSLAMFar;
        private string outputOfDSLAMNear;
        int counter;
        List<int> yVals;
        List<int> xVals;
        private List<ChartValues> _chartV = new List<ChartValues>();
        public List<ChartValues> chartV
        {
            get { return _chartV; }
            set
            {
                if (_chartV != value)
                    _chartV = value;
            }
        }
        public string name = "";

        private double _hzConstant = 1;
        public double hzConstant 
        {
            get { return _hzConstant;}
            set 
            {
                _hzConstant = value;
            }
        }

        public GraphLogic(string outputOfDSLAFar, string outputOfDSLAMNear, List<bool> graphSelector, string currentMode, bool hertz)
        {
            this.outputOfDSLAMFar = outputOfDSLAFar;
            this.outputOfDSLAMNear = outputOfDSLAMNear;
            this.graphSelector = graphSelector;
            if (hertz)
                _hzConstant = gethzConstant(currentMode);
        }

        private double gethzConstant(string mode) 
        {
            double output = 0;
            switch (mode)
            {
                case string modeis when modeis.Contains("g992-2-30a"):
                    output = 4.3125;
                    break;
                case string modeis when modeis.Contains("gfast"):
                    output = 51.75;
                    break;
                default:
                    output = 8.625;
                    break;
            }
            return output;
        }


        private int GetDecValues(string c)
        {
            return Int32.Parse(c.ToString(), System.Globalization.NumberStyles.HexNumber);
        }

        public void SelectGraphNeeeded(int i)
        {
            string[] substringIndexes = { "load-distribution", "gain-allocation", "snr", "qln", "char-func-complex", "char-func-real", "tx-psd", "tx-psd-carr-grop" };
            string[] carrGrpindex = { "load-carr-grp :", "gain-carr-grp :", "snr-carr-grp :", "qln-carr-grp :", "hlin-carr-grp :", "hlog-carr-grp :", "tx-psd-carr-grop :" };
            string substringFarEnd = outputOfDSLAMFar.Substring(outputOfDSLAMFar.IndexOf(substringIndexes[i]), outputOfDSLAMFar.IndexOf(substringIndexes[i + 1]) - outputOfDSLAMFar.IndexOf(substringIndexes[i]));
            string substringNearEnd = outputOfDSLAMNear.Substring(outputOfDSLAMNear.IndexOf(substringIndexes[i]), outputOfDSLAMNear.IndexOf(substringIndexes[i + 1]) - outputOfDSLAMNear.IndexOf(substringIndexes[i]));
            string carrGrpFarEnd = outputOfDSLAMFar.Substring(outputOfDSLAMFar.IndexOf(carrGrpindex[i]) + carrGrpindex[i].Length, 3).Trim();
            string carrGrpNearEnd = outputOfDSLAMNear.Substring(outputOfDSLAMNear.IndexOf(carrGrpindex[i]) + carrGrpindex[i].Length, 3).Trim();
            getGraphLogic(substringFarEnd, Int32.Parse(carrGrpFarEnd));
            getGraphLogic(substringNearEnd, Int32.Parse(carrGrpNearEnd));
        }

        private void getGraphLogic(string inputString, int carrGrp)
        {
            yVals = new List<int>();
            xVals = new List<int>();
            List<int> outputListY = new List<int>();
            List<int> outputListX = new List<int>();
            List<List<int>> outputVals = new List<List<int>>();
            int i = 0;
            int adder = 0;
            int check = 0;
            string[] inputSplit = inputString.Split(new[] { ':' }, 2);
            string bitload = String.Concat(inputSplit[1].Replace(":", "").Where(c => !Char.IsWhiteSpace(c)));
            switch (inputSplit[0])
            {
                case string name when inputSplit[0].Contains("load-distribution"):
                    while ((check) + adder < bitload.Length)
                    {
                        string startIndex = "";
                        string stopIndex = "";
                        for (i = check + adder; i < check + adder + 8; i++)
                        {
                            if (i < check + adder + 4)
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
                        check += SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i, 1, 0, 1, carrGrp, null, name);
                    }
                    break;
                case string name when inputSplit[0].Contains("gain-allocation"):
                    while ((check) * 4 + adder < bitload.Length)
                    {
                        string startIndex = "";
                        string stopIndex = "";
                        for (i = (check) * 4 + adder; i < (check) * 4 + adder + 8; i++)
                        {
                            if (i < check * 4 + adder + 4)
                                startIndex += bitload[i];
                            else
                                stopIndex += bitload[i];
                        }
                        adder += 8;
                        check += SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i, 4, 0, 512, carrGrp, null, name);
                    }
                    break;
                case string name when inputSplit[0].Contains("snr"):
                    adder = 4;
                    while ((check) * 2 + adder < bitload.Length)
                    {
                        string startIndex = "";
                        string stopIndex = "";
                        for (i = (check) * 2 + adder; i < (check) * 2 + adder + 8; i++)
                        {
                            if (i < (check) * 2 + adder + 4)
                                startIndex += bitload[i];
                            else
                                stopIndex += bitload[i];
                        }
                        adder += 8;
                        check += SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i, 2, -32, 2, carrGrp, 255, name);
                    }
                    break;
                case string name when inputSplit[0].Contains("qln"):
                    adder = 4;
                    while ((check) * 2 + adder < bitload.Length)
                    {
                        string startIndex = "";
                        string stopIndex = "";
                        for (i = (check) * 2 + adder; i < (check) * 2 + adder + 8; i++)
                        {
                            if (i < (check) * 2 + adder + 4)
                                startIndex += bitload[i];
                            else
                                stopIndex += bitload[i];
                        }
                        adder += 8;
                        check += SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i, 2, -23, -2, carrGrp, null, name);
                    }
                    break;
                case string name when inputSplit[0].Contains("char-func-complex"):
                    break;
                case string name when inputSplit[0].Contains("char-func-real"):
                    adder = 4;
                    while ((check) * 4 + adder < bitload.Length)
                    {
                        string startIndex = "";
                        string stopIndex = "";
                        for (i = (check) * 4 + adder; i < (check) * 4 + adder + 8; i++)
                        {
                            if (i < (check) * 4 + adder + 4)
                                startIndex += bitload[i];
                            else
                                stopIndex += bitload[i];
                        }
                        adder += 8;
                        check += SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i, 4, 6, -10, carrGrp, null, name);
                    }
                    break;
                case string name when inputSplit[0].Contains("tx-psd"):
                    while ((check) * 2 + adder < bitload.Length)
                    {
                        string startIndex = "";
                        string stopIndex = "";
                        for (i = (check) * 2 + adder; i < (check) * 2 + adder + 8; i++)
                        {
                            if (i < (check) * 2 + adder + 4)
                                startIndex += bitload[i];
                            else
                                stopIndex += bitload[i];
                        }
                        adder += 8;
                        check += SetGraphValues(bitload, GetDecValues(startIndex), GetDecValues(stopIndex), i, 2, 0, -2, carrGrp, null, name);
                    }
                    break;
            }
            name = inputSplit[0];
            _chartV.Add(new ChartValues { name = inputSplit[0], Xvals = xVals, Yvals = yVals });
        }

        private int SetGraphValues(string inputString, int startIndex, int stopIndex, int charIndexOfStart, int NumberOfNibble, int adder, int divider, int carrGrp, int? valsToSkip, string name)
        {
            int charIndex = charIndexOfStart;
            counter = 0;
            for (int i = 0; i <= (stopIndex - startIndex); i++)
            {
                string input = "";
                for (int j = 0; j < NumberOfNibble; j++)
                    input += inputString[charIndex + j];
                for (int k = 0; k < carrGrp; k++)
                {
                    if (GetDecValues(input) != valsToSkip)
                    {
                        yVals.Add(adder + GetDecValues(input) / divider);
                        xVals.Add(Convert.ToInt32(((startIndex + i) * carrGrp + k) * _hzConstant));
                    }
                }
                charIndex += NumberOfNibble;
                counter++;
            }
            return counter;
        }
    }
}

