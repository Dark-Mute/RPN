using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serwer.Models
{
    public class range
    {
        public double x { get; set; }
        public double y { get; set; }
        public range(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        
    }
}
