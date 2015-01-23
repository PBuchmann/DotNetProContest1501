namespace Dnp012015.Mojado
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using contest.submission.contract;

    class Step
    {
        public Point Point; // The point coordinate in the 1024x1024 ground 
        public int StepCount;        // Every Step has a number representing the number  
        // of steps needed going from the endpoint to the startpoint whereas Step 
    }

    public interface IPathFinder
    {
        /** Algorithm: 
         *  
          * It's more or less the same as described here: http://en.wikipedia.org/wiki/Pathfinding#Sample_algorithm 
          * 
          * - Create a list of the eight adjacent cells, with a counter variable of the current element's counter variable + 1 
          *  
          * - Check all cells in each list for the following two conditions: 
          *      * If the cell is a wall, remove it from the list 
          *      * If there is an element in the main list with the same coordinate and an equal or higher counter 
          *  
          * - Add all remaining cells in the list to the end of the main list 
          *  
          * - Go to the next item in the list and repeat until start point is reached  
          *  
         **/
        Point[] FindAPath();
    }

    class PlayGround
    {
        public BoolArray Ground;
        public Point Startpoint;
        public Point Endpoint;


        const int PlayGroundDimMaxX = 1024;
        const int PlayGroundDimMaxY = 1024;


        private readonly int[,] _stepData = new int[PlayGroundDimMaxX, PlayGroundDimMaxY];


        public PlayGround(BoolArray ground, Point startpoint, Point endpoint)
        {
            this.Ground = ground;
            this.Startpoint = startpoint;
            this.Endpoint = endpoint;


            InitializeWithZero();
        }


        public bool IsStepUsedBefore(Point newStep)
        {
            //this is too slow: 
            //return pathSteps.Any(storedSteps => storedSteps.Point.IsEqual(newStep)); 


            //broke down to O(1) instead of O(x^2) by using a "hash-array" 
            return IsPointUsed(newStep);
        }


        public void MapNewStep(List<Step> pathSteps, Point step, int stepNumber)
        {
            pathSteps.Add(new Step { Point = step, StepCount = stepNumber });
            SetStepNumberToPoint(stepNumber, step);
        }


        public List<Point> FilterOutInvalidSteps(List<Point> possibleSteps)
        {
            var filteredSteps = new List<Point>(8);
            filteredSteps.AddRange(possibleSteps.
                Where(point => !IsOutsideOfTheGround(point)).   //this check must be before.. 
                Where(point => !IsPointAWall(point)));          //..checking the actual array 
            return filteredSteps;
        }


        private static bool IsOutsideOfTheGround(Point p)
        {
            return ((p.x < 0) || (p.x > PlayGroundDimMaxX - 1) ||
                    (p.y < 0) || (p.y > PlayGroundDimMaxY - 1));
        }


        private bool IsPointAWall(Point p)
        {
            // return _ground.IsTrue(p.x, p.y);  // Gives an error others also found (http://www.dotnetpro.de/newsgroups/newsgroupthread.aspx?id=8779) 
            return Ground.Data[p.x, p.y];
        }


        private bool IsPointUsed(Point p)
        {
            return _stepData[p.x, p.y] > 0;
        }


        private void SetStepNumberToPoint(int stepCount, Point p)
        {
            _stepData[p.x, p.y] = stepCount;
        }


        private void InitializeWithZero()
        {
            for (var i = 0; i < PlayGroundDimMaxX; i++)
            {
                for (var j = 0; j < PlayGroundDimMaxY; j++)
                {
                    _stepData[i, j] = 0;
                }
            }
        }
    }


    public class PathFinder : IPathFinder
    {
        private readonly PlayGround _playGround;


        public PathFinder(BoolArray ground, Point startpoint, Point endpoint)
        {
            _playGround = new PlayGround(ground, startpoint, endpoint);
        }


        /** Algorithm: 
         *  
         * It's more or less the same as described here: http://en.wikipedia.org/wiki/Pathfinding#Sample_algorithm 
         * 
         * - Create a list of the eight adjacent cells, with a counter variable of the current element's counter variable + 1 
         *  
         * - Check all cells in each list for the following two conditions: 
         *      * If the cell is a wall, remove it from the list 
         *      * If there is an element in the main list with the same coordinate and an equal or higher counter 
         *  
         * - Add all remaining cells in the list to the end of the main list 
         *  
         * - Go to the next item in the list and repeat until start is reached  
         *  
        **/
        public Point[] FindAPath()
        {
            Console.WriteLine("Robot uses his tricorder to find a path...");
            List<Step> pathSteps = new List<Step>();
            var stepNumber = 0;


            _playGround.MapNewStep(pathSteps, _playGround.Endpoint, stepNumber);


            do
            {
                stepNumber++;
                var lastStepNumber = stepNumber - 1;


                for (var i = 0; i < pathSteps.Count; i++)
                {
                    if (pathSteps[i].StepCount != lastStepNumber) continue;


                    //the eight adjacent cells 
                    List<Point> everyDirectionsSteps = GetStepsForEveryDirections(pathSteps[i].Point);


                    //Check.. If the cell is a wall, remove it from the list 
                    List<Point> possibleSteps = _playGround.FilterOutInvalidSteps(everyDirectionsSteps);


                    //Check.. If there is an element in the main list with the same coordinate 
                    foreach (var step in possibleSteps.Where(step => !_playGround.IsStepUsedBefore(step)))
                    {
                        _playGround.MapNewStep(pathSteps, step, stepNumber);
                    }
                }


                if (stepNumber % 100 == 0) Console.Write("\r{0}-th step", stepNumber);
            } while (!_playGround.IsStepUsedBefore(_playGround.Startpoint)); //Go to the next item in the list and repeat until start is reached 


            return ChooseAPathLine(pathSteps);
        }


        private Point[] ChooseAPathLine(List<Step> pathSteps)
        {
            int stepCountMax = pathSteps.Max(step => step.StepCount);
            int stepCount = stepCountMax;


            Point[] choosenPath = new Point[stepCount + 1];


            choosenPath[stepCount] = _playGround.Startpoint;


            stepCount--;


            Console.WriteLine(Environment.NewLine + "Robot's tricorder found several paths with " + stepCountMax + " steps.");
            Console.WriteLine("Robot now chooses one of them... ");


            for (var i = stepCount; i > 0; i--)
            {
                List<Step> stepsWithNextStepCount = pathSteps.FindAll(step => step.StepCount == i);
                List<Point> stepsForEveryDirections = GetStepsForEveryDirections(choosenPath[i + 1]);


                choosenPath[i] = IntersectAndChooseRandom(stepsForEveryDirections, stepsWithNextStepCount);
                if (i % 100 == 0) Console.Write(".");
            }
            Console.Write(Environment.NewLine);
            choosenPath[0] = _playGround.Endpoint;
            Array.Reverse(choosenPath);
            return choosenPath;
        }


        // chooses randomely one step from from the adjacent steps that is also one in a path to the endpoint 
        private static Point IntersectAndChooseRandom(IEnumerable<Point> stepsForEveryDirections, List<Step> stepsWithNextStepCount)
        {
            List<Point> intersectedPoints = new List<Point>(8);
            foreach (Point point in stepsForEveryDirections)
            {
                for (var j = stepsWithNextStepCount.Count - 1; j >= 0; j--)
                {
                    if (point.IsEqual(stepsWithNextStepCount[j].Point))
                    {
                        intersectedPoints.Add(point);
                    }
                }
            }
            var rnd = new Random();
            return intersectedPoints[rnd.Next(intersectedPoints.Count)];
        }


        static List<Point> GetStepsForEveryDirections(Point p)
        {
            List<Point> possiblePoints = new List<Point>(8) 
             { 
                 new Point {x = p.x - 1, y = p.y - 1}, //North-West 
                 new Point {x = p.x    , y = p.y - 1}, //North 
                 new Point {x = p.x + 1, y = p.y - 1}, //North-East 
                 new Point {x = p.x - 1, y = p.y    }, //West 
                 new Point {x = p.x + 1, y = p.y    }, //East 
                 new Point {x = p.x - 1, y = p.y + 1}, //South-West 
                 new Point {x = p.x    , y = p.y + 1}, //South 
                 new Point {x = p.x + 1, y = p.y + 1}  //South-East 
             };
            return possiblePoints;
        }
    }

    [Serializable]
    public class Solution : IDnp1501Solution
    {
        private int _stepNumber;
        private Point[] Path { get; set; }


        public void Start(BoolArray ground, Point startpoint, Point endpoint)
        {
            IPathFinder pathFinder = new PathFinder(ground, startpoint, endpoint);
            Path = pathFinder.FindAPath();

            _stepNumber = 1; // 0 would be the startpoint 


            NextStep();
        }


        public void NextStep()
        {


            MakeMove(Path[_stepNumber++]);
        }


        public event Action<Point> MakeMove;
    }

}
