using System;

namespace GeneticAlgorithmPCB.Model
{
    class Randomizer
    {
        private static Random randomizer = new Random();

        public static int IntBetween(int lower, int upper)
        {
            return randomizer.Next(lower, upper);
        }

        public static int IntLessThan(int upper)
        {
            return randomizer.Next(upper);
        }

        public static double GetDoubleFromZeroToOne()
        {
            return randomizer.NextDouble();
        }
    }
}
