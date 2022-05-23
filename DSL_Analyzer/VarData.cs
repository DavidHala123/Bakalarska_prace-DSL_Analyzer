using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSL_Analyzer
{
    internal class VarData
    {
        public double hzCons { get; set; }
        public string dataFarEnd { get; set; }
        public string dataNearEnd { get; set; }
        public string generalInfo { get; set; }
        public string rtInfo { get; set; }
        public bool conChanged { get; set; }
    }
}
