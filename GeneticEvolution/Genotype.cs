using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Position = System.Collections.Generic.KeyValuePair<int, int>;


namespace GeneticEvolution
{
    abstract class Genotype
    {
        public abstract Position this[int index]
        {
            get; set;
        }

        public abstract int Count
        {
            get;
        }
    }
}
