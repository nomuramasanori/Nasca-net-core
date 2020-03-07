using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class ElementType
    {
        private String elementType;
        private String svgFile;

        public String getElementType()
        {
            return elementType;
        }
        public void setElementType(String elementType)
        {
            this.elementType = elementType;
        }
        public String getSvgFile()
        {
            return svgFile;
        }
        public void setSvgFile(String svgFile)
        {
            this.svgFile = svgFile;
        }
    }
}
