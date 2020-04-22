using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serwer.Models
{
    public class Results : Imodel
    {
        public string status { get; set; }
        public object result { get; set; }
    }
}
