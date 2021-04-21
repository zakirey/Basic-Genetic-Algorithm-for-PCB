using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmPCB.Model
{
    class Paths
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int Y1 { get; set; }
        public int Y2 { get; set; }
        public List<Segment> Segments { get; set; }
        public Paths(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
            Segments = new List<Segment>();
        }


        public string AgainstDirection(string dir)
        {
            if (dir == "U") { dir = "D"; }
            else if (dir == "D") { dir = "U"; }
            else if (dir == "R") { dir = "L"; }
            else if (dir == "L") { dir = "R"; }
            return dir;
        }

        public List<Segment> GetRandomSegments()
        {
            if (X1 == X2 && Y1 == Y2)
            {
                Segments =  new List<Segment>();
            }

            int x = X1;
            int y = Y1;
            string previousDirection = "";
            List<Segment> segments = new List<Segment>();

            int limit = Randomizer.IntBetween(2, 3);
            for (int i = 0; i < limit; i++)
            {
                if (x == X2 && y == Y2)
                {
                    Segments = segments;
                }

                List<string> directions = new List<string>() {"U", "D", "L", "R" };

                if (directions.Contains(previousDirection))
                {
                    directions.Remove(previousDirection);
                    directions.Remove(AgainstDirection(previousDirection));
                }

                int randomDirectionIndex = Randomizer.IntLessThan(directions.Count());
                string direction = directions[randomDirectionIndex];
                int distance = Randomizer.IntBetween(1, 4);

                Segment s = new Segment(direction, distance);
                segments.Add(s);

                if (direction == "U") { y -= distance; }
                else if (direction == "D") { y += distance; }
                else if (direction == "R") { x += distance; }
                else if (direction == "L") { x -= distance; }

                previousDirection = direction;
            }

            List<string> newDirections = new List<string>() { "U", "D", "L", "R" };
            newDirections.Remove(previousDirection);
            newDirections.Remove(AgainstDirection(previousDirection));

            int newRandomDirectionIndex = Randomizer.IntLessThan(newDirections.Count());
            string newDirection = newDirections[newRandomDirectionIndex];
            int newDistance = Randomizer.IntBetween(1, 4);
            bool addDirection = false;

            if ((previousDirection == "U" || previousDirection == "D") && x == X2)
            {
                segments.Add(new Segment(newDirection, newDistance));
                if( newDirection == "R") { x += newDistance * 1; } else { x += newDistance * -1; }
                addDirection = true;
            } else if ((previousDirection == "R" || previousDirection == "L") && y == Y2)
            {
                segments.Add(new Segment(newDirection, newDistance));
                if (newDirection == "D") { y += newDistance * 1; } else { y += newDistance * -1; }
                addDirection = true;
            }

            int horizontal = X2 - x;
            int vertical= Y2 - y;

            string dir = "";
            if (horizontal > 0 )
            {
                dir = "R";
            } else
            {
                dir = "L";
            }

            Segment c1 = new Segment(dir, Math.Abs(horizontal));

            if (vertical> 0)
            {
                dir = "D";
            }
            else
            {
                dir = "U";
            }

            Segment c2 = new Segment(dir, Math.Abs(vertical));

            if (!addDirection){ newDirection = previousDirection; }
            if (newDirection == "U" || newDirection == "D") { segments.Add(c1); segments.Add(c2); }
            else if (newDirection == "R" || newDirection == "L") { segments.Add(c2); segments.Add(c1); }

            return segments;
        }

        public override string ToString()
        {
            string seg = " ";
            foreach (var segment in Segments)
            {
                seg += segment.ToString();
            }
            return " { X: " + X1 + " Y: " + Y1 + " X2: " + X2 + " Y2: " + Y2 + " Segments: " + seg;
        }
    }
}

