using GeneticAlgorithmPCB.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace GeneticAlgorithmPCB
{
    public partial class MainWindow : Window
    {
        
        List<Array> points = new List<Array>();
        int boardWidth = 0;
        int boardHeight = 0;
        public MainWindow()
        {
            InitializeComponent();
        }
        //private void drawBoard()
        //{
        //    for (int i = 0; i < boardWidth; i += 40)
        //    {
        //        for (int j = 0; j < boardHeight; j += 40)
        //        {
        //            var offset = 40 / 2;
        //        }
        //    }
        //}
        public void btnDrawOne(object sender, RoutedEventArgs e)
        {
            String[] lines = System.IO.File.ReadAllLines(@"D:\UniLabs\AI&KE\zad0.txt");
            String[] tempDimensionValues = lines[0].Split(';');
            boardWidth = Convert.ToInt32(tempDimensionValues[0]);
            boardHeight = Convert.ToInt32(tempDimensionValues[1]);


            for (int i = 1; i < lines.Length; i++)
            {
                String[] startEnd = lines[i].Split(';');
                int x1 = Convert.ToInt32(startEnd[0]);
                int y1 = Convert.ToInt32(startEnd[1]);
                int x2 = Convert.ToInt32(startEnd[2]);
                int y2 = Convert.ToInt32(startEnd[3]);
                points.Add(new int[] { x1, y1, x2, y2 });
            }


        }

        public void btnDrawTwo(object sender, RoutedEventArgs e)
        {
            String[] lines = System.IO.File.ReadAllLines(@"D:\UniLabs\AI&KE\zad1.txt");
            String[] tempDimensionValues = lines[0].Split(';');
            boardWidth = Convert.ToInt32(tempDimensionValues[0]);
            boardHeight = Convert.ToInt32(tempDimensionValues[1]);

            for (int i = 1; i < lines.Length; i++)
            {
                String[] startEnd = lines[i].Split(';');
                int x1 = Convert.ToInt32(startEnd[0]);
                int y1 = Convert.ToInt32(startEnd[1]);
                int x2 = Convert.ToInt32(startEnd[2]);
                int y2 = Convert.ToInt32(startEnd[3]);
                points.Add(new int[] { x1, y1, x2, y2 });
            }
        }

        public void btnDrawThree(object sender, RoutedEventArgs e)
        {
            String[] lines = System.IO.File.ReadAllLines(@"D:\UniLabs\AI&KE\zad2.txt");
            String[] tempDimensionValues = lines[0].Split(';');
            boardWidth = Convert.ToInt32(tempDimensionValues[0]);
            boardHeight = Convert.ToInt32(tempDimensionValues[1]);

            for (int i = 1; i < lines.Length; i++)
            {
                String[] startEnd = lines[i].Split(';');
                int x1 = Convert.ToInt32(startEnd[0]);
                int y1 = Convert.ToInt32(startEnd[1]);
                int x2 = Convert.ToInt32(startEnd[2]);
                int y2 = Convert.ToInt32(startEnd[3]);
                points.Add(new int[] { x1, y1, x2, y2 });
            }

    }

        public void btnDrawFour(object sender, RoutedEventArgs e)
        {
            String[] lines = System.IO.File.ReadAllLines(@"D:\UniLabs\AI&KE\zad3.txt");
            String[] tempDimensionValues = lines[0].Split(';');
            boardWidth = Convert.ToInt32(tempDimensionValues[0]);
            boardHeight = Convert.ToInt32(tempDimensionValues[1]);


            for (int i = 1; i < lines.Length; i++)
            {
                String[] startEnd = lines[i].Split(';');
                int x1 = Convert.ToInt32(startEnd[0]);
                int y1 = Convert.ToInt32(startEnd[1]);
                int x2 = Convert.ToInt32(startEnd[2]);
                int y2 = Convert.ToInt32(startEnd[3]);
                points.Add(new int[] { x1, y1, x2, y2 });
            }
    }

        private Dictionary<string, List<int>> CalculateStatistics(int populationSize, List<Individual> currentGeneration)
        {
            List<int> averageFitnes = new List<int>();
            List<int> bestFitnes = new List<int>();
            List<int> worstFitnes = new List<int>();
            List<int> stdFitness = new List<int>();

            Dictionary<string, List<int>> Stats = new Dictionary<string, List<int>>();
            List<int> allFitnessThisGeneration = new List<int>();
            foreach (var individual in currentGeneration)
            {
                allFitnessThisGeneration.Add(individual.fitnessScore);
            }
            double mean = (double)allFitnessThisGeneration.Sum() / (double)allFitnessThisGeneration.Count();
            var squareValues = from int value in allFitnessThisGeneration select (value - mean) * (value - mean);
            double sumOfSquares = squareValues.Sum();

            bestFitnes.Add(allFitnessThisGeneration.Min());
            worstFitnes.Add(allFitnessThisGeneration.Max());
            averageFitnes.Add(allFitnessThisGeneration.Sum() / populationSize);
            stdFitness.Add((int)Math.Sqrt(sumOfSquares / allFitnessThisGeneration.Count()));

            Stats.Add("Best", bestFitnes);
            Stats.Add("Worst", worstFitnes);
            Stats.Add("Average", averageFitnes);
            Stats.Add("STD", stdFitness);

            return Stats;
        }

        private void btnSolveNONGA_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count() == 0 && boardHeight == 0 && boardWidth == 0)
            {
                txt1.Text = "Select Task, Dumbass!";
            }
            else
            {
                int bestCost = int.MaxValue;
                Individual bestFellow = null;
                int populationSize = Convert.ToInt32(pop.Text);
                int numIterations = Convert.ToInt32(num.Text);
                List<Individual> allBest = new List<Individual>();

                for (int j = 0; j < numIterations; j++)
                {
                    List<Individual> solutions = new List<Individual>();
                    for (int i = 0; i < populationSize; i++)
                    {
                        Individual individual = new Individual();
                        foreach (int[] coordinate in points)
                        {
                            int x1 = coordinate[0];
                            int y1 = coordinate[1];
                            int x2 = coordinate[2];
                            int y2 = coordinate[3];

                            Paths path = new Paths(x1, y1, x2, y2);
                            path.Segments = path.GetRandomSegments();
                            individual.paths.Add(path);

                        }
                        individual.fitnessScore = individual.FitnessScore(boardWidth, boardHeight);
                        solutions.Add(individual);
                    }

                    //var statsOfRandom = CalculateStatistics(populationSize, solutions);

                    foreach (Individual s in solutions)
                    {
                        if (s.fitnessScore < bestCost)
                        {
                            bestCost = s.fitnessScore;
                            bestFellow = s;
                        }
                    }
                    best.Text = bestFellow.ToString();
                    allBest.Add(bestFellow);
                }

                List<int> allFitnessFromRuns = new List<int>();
                foreach (var individual in allBest)
                {
                    allFitnessFromRuns.Add(individual.fitnessScore);
                }

                double mean = (double)allFitnessFromRuns.Sum() / (double)allFitnessFromRuns.Count();
                var squareValues = from int value in allFitnessFromRuns select (value - mean) * (value - mean);
                double sumOfSquares = squareValues.Sum();

                var bestFitnes = allFitnessFromRuns.Min();
                var worstFitnes = allFitnessFromRuns.Max();
                var averageFitnes = allFitnessFromRuns.Sum() / populationSize;
                var stdFitness = (int)Math.Sqrt(sumOfSquares / allFitnessFromRuns.Count());
                result.Text =  "Random " + bestFitnes + " " + worstFitnes + " " + averageFitnes + " " + stdFitness;
                //Debug.WriteLine(bestFellow.ToString());
                txt1.Text = "DONE";
            }
        }

        private void btnSolveGA_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count() == 0 && boardHeight == 0 && boardWidth == 0)
            {
                txt.Text = "Select Task, Dumbass!";
            }
            else
            {
                int populationSize = Convert.ToInt32(pop.Text);
                double crossoverRate = Convert.ToDouble(cros.Text);
                double mutationRate = Convert.ToDouble(mut.Text);
                int numIterations = Convert.ToInt32(num.Text);
                int tourneySize = Convert.ToInt32(tour.Text);
                Task.Factory.StartNew(() => AsyncGACall(numIterations, populationSize, crossoverRate, mutationRate, tourneySize));
            }

        }

        private void AsyncGACall(int numIterations, int populationSize, double crossoverRate, double mutationRate, int tourneySize)
        {
            Individual bestIndividual = null;
            List<Individual> allBest = new List<Individual>();
            
            int bestScore = int.MaxValue;

            for (int i = 0; i < numIterations; i++)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    txt.Text = "Calculating #" + (i + 1) + "...";
                }),
                    DispatcherPriority.Background);

                var geneticEngine = new GeneticAlgorithmEngine(populationSize, crossoverRate, mutationRate, tourneySize);
                bestIndividual = geneticEngine.FindSolution(boardWidth, boardHeight, points);
                allBest.Add(bestIndividual);
                //Debug.WriteLine(bestIndividual.ToString());
                var stats = geneticEngine.Stats;
            }

            List<int> allFitnessFromRuns = new List<int>();
            foreach (var individual in allBest)
            {
                allFitnessFromRuns.Add(individual.fitnessScore);
                if(individual.fitnessScore < bestScore)
                {
                    bestScore = individual.fitnessScore;
                    bestIndividual = individual;
                }
            }

            double mean = (double)allFitnessFromRuns.Sum() / (double)allFitnessFromRuns.Count();
            var squareValues = from int value in allFitnessFromRuns select (value - mean) * (value - mean);
            double sumOfSquares = squareValues.Sum();

            var bestFitnes = allFitnessFromRuns.Min();
            var worstFitnes= allFitnessFromRuns.Max();
            var averageFitnes = allFitnessFromRuns.Sum() / populationSize;
            var stdFitness = (int)Math.Sqrt(sumOfSquares / allFitnessFromRuns.Count());
            Dispatcher.BeginInvoke(new Action(() =>
            {
                result1.Text = "GA " + bestFitnes + " " + worstFitnes + " " + averageFitnes + " " + stdFitness;
            }), DispatcherPriority.Background);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                txt.Text = "END";
            }), DispatcherPriority.Background);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                
                best.Text = bestIndividual.ToString();
            }), DispatcherPriority.Background);
        }
    }
}
