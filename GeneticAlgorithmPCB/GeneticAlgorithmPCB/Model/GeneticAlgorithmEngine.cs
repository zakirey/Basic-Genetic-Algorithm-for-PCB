using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmPCB.Model
{

    class GeneticAlgorithmEngine
    {
        private const int NoChangeTermination = 50;

        private int populationSize;
        private double crossoverRate;
        private double mutationRate;
        private int tourneySize;

        List<Individual> currentGeneration;
        private int totalFitnessThisGeneration = 0;
        private Individual bestSolution = null;
        private int bestFitnessScore = int.MaxValue;
        private int bestSolutionGenerationNumber = 0;

        List<int> averageFitnes = new List<int>();
        List<int> bestFitnes = new List<int>();
        List<int> worstFitnes = new List<int>();
        List<int> stdFitness = new List<int>();

        public Dictionary<string, List<int>> Stats { get; set; }
        public GeneticAlgorithmEngine(int intialPopulationSize, double crossoverRate, double mutationRate, int tourneySize )
        {
            populationSize = intialPopulationSize;
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.tourneySize = tourneySize;
        }

        public void InitalPopulation(List<Array> points)
        {
            for (int i = 0; i < populationSize; i++)
            {
                Individual individual = new Individual();
                foreach(int[] coordinate in points)
                {
                    int x1 = coordinate[0];
                    int y1 = coordinate[1];
                    int x2 = coordinate[2];
                    int y2 = coordinate[3];

                    Paths path = new Paths(x1, y1, x2, y2);
                    path.Segments = path.GetRandomSegments();
                    individual.paths.Add(path);
                    
                }
                
                currentGeneration.Add(individual);
            }
        }
        public void CalculateStatistics()
        {
            Stats = new Dictionary<string, List<int>>();
            List<int> allFitnessThisGeneration = new List<int>();
            foreach (var individual in currentGeneration)
            {
                allFitnessThisGeneration.Add(individual.fitnessScore);
            }
            double mean = (double) allFitnessThisGeneration.Sum() / (double) allFitnessThisGeneration.Count();
            var squareValues = from int value in allFitnessThisGeneration select (value - mean) * (value - mean);
            double sumOfSquares = squareValues.Sum();

            bestFitnes.Add(allFitnessThisGeneration.Min());
            worstFitnes.Add(allFitnessThisGeneration.Max());
            averageFitnes.Add(totalFitnessThisGeneration / populationSize);
            stdFitness.Add((int) Math.Sqrt( sumOfSquares /  allFitnessThisGeneration.Count()));

            Stats.Add("Best", bestFitnes);
            Stats.Add("Worst", worstFitnes);
            Stats.Add("Average", averageFitnes);
            Stats.Add("STD", stdFitness);

        }

        public Individual FindSolution(int boardWidth, int boardHeight, List<Array> points)
        {
            int generationNumber = 0;
            Individual bestSolutionThisGeneration = null;
            int bestFitnessScoreThisGeneration = int.MaxValue;

            currentGeneration = new List<Individual>();
            InitalPopulation(points);

            while (true)
            {
                foreach (var individual in currentGeneration)
                {
                    individual.fitnessScore = individual.FitnessScore(boardWidth, boardHeight);
                    totalFitnessThisGeneration += (int)individual.fitnessScore;
                    if (individual.fitnessScore < bestFitnessScoreThisGeneration )
                    {

                        bestFitnessScoreThisGeneration = (int)individual.fitnessScore;
                        bestSolutionThisGeneration = individual;
                        
                    } 

                }

                //CalculateStatistics();
                //Debug.WriteLine(bestSolutionThisGeneration.ToString());
                //Debug.WriteLine(generationNumber + "    " + bestSolutionGenerationNumber);

                if (bestFitnessScoreThisGeneration < bestFitnessScore)
                {
                    bestFitnessScore = bestFitnessScoreThisGeneration;
                    bestSolution = bestSolutionThisGeneration;

                    bestSolutionGenerationNumber = generationNumber;
                } else if((generationNumber - bestSolutionGenerationNumber) > NoChangeTermination) { break; }



                List<Individual> nextGeneration = new List<Individual>();
                while (nextGeneration.Count() < populationSize)
                {

                    Individual parent1;
                    Individual parent2;

                    //Roulette wheel selection
                    parent1 = SelectCandidateViaRoulette();
                    parent2 = SelectCandidateViaRoulette();

                    //Tournament selection
                    //parent1 = SelectCandidateViaTournament();
                    //parent2 = SelectCandidateViaTournament();

                    Individual child1, child2;
                    CrossOverParents(parent1, parent2, out child1, out child2);
                    Mutation(child1);
                    Mutation(child2);

                    nextGeneration.Add(child1);
                    nextGeneration.Add(child2);

                }
                currentGeneration = nextGeneration;
                generationNumber++;
            }
            return bestSolution;
        }
        public void CrossOverParents(Individual parent1, Individual parent2, out Individual child1, out Individual child2)
        {
            child1 = parent1.DeepClone();  
            child2 = parent2.DeepClone();

            if(Randomizer.GetDoubleFromZeroToOne() < crossoverRate)
            {
                var exchangePoint = Randomizer.IntBetween(1, parent2.paths.Count());
                List<Paths> newChildOnePaths = new List<Paths>();
                List<Paths> newChildTwoPaths = new List<Paths>();
                for (int i = 0; i < exchangePoint; i++)
                {
                    newChildOnePaths.Add(parent1.paths[i]);
                    newChildTwoPaths.Add(parent2.paths[i]);
                }
                for (int i = exchangePoint; i < parent2.paths.Count(); i++)
                {
                    newChildOnePaths.Add(parent2.paths[i]);
                    newChildTwoPaths.Add(parent1.paths[i]);
                }
                child1.paths = newChildOnePaths;
                child2.paths = newChildTwoPaths;
            }
        }

        public void Mutation(Individual i)
        {
            if (Randomizer.GetDoubleFromZeroToOne() < mutationRate)
            {
                foreach(Paths p in i.paths)
                {
                    int randomIndex = Randomizer.IntLessThan(p.Segments.Count());
                    Segment segment = p.Segments[randomIndex];

                    //Comment it if mutation start to have problems
                    //if (Randomizer.GetDoubleFromZeroToOne() > 1 - mutationRate)
                    //{
                    //    if (segment.distance > 2)
                    //    {
                    //        var newDistance = Randomizer.IntBetween(1, segment.distance);
                    //        segment.distance -= newDistance;
                    //        p.Segments.Insert(randomIndex + 1, new Segment(segment.orientation, newDistance));
                    //        randomIndex += 1;
                    //        segment = p.Segments[randomIndex];
                    //    }
                    //}

                    string ori1 = "";
                    string ori2 = "";
                    string orient = "";
                    string againstOrient = "";

                    if (segment.orientation == "U" || segment.orientation == "D")
                    {
                        
                        if (Randomizer.GetDoubleFromZeroToOne() >= 0.5){ orient = "L"; } else { orient = "R"; }
                        if(orient == "L") { againstOrient = "R"; } else { againstOrient = "L"; }
                        ori1 = "U"; ori2 = "D";
                    }else 
                    {
                        if (Randomizer.GetDoubleFromZeroToOne() >= 0.5) { orient = "D"; } else { orient = "U"; }
                        if (orient == "D") { againstOrient = "U"; } else { againstOrient = "D"; }
                        ori1 = "L"; ori2 = "R";
                    }

                    if (randomIndex > 0 && (p.Segments[randomIndex -1].orientation != ori1 || p.Segments[randomIndex - 1].orientation != ori2)) 
                    {
                        if (p.Segments[randomIndex - 1].orientation == againstOrient) {p.Segments[randomIndex - 1].distance += 1 * -1;} 
                        else { p.Segments[randomIndex - 1].distance += 1 * 1; }
                        if(p.Segments[randomIndex - 1].distance == 0) { p.Segments.RemoveAt(randomIndex - 1); randomIndex -= 1; }
                    } else { p.Segments.Insert(randomIndex, new Segment(orient, 1)); randomIndex += 1; }

                    if(randomIndex < p.Segments.Count() - 1 && (p.Segments[randomIndex + 1].orientation != ori1 || p.Segments[randomIndex + 1].orientation != ori2))
                    {
                        if (p.Segments[randomIndex + 1].orientation == orient) { p.Segments[randomIndex + 1].distance += 1 * - 1; }
                        else { p.Segments[randomIndex + 1].distance += 1 * 1; }
                        if(p.Segments[randomIndex+1].distance == 0) { p.Segments.RemoveAt(randomIndex + 1); }
                         
                    } else { p.Segments.Insert(randomIndex + 1, new Segment(againstOrient, 1)); }

                    int pos = 0;
                    while(true)
                    {
                        if (pos >= p.Segments.Count()) { break; }

                        if(p.Segments[pos].distance == 0) { p.Segments.RemoveAt(pos); continue; }

                        if(pos > 0 && p.Segments[pos - 1].orientation == p.Segments[pos].orientation) 
                        { 
                            p.Segments[pos - 1].distance += p.Segments[pos].distance;
                            p.Segments.RemoveAt(pos);
                            continue;
                        }
                        
                        if(pos > 0 && p.Segments[pos].orientation == p.AgainstDirection(p.Segments[pos-1].orientation))
                        {
                            if(p.Segments[pos-1].distance < p.Segments[pos].distance)
                            {
                                p.Segments[pos - 1].orientation = p.Segments[pos].orientation;
                                p.Segments[pos - 1].distance = p.Segments[pos].distance - p.Segments[pos - 1].distance;
                            } else if(p.Segments[pos -1].distance > p.Segments[pos].distance)
                            {
                                p.Segments[pos - 1].distance -= p.Segments[pos].distance;
                            } else
                            {
                                p.Segments.RemoveAt(pos);
                                pos -= 1;
                            }

                            p.Segments.RemoveAt(pos);
                            pos -= 1;
                        }

                        pos += 1;
                    }
                }
            }
        }
        public Individual SelectCandidateViaRoulette()
        {
            double probabilitySum = 0;
            List<double> probabilityList = new List<double>();
            foreach (Individual i in currentGeneration)
            {
                probabilitySum += (1 - (double)i.fitnessScore / (double)totalFitnessThisGeneration)/((double)populationSize-1);
                probabilityList.Add(probabilitySum);
            }
            double randomValue = Randomizer.GetDoubleFromZeroToOne();
            for(int i = 0; i< probabilityList.Count(); i++) 
            {
                if(randomValue < probabilityList[i])
                {
                    return currentGeneration[i];
                }
            }
            return currentGeneration[populationSize - 1];
        }

        public Individual SelectCandidateViaTournament()
        {
            List<int> selectedIndividuals = new List<int>();
            List<int> fitnessOfSelected = new List<int>();

            int topParticicpant = 0;
            int topFitness = int.MaxValue;

            Individual bestFound = null;
            for (int i = 0; i < tourneySize; i++)
            {
                int randomIndex = Randomizer.IntLessThan(populationSize);
                //selectedIndividuals.ToList().Add(randomIndex);
                //for(int j = 0; j < selectedIndividuals.Length; j++)
                //{
                //while(randomIndex == selectedIndividuals[j])
                //{
                //randomIndex = Randomizer.IntLessThan(populationSize);
                //}
                //}
                Individual randomSolution = currentGeneration[randomIndex];
                selectedIndividuals.Add(randomIndex);
                fitnessOfSelected.Add(randomSolution.fitnessScore);
            }

            for(int i = 1; i < selectedIndividuals.Count(); i++) 
            { 
                if(fitnessOfSelected[i] < topFitness)
                {
                    topFitness = fitnessOfSelected[i];
                    topParticicpant = selectedIndividuals[i];
                }
            }

            bestFound = currentGeneration[topParticicpant];
            return bestFound;
        }
    }
}
