using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serwer.Models
{
    public class Range
    {
        public double x { get; set; }
        public double y { get; set; }
        public Range(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        
    }
}
