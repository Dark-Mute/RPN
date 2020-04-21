using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serwer.Models
{
    public class error : Imodel
    {
        public string status { get; set; }
        public object message { get; set; }
    }
}
