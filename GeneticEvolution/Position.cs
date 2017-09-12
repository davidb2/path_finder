using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticEvolution
{
    public class Position
    {
        public int Key { get; set; }
        public int Value { get; set; }
        public Position(int key, int value)
        {
            Key = key;
            Value = value;
        }
    }
}
