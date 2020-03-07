using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public class AddtionalLinkInfomation
    {
        private Dependency dependency;
        private int distance;
        private Direction direction;
        public bool isVirtual { get; set; }

        public Dependency getDependency()
        {
            return dependency;
        }
        public int getDistance()
        {
            return distance;
        }
        public Direction getDirection()
        {
            return direction;
        }
        public void setDirection(Direction direction)
        {
            this.direction = direction;
        }
        public AddtionalLinkInfomation(Dependency dependency, int distance, Direction direction)
        {
            this.dependency = dependency;
            this.distance = distance;
            this.direction = direction;
        }
        public void addDirection(Direction direction)
        {
            this.setDirection(this.getDirection().add(direction));
        }
    }
}
