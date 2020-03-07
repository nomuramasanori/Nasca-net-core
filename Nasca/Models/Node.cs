using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class Node
    {
        public string id { get; set; }
        public string text { get; set; }
        public string parent { get; set; }
        public string parentText { get; set; }
        public string type { get; set; }
        public string remark { get; set; }
        public string icon { get; set; }
        public bool hasDependency { get; set; }
    }
}
