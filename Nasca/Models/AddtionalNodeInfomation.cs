using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class AddtionalNodeInfomation
    {
        private int distance;
        public int getDistance()
        {
            return distance;
        }
        public AddtionalNodeInfomation(int distance)
        {
            this.distance = distance;
        }
    }
}
