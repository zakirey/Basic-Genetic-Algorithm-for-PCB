using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmPCB.Model
{
    class Individual
    {
        public List<Paths> paths { get; set; }
        public int fitnessScore { get; set; }
        public Individual()
        {
            paths = new List<Paths>();
            fitnessScore = 0;
        }

        public Individual(List<Paths> paths)
        {
            this.paths = paths;
            fitnessScore = 0;
        }

        public static bool LineIntersection(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4 )
        {
            double a1 = y2 - y1;
            double b1 = x1 - x2;
            double a2 = y4 - y3;
            double b2 = x3 - x4;

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int FitnessScore(int boardWidth, int boardHeight)
        {
            double sumOfDistance = 0;
            double sumOfSegments = 0;
            double crosses = 0;
            double sumOfDistanceOut = 0;
            double sumOfSegmentsOut = 0;

            List<Object> lines = new List<Object>();
            
            foreach(Paths path in paths)
            {
                sumOfSegments += path.Segments.Count();

                int x = path.X1;
                int y = path.Y1;
                foreach(Segment segment in path.Segments)
                {
                    sumOfDistance += segment.distance;

                    int x1 = x, y1 = y;
                    if(segment.orientation == "U") { y1 -= segment.distance; }
                    if (segment.orientation == "D") { y1 += segment.distance; }
                    if (segment.orientation == "L") { x1 -= segment.distance; }
                    if (segment.orientation == "R") { x1 += segment.distance; }
                    Object[] line = new Object[5];
                    line[0] = x;
                    line[1] = y;
                    line[2] = x1;
                    line[3] = y1;
                    line[4] = path;
                    lines.Add(line);

                    for( int i = 0; i < segment.distance; i++)
                    {
                        if (x <=0 || y <=0 || x > boardWidth || y> boardHeight){ sumOfDistanceOut += 1;}
                        if (segment.orientation == "U") { y -= 1; }
                        if (segment.orientation == "D") { y += 1; }
                        if (segment.orientation == "L") { x -= 1; }
                        if (segment.orientation == "R") { x += 1; }
                    }

                    if(x <= 0 || x > boardWidth || y <= 0 || y >= boardHeight) { sumOfSegmentsOut += 1; }
                    else
                    {
                        if (segment.orientation == "U" && y + segment.distance <= boardHeight) { sumOfSegmentsOut += 1; }
                        if (segment.orientation == "D" && y - segment.distance <= 0) { sumOfSegmentsOut += 1; }
                        if (segment.orientation == "L" && x + segment.distance <= boardWidth) { sumOfSegmentsOut += 1; }
                        if (segment.orientation == "R" && x - segment.distance <= 0) { sumOfSegmentsOut += 1; }
                    }
                }
            }

            for (int i = 0; i < lines.Count() - 1; i++)
            {
                for (int j = i + 1; j < lines.Count(); j++)
                {
                    Object[] line = lines.ToArray();
                    Object[] line1 = (Object[])line[i];
                    Object[] line2 = (Object[])line[j];
                    if (LineIntersection((int) line1[0], (int) line1[1], (int) line1[2], (int) line1[3],
                        (int) line2[0], (int) line2[1], (int) line2[2], (int) line2[3]))
                    {
                        crosses += 1;
                    }
                }
            }

            if(sumOfDistanceOut != 0)
            {
                sumOfDistanceOut += 1;
            }
            else
            {
                sumOfDistanceOut = 0;
            }
            double[] first_array = { crosses, sumOfDistance, sumOfSegments, sumOfSegmentsOut, sumOfDistanceOut };
            double[] second_array = { 25, 0, 0.5, 15, 15 };
            List<int> result = new List<int>();
            for (int i = 0; i < first_array.Length; i++)
            {

                result.Add((int)(first_array[i] * second_array[i]));
            }
            return result.Sum();

        }

        public override string ToString()
        {
            string pathstr = " ";
            foreach (var path in paths)
            {
                pathstr += path.ToString();
            }
            return "Fitness: " + fitnessScore + " [ Paths: " + pathstr + "]";
        }

        public Individual DeepClone()
        {
            return new Individual(paths);
        }

    }
}
