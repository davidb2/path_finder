using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Position = System.Collections.Generic.KeyValuePair<int, int>;

namespace GeneticEvolution
{
    public class Direction
    {
        public readonly Position UP = new Position(0, -1);
        public readonly Position DOWN = new Position(0, 1);
        public readonly Position LEFT = new Position(-1, 0);
        public readonly Position RIGHT = new Position(1, 0);
        public readonly Position NONE = new Position(0, 0);
        public readonly static Position[] Positions = {
            new Position(0, -1), new Position(0, 1),
            new Position(-1, 0), new Position(1, 0),
            new Position(0, 0)
        };

        // disallow constructor
        private Direction() { }
    }
}
