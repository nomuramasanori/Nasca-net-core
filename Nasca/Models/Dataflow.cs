using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class Dataflow
    {
        public List<Node2> nodes { get; set; }
        public List<Link> links { get; set; }

        public Dataflow()
        {
            this.nodes = new List<Node2>();
            this.links = new List<Link>(); 
        }
    }
}
