using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class InsertedLink
    {
        public string source { get; set; }
        public string target { get; set; }
        public bool dependencyTypeC { get; set; }
        public bool dependencyTypeR { get; set; }
        public bool dependencyTypeU { get; set; }
        public bool dependencyTypeD { get; set; }
        public string remark { get; set; }
    }
}


