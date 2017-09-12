using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GeneticEvolution
{
    class Population<T> : IEnumerable<Agent[]>
        where T : Agent, new()
    {
        public static readonly int ROWS = 25;
        public static readonly int COLS = 25;
        public static readonly int CYCLES = ROWS + COLS + 10;
        public static readonly int STARTROW = COLS/2- (1 - (COLS % 2));
        public static readonly int STARTCOL = ROWS/2-(1-(ROWS%2));
        public static readonly int ENDROW = ROWS - 1;
        public static readonly int ENDCOL = COLS - 1;
        private static readonly double CROSSOVER_RATE = 0.5;
        private static readonly double MUTATION_RATE = 0.05;
        private static readonly int GENERATION_STEP = 50;
        public int Size { get; set; }
        public int Generation { get; private set; } = 0;
        public Agent[] Agents { get; set; }
        public HashSet<Agent>[,] Grid { get; private set; }
        public Dictionary<Agent, Position> Positions { get; set; }
        private static readonly int SEED = (1 << 11) - 0x1f;
        private Random _random;

        public Population(int size)
        {
            Size = size;
            _random = new Random(SEED);
            Agents = new Agent[Size];
            Grid = new HashSet<Agent>[ROWS, COLS];
            for (int i = 0; i < Size; i++)
            {
                Agents[i] = new T();
            }
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    Grid[row, col] = new HashSet<Agent>();
                }
            }
            foreach (Agent agent in Agents)
            {
                Grid[STARTROW, STARTCOL].Add(agent);
            }
        }

        public IEnumerator<Agent[]> GetEnumerator()
        {
            while (Positions.Count > 0)
            {
                yield return NextGeneration();
            }
            yield return null;   
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private double ManhattanDistance(Position position)
        {
            return ENDROW - position.Key + ENDCOL - position.Value;
        }

        private double Score(Position position)
        {
            // use manhattan distance
            double distance = ManhattanDistance(position);

            // invert (smaller distance == higher score)
            return (ENDROW + ENDCOL) - distance;
        }

        private Dictionary<Agent, double> EvaluateFitnesses()
        {
            Dictionary<Agent, double> fitnesses = new Dictionary<Agent, double>();
            foreach (Agent agent in Positions.Keys)
            {
                fitnesses.Add(agent, Score(Positions[agent]));
            }
            if (Generation % GENERATION_STEP == 0)
            {
                double distance = ManhattanDistance(Positions[fitnesses.Max(kv => new Tuple<double, Agent>(kv.Value, kv.Key)).Item2]);
                Console.WriteLine("Closest agent of Generation {0} is {1} cells away from target.", Generation, distance);
            }

            return fitnesses;
        }

        private Agent[] Crossover(Dictionary<Agent, double> fitnesses)
        {
            // normalize fitnesses
            double sumOfFitnesses = fitnesses.Values.Sum();
            double cumSum = 0.0;

            Agent[] agents = new Agent[fitnesses.Count];
            double[] probs = new double[fitnesses.Count];

            foreach (var it in fitnesses.Keys.Select((x, i) => new { Agent = x, Index = i }))
            {
                cumSum += (fitnesses[it.Agent] / sumOfFitnesses);
                agents[it.Index] = it.Agent;
                probs[it.Index] = cumSum;
            }

            Agent[] children = new Agent[Size];

            // spin the wheel n times
            for (int i = 0; i < Size; i++)
            {
                double p1 = _random.NextDouble();
                double p2 = _random.NextDouble();
                Agent agent1 = agents[BisectLeft(probs, p1)];
                Agent agent2 = agents[BisectLeft(probs, p2)];
                Agent child = new T();

                for (int j = 0; j < agent1.Genotype.Count; j++)
                {
                    child.Genotype[j] = (_random.NextDouble() < CROSSOVER_RATE) ? agent2.Genotype[j] : agent1.Genotype[j];
                }
                children[i] = child;
            }

            return children;
        }

        private void MutateChildren(Agent[] children)
        {
            for (int i = 0; i < children.Length; i++)
            {
                for (int j = 0; j < children[i].Genotype.Count; j++) {
                    if (_random.NextDouble() < MUTATION_RATE)
                    {
                        children[i].Genotype[j] =
                            Direction.Positions[_random.Next(Direction.Positions.Length)];
                    }
                }
            }
        }

        private int BisectLeft(double[] arr, double x)
        {
            int lo = 0, hi = arr.Length;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (arr[mid] < x) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }

        private Agent[] NextGeneration()
        {
            Dictionary<Agent, double> fitnesses = EvaluateFitnesses();
            Agent[] children = Crossover(fitnesses);
            MutateChildren(children);
           
            this.Agents = children;
            this.Size = this.Agents.Length;
            this.Generation += 1;
            return children;
        }
    }
}
