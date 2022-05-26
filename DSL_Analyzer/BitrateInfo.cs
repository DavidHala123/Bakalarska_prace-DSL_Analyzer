using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSL_Analyzer
{
    //HOLDS INFORMATION OF STATIC BITRATES
    internal class BitrateInfo
    {
        public string name { get; set; }
        public double upbitr { get; set; }
        public double downbitr { get; set; }
    }
}
