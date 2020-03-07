using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class LinkKey
    {
        private Element source;
        private Element target;
        public Element getSource()
        {
            return source;
        }
        public Element getTarget()
        {
            return target;
        }

        public LinkKey(Element source, Element target)
        {
            this.source = source;
            this.target = target;
        }

        override public bool Equals(Object obj)
        {
            LinkKey linkKey = null;

            // オブジェクトがnullでないこと
            if (obj == null)
            {
                return false;
            }
            // オブジェクトが同じ型であること
            if (obj is LinkKey) {
                linkKey = (LinkKey)obj;
            }else{
                return false;
            }
            // 同値性を比較
            return this.getSource().Equals(linkKey.getSource()) && this.getTarget().Equals(linkKey.getTarget());
        }

        public override int GetHashCode()
        {
            return this.getSource().getId().GetHashCode() + this.getTarget().getId().GetHashCode();
        }
    }
}
