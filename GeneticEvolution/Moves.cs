using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticEvolution
{
    class Moves : Genotype
    {
        private static Random random;
        private Position[] _moves;

        static Moves()
        {
            random = new Random();
        }

        public Moves(int count)
        {
            Count = count;
            _moves = new Position[count];
            for (int i = 0; i < count; i++)
            {
                _moves[i] = Direction.Positions[random.Next(Direction.Positions.Length)];
            }
        }

        public override Position this[int index]
        {
            get
            {
                return _moves[index];
            }
            set
            {
                _moves[index] = value;
            }
        }

        public override int Count
        {
            get;
        }
    }
}
