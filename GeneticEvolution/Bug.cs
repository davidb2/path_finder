using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticEvolution
{
    class Bug : Agent
    {
        private static Random random;
        static Bug()
        {
            random = new Random();
        }

        public int Index { get; private set; } = 0;

        public override int Id { get; }

        public Bug()
        {
            Genotype = new Moves(Population<Bug>.CYCLES);
            Id = random.Next();
        }

        public override void Reset()
        {
            Index = 0;
        }

        public override Position Move()
        {
            return Genotype[Index++];
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
