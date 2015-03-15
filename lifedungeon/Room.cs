using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace lifedungeon
{
    public class Room
    {
        public Point pos;
        public Point size;
        public int type;

        public Room( Point p, Point s, int t )
        {
            this.pos = p;
            this.size = s;
            this.type = t;
        }
    }
}
