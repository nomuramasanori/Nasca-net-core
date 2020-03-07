using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class Node2
    {
        public string parent { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string remark { get; set; }
        public string svgFile { get; set; }
        public bool visible { get; set; }
        public int size { get; set; }
        public bool group { get; set; }
        public int depth { get; set; }
    }
}
