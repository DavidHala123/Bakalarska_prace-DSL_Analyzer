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
        int ind = 0;
        public GraphLogic(string outputOfDSLAM) 
        {

            this.outputOfDSLAM = outputOfDSLAM;
            setGraphDecValues(this.outputOfDSLAM);
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
                //getIndexes(line, index);
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
        private async Task setGraphDecValues(string input)
        {
            List<int> listOfDecValues = new List<int>();
            string[] outputSplit = input.Split(':');
            name = outputSplit[0].Trim();
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
            decValues = listOfDecValues;
        }

        //private void getIndexes(string line, int index)
        //{
        //    switch (line)
        //    {
        //        case string lineNow when line.Contains("grp") || line.Contains("grop") || line.Contains("vect-qln")
        //        || line.Contains("rmc-carr") | line.Contains("gf"):
        //            groupIndexes.Add(index);
        //            break;
        //        case string lineNow when line.Contains("load-distribution") || line.Contains("gain-allocation")
        //        || line.Contains("snr") || line.Contains("qln") || line.Contains("char-func-complex")
        //        || line.Contains("char-func-real") || line.Contains("tx-psd"):
        //            indexes.Add(index);
        //            break;
        //        default:
        //            break;
        //    }
        //}

    }
}
