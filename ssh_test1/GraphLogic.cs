using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ssh_test1
{
    internal class GraphLogic
    {
        private List<List<int>> decValues = new List<List<int>>();
        List<int> indexes = new List<int>();
        List<int> groupIndexes = new List<int>();
        private List<string> name = new List<string>();
        public GraphLogic(string outputOfDSLAM, List<bool> graphselector) 
        {
            FillInedex(outputOfDSLAM, graphselector);
        }
        public List<string> getName() 
        {
            return name;
        }

        public List<List<int>> getGraphDecValues() 
        {
            return decValues;
        }
        private void setGraphDecValues(string input)
        {
            List<int> listOfDecValues = new List<int>();
            string[] outputSplit = input.Split(':');
            name.Add(outputSplit[0].Trim());
            for (int i = 6; i <= outputSplit.Count(); i++)
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
            decValues.Add(listOfDecValues);
        }

        private void FillInedex(string outputOfDSLAM, List<bool> graphSelector)
        {
            string[] substringIndexes = { "load-distribution", "gain-allocation", "snr", "qln", "char-func-complex", "char-func-real", "tx-psd", "tx-psd-carr-grop" };
            List<string> substrings = new List<string>();
            for (int i = 0; i < graphSelector.Count; i++) 
            {
                if(graphSelector[i] == true) 
                {
                    string substring = outputOfDSLAM.Substring(outputOfDSLAM.IndexOf(substringIndexes[i]),
                            outputOfDSLAM.IndexOf(substringIndexes[i+1]) - outputOfDSLAM.IndexOf(substringIndexes[i]));
                    setGraphDecValues(substring);
                }
            }
        }
    }
}

