using contest.submission.contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dnp012015.Hlawatsch
{

    [Serializable]
    public class Solution : IDnp1501Solution
    {
        Point startpoint, endpoint;
        Point[] path;
        int position = 0;
        int dimx = 0;
        int dimy = 0;

        public void Start(BoolArray ground, Point startpoint, Point endpoint)
        {
            if (ground == null) throw new ArgumentNullException("ground");
            if (startpoint == null) throw new ArgumentNullException("startpoint");
            if (endpoint == null) throw new ArgumentNullException("endpoint");
            //if (startpoint.IsEqual(endpoint)) throw new ArgumentException("Ziel gleich Start - das gibt die API nicht her, da kein Move mehr nötig ist.");

            dimx = ground.Data.GetLength(0);
            dimy = ground.Data.GetLength(1);

            //if (!IsValid(startpoint, ground)) throw new ArgumentException("kein gültiger Startpunkt");
            //if (!IsValid(endpoint, ground))  throw new ArgumentException("kein gültiger Zielpunkt");


            this.startpoint = startpoint;
            this.endpoint = endpoint;

            Point[,] tree = GeneratePathTree(ground);

            IEnumerable<Point> reversePath = ExtractPath(tree);

            path = reversePath.Reverse().ToArray();

            NextStep();
        }

        private IEnumerable<Point> ExtractPath(Point[,] tree)
        {
            Point current = endpoint;

            do
            {
                yield return current;

                current = tree[current.x, current.y];
            }
            while (!current.IsEqual(startpoint));
        }

        private Point[,] GeneratePathTree(BoolArray ground)
        {
            //System.Diagnostics.Debugger.Break();

            var result = new Point[dimx, dimy];
            result[startpoint.x, startpoint.y] = startpoint;

            var previousLevel = new List<Point>() { startpoint };

            while (true)
            {
                var newLevel = new List<Point>();

                foreach (Point p in previousLevel)
                {
                    foreach (Point neighbour in GetNeighbours(p).Where(x => IsValid(x, ground)))
                    {
                        if (result[neighbour.x, neighbour.y] == null)
                        {
                            result[neighbour.x, neighbour.y] = p;

                            if (neighbour.IsEqual(endpoint)) return result;

                            newLevel.Add(neighbour);
                        }
                    }
                }

                //Console.Write('.');

                if (newLevel.Count == 0) throw new ArgumentException("Auf diesem Gelände gibt es keinen gültigen Weg.");

                previousLevel = newLevel;
            };
        }

        private bool IsValid(Point p, BoolArray ground)
        {
            bool result = p.x >= 0 && p.x < dimx
                && p.y >= 0 && p.y < dimy
                && !ground.IsTrue(p.x, p.y);

            return result;
        }

        private IEnumerable<Point> GetNeighbours(Point p)
        {
            yield return new Point() { x = p.x, y = p.y + 1 };
            yield return new Point() { x = p.x + 1, y = p.y + 1 };
            yield return new Point() { x = p.x + 1, y = p.y };
            yield return new Point() { x = p.x + 1, y = p.y - 1 };
            yield return new Point() { x = p.x, y = p.y - 1 };
            yield return new Point() { x = p.x - 1, y = p.y - 1 };
            yield return new Point() { x = p.x - 1, y = p.y };
            yield return new Point() { x = p.x - 1, y = p.y + 1 };
        }

        public void NextStep()
        {
            MakeMove(path[position++]);
        }

        public event Action<Point> MakeMove;
    }
}

