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
        private string outputOfDSLAM;
        private string name;
        private List<int> indexes = new List<int>();
        private List<int> groupIndexes = new List<int>();
        private int listindex;
        public GraphLogic(string outputOfDSLAM, int listindex) 
        {

            this.outputOfDSLAM = outputOfDSLAM;
            this.listindex = listindex;
            SelectData();
        }
        public string getName() 
        {
            return name;
        }

        private void SelectData()
        {
            int index = 0;
            String[] outputSplit = outputOfDSLAM.Split('\n');
            foreach(string line in outputSplit) 
            {
                getIndexes(line, index);
                index++;
            }
            indexes.Add(groupIndexes[0]);
            string output = "";
            for (int i = indexes[listindex]; i < indexes[listindex+1]; i++)
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

        private void getIndexes(string line, int index)
        {
            switch (line)
            {
                case string lineNow when line.Contains("grp") || line.Contains("grop") || line.Contains("vect-qln")
                || line.Contains("rmc-carr") | line.Contains("gf"):
                    groupIndexes.Add(index);
                    break;
                case string lineNow when line.Contains("load-distribution") || line.Contains("gain-allocation")
                || line.Contains("snr") || line.Contains("qln") || line.Contains("char-func-complex")
                || line.Contains("char-func-real") || line.Contains("tx-psd"):
                    indexes.Add(index);
                    break;
                default:
                    break;
            }
        }

    }
}
