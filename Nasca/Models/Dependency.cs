using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class Dependency
    {
        public String id { get; set; }
        public Element element { get; set; }
        public Element dependencyElement { get; set; }
        public bool dependencyTypeCreate { get; set; }
        public bool dependencyTypeRead { get; set; }
        public bool dependencyTypeUpdate { get; set; }
        public bool dependencyTypeDelete { get; set; }
        public String remark { get; set; }

        public void setDependencyTypeDelete(bool dependencyTypeDelete)
        {
            this.dependencyTypeDelete = dependencyTypeDelete;
        }
        public String getDependencyType()
        {
            return
                (this.dependencyTypeCreate ? "1" : "0") +
                (this.dependencyTypeRead ? "1" : "0") +
                (this.dependencyTypeUpdate ? "1" : "0") +
                (this.dependencyTypeDelete ? "1" : "0");
        }
    }
}
