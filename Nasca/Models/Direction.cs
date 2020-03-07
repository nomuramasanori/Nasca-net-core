using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nasca.Models
{
    public enum Direction
    {
        I, O, IO, None
    }

    public static partial class EnumExtend
    {
        public static Direction reverse(this Direction param)
        {
            Direction result = Direction.None;
            if (param == Direction.IO || param == Direction.None) result = param;
            if (param == Direction.I) result = Direction.O;
            if (param == Direction.O) result = Direction.I;
            return result;
        }

        public static Direction add(this Direction param, Direction direction)
        {
            Direction result = param;
            if (direction == Direction.IO)
            {
                result = direction;
            }
            if (param == Direction.None)
            {
                result = direction;
            }
            if (param == Direction.I && direction == Direction.O)
            {
                result = Direction.IO;
            }
            if (param == Direction.O && direction == Direction.I)
            {
                result = Direction.IO;
            }
            return result;
        }
    }
}
