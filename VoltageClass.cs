using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireTestProgram
{
   public class VoltageClass
    {

        public string  RDType { get; set; }
        public double RDValue { get; set; }
        public string  SmallPoint { get; set; }
        public string  BigPoint { get; set; }
        public double SmallPointValue { get; set; }
        public List<TestVoltagePoint> BigPoints { get; set; }

        public bool VisibleFlag { get; set; }
        public bool ISConnection { get; set; }
        public string RDirection { get; set; }

    }
}
