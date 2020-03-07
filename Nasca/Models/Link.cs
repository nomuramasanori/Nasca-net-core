using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class Link
    {
        public string source { get; set; }
        public string target { get; set; }
        public bool isCreate { get; set; }
        public bool isRead { get; set; }
        public bool isUpdate { get; set; }
        public bool isDelete { get; set; }
        public string remark { get; set; }
        public string io { get; set; }
        public string colorIndex { get; set; }
        public bool visible { get; set; }
        public bool isVirtual { get; set; }
    }
}
