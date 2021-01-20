using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WDPR_MVC.Models
{
    public class IPModel
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public int FailCount { get; set; }
    }
}
