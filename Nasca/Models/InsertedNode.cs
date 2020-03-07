using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class InsertedNode
    {
        public string originalid { get; set; }
        public string parentid { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string remark { get; set; }
    }
}
