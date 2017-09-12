using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticEvolution
{
    abstract class Agent : IComparable
    {
        public Genotype Genotype { get; set; }
        public Phenotype Phenotype { get; set; }
        public abstract void Reset();
        public abstract Position Move();
        public abstract int Id { get; }

        public abstract override int GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Agent agent = obj as Agent;
            return Id == agent.Id;
        }

        public int CompareTo(object other)
        {
            return (other == null || GetType() != other.GetType()) ? -1 : 0;
        }
    }
}
