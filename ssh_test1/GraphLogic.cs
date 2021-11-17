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
        private List<int> decValues = new List<int>();
        private int startIndex;
        private int stopIndex;
        private string outputOfDSLAM;
        private string name;
        public GraphLogic(int startIndex, int stopIndex, string outputOfDSLAM) 
        {
            this.startIndex = startIndex;
            this.stopIndex = stopIndex;
            this.outputOfDSLAM = outputOfDSLAM;
            SelectData();
        }
        public string getName() 
        {
            return name;
        }

        private void SelectData()
        {
            String[] outputSplit = outputOfDSLAM.Split('\n');
            string output = "";
            for (int i = startIndex; i < stopIndex; i++)
            {
                output += outputSplit[i] + '\n';
            }
            setGraphDecValues(output);
        }
        public List<int> getGraphDecValues() 
        {
            return decValues;
        }
        private void setGraphDecValues(string input)
        {
            List<int> listOfDecValues = new List<int>();
            string[] outputSPlit = input.Split(':');
            foreach (string j in outputSPlit)
            {
                try
                {
                    listOfDecValues.Add(Int32.Parse(j, System.Globalization.NumberStyles.HexNumber));
                }
                catch
                {
                    if(!String.IsNullOrEmpty(j.Trim()))
                        name = j.Trim();
                    continue;
                }
            }
            decValues = listOfDecValues;
        }
       
    }
}
