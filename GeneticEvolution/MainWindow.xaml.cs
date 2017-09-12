using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace GeneticEvolution
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int SLEEP = 50;
        private readonly int POPULATION_SIZE = 100;
        private Rectangle[,] _gridElements;
        private Population<Bug> _population;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGrid();
            InitializePopulation();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        void InitializePopulation()
        {
            _population = new Population<Bug>(POPULATION_SIZE);
            _population.Positions = new Dictionary<Agent, Position>();

            foreach (Agent agent in _population.Agents)
            {
                _population.Positions[agent] = new Position(Population<Bug>.STARTCOL, Population<Bug>.STARTROW);
            }
            UpdateGrid();
        }

        void InitializePopulation(Agent[] agents)
        {
            _population.Positions = new Dictionary<Agent, Position>();

            foreach (Agent agent in _population.Agents)
            {
                _population.Positions[agent] = new Position(Population<Bug>.STARTCOL, Population<Bug>.STARTROW);
            }
            UpdateGrid();
        }

        void InitializeGrid()
        {

            _gridElements = new Rectangle[Population<Bug>.ROWS, Population<Bug>.COLS];
            _grid.Rows = Population<Bug>.ROWS;
            _grid.Columns = Population<Bug>.COLS;
            for (int row = 0; row < Population<Bug>.ROWS; row++)
            {
                for (int col = 0; col < Population<Bug>.COLS; col++)
                {
                    _gridElements[row, col] = new Rectangle
                    {
                        Fill = Brushes.Blue, 
                        Stretch = Stretch.Fill,
                        
                    };
                    Grid.SetRow(_gridElements[row, col], row);
                    Grid.SetColumn(_gridElements[row, col], col);
                    _grid.Children.Add(_gridElements[row, col]);
                }
            }
            Grid.SetRow(_grid, 0);
            Grid.SetColumn(_grid, 0);
        }

        private void ClearGrid()
        {
            for (int row = 0; row < Population<Bug>.ROWS; row++)
            {
                for (int col = 0; col < Population<Bug>.COLS; col++)
                {
                    _gridElements[row, col].Fill = Brushes.Blue;
                }
            }
        }

        private void UpdateGrid()
        {
            ClearGrid();
            foreach (Agent agent in _population.Positions.Keys)
            {
                Position position = _population.Positions[agent];
                int row = position.Value, col = position.Key;
                _gridElements[row, col].Fill = Brushes.Red;
            }
        }

        private bool AgentSucceeded(out Agent successfulAgent)
        {
            foreach (Agent agent in _population.Positions.Keys)
            {
                Position position = _population.Positions[agent];
                if (position.Key == Population<Bug>.ENDCOL &&
                    position.Value == Population<Bug>.ENDROW)
                {
                    successfulAgent = agent;
                    return true;
                }
            }
            successfulAgent = null;
            return false;
        }

        async Task ShowPath(Agent agent)
        {
            ClearGrid();
            agent.Reset();
            _gridElements[Population<Bug>.STARTCOL, Population<Bug>.STARTROW].Fill = Brushes.Green;
            Position position =
                new Position(Population<Bug>.STARTCOL, Population<Bug>.STARTROW);

            for (int i = 0; i < Population<Bug>.CYCLES; i++)
            {
                ClearGrid();
                Position updatedPosition = Add(agent.Genotype[i], position);
                // if the agent is not on the grid anymore, consider it dead; thus it cannot reproduce.
                if (0 <= updatedPosition.Key && updatedPosition.Key < Population<Bug>.ROWS &&
                    0 <= updatedPosition.Value && updatedPosition.Value < Population<Bug>.COLS)
                {
                    int row = updatedPosition.Value;
                    int col = updatedPosition.Key;
                    _gridElements[row, col].Fill = Brushes.Green;
                }
                await Task.Delay(SLEEP);
                position = updatedPosition;
            }
        }

        async Task RunSimulation()
        {
            var enumerator = _population.GetEnumerator();
            while (true)
            {
                for (int _ = 0; _ < Population<Bug>.CYCLES; _++)
                {
                    Dictionary<Agent, Position> updatedPositions =
                        new Dictionary<Agent, Position>();
                    foreach (Agent agent in _population.Positions.Keys)
                    {
                        Position updatedPosition = Add(_population.Positions[agent], agent.Move());

                        // if the agent is not on the grid anymore, consider it dead; thus it cannot reproduce.
                        if (0 <= updatedPosition.Key && updatedPosition.Key < Population<Bug>.ROWS &&
                            0 <= updatedPosition.Value && updatedPosition.Value < Population<Bug>.COLS)
                        {
                            updatedPositions.Add(agent, updatedPosition);
                        }
                    }
                    _population.Positions = updatedPositions;
                    UpdateGrid();
                    await Task.Delay(1);
                }
                Agent successfulAgent;
                if (AgentSucceeded(out successfulAgent))
                {
                    System.Media.SystemSounds.Beep.Play();
                    Console.WriteLine("Succeeded at Generation {0}", _population.Generation);
                    LogPath(successfulAgent);
                    await ShowPath(successfulAgent);
                    break;
                }
                enumerator.MoveNext();
                if (enumerator.Current == null)
                {
                    Console.WriteLine("Population died off.");
                    Console.WriteLine("Last Generation: {0}", _population.Generation);
                    break;
                }
                InitializePopulation(_population.GetEnumerator().Current);
            }
        }

        private Position Add(Position p1, Position p2)
        {
            return new Position(p1.Key + p2.Key, p1.Value + p2.Value);
        }

        private void LogPath(Agent agent)
        {
            Position position = 
                new Position(Population<Bug>.STARTCOL, Population<Bug>.STARTROW);
            for (int i = 0; i < agent.Genotype.Count; i++)
            {
                Console.Write("({0}, {1}), ", position.Key, position.Value);
                position = Add(position, agent.Genotype[i]);
            }
            Console.WriteLine("({0}, {1})", position.Key, position.Value);
        }

        private async void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Console.WriteLine("running simulation");
                await RunSimulation();
                Console.WriteLine("done simulation");
                InitializePopulation();
            }
        }
    }
}
