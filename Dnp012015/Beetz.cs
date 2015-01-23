
namespace Dnp012015.Beetz
{
    using System;

    using contest.submission.contract;
    [Serializable]
    public sealed class Solution : IDnp1501Solution
    {
        private const int _DIM_X = 1024, _DIM_Y = 1024;
        private Point _current;
        private Go[,] _map;

        public void NextStep()
        {
            var c = _current;

            switch (_map[c.x, c.y])
            {
                case Go.North:
                    c.y--;
                    break;
                case Go.East:
                    c.x++;
                    break;
                case Go.West:
                    c.x--;
                    break;
                case Go.South:
                    c.y++;
                    break;
                case Go.NorthWest:
                    c.y--;
                    c.x--;
                    break;
                case Go.SouthWest:
                    c.y++;
                    c.x--;
                    break;
                case Go.NorthEast:
                    c.y--;
                    c.x++;
                    break;
                case Go.SouthEast:
                    c.y++;
                    c.x++;
                    break;
            }

            MakeMove(c);
        }

        public void Start(BoolArray ground, Point startpoint, Point endpoint)
        {
            var data = ground.Data;
            var sx = startpoint.x;
            var sy = startpoint.y;
            var ex = endpoint.x;
            var ey = endpoint.y;

            var map = new Go[_DIM_X, _DIM_Y];
            map[ex, ey] = Go.NoMore;

            var @in = 0;
            var @out = 0;

            var queue = new Pnt[_DIM_X * _DIM_Y];
            queue[@in++] = new Pnt(ex, ey);

            while (@in != @out)
            {
                var pt = queue[@out++];

                var x = pt.X;
                var y = pt.Y;

                if (sx == x && sy == y)
                {
                    break;
                }

                var n = y - 1;
                var w = x - 1;
                var o = x + 1;
                var s = y + 1;

                if (n >= 0 && !data[x, n] && map[x, n] == Go.Somewhere)
                {
                    map[x, n] = Go.South;
                    queue[@in++] = new Pnt(x, n);
                }
                if (w >= 0 && !data[w, y] && map[w, y] == Go.Somewhere)
                {
                    map[w, y] = Go.East;
                    queue[@in++] = new Pnt(w, y);
                }
                if (o < _DIM_X && !data[o, y] && map[o, y] == Go.Somewhere)
                {
                    map[o, y] = Go.West;
                    queue[@in++] = new Pnt(o, y);
                }
                if (s < _DIM_Y && !data[x, s] && map[x, s] == Go.Somewhere)
                {
                    map[x, s] = Go.North;
                    queue[@in++] = new Pnt(x, s);
                }
                if (w >= 0 && n >= 0 && !data[w, n] && map[w, n] == Go.Somewhere)
                {
                    map[w, n] = Go.SouthEast;
                    queue[@in++] = new Pnt(w, n);
                }
                if (w >= 0 && s < _DIM_Y && !data[w, s] && map[w, s] == Go.Somewhere)
                {
                    map[w, s] = Go.NorthEast;
                    queue[@in++] = new Pnt(w, s);
                }
                if (o < _DIM_X && n >= 0 && !data[o, n] && map[o, n] == Go.Somewhere)
                {
                    map[o, n] = Go.SouthWest;
                    queue[@in++] = new Pnt(o, n);
                }
                if (o < _DIM_X && s < _DIM_Y && !data[o, s] && map[o, s] == Go.Somewhere)
                {
                    map[o, s] = Go.NorthWest;
                    queue[@in++] = new Pnt(o, s);
                }
            }

            _map = map;
            _current = new Point { x = startpoint.x, y = startpoint.y };

            NextStep();
        }

        public event Action<Point> MakeMove;

        private enum Go
        {
            Somewhere,
            NoMore,
            North,
            West,
            East,
            South,
            NorthWest,
            NorthEast,
            SouthWest,
            SouthEast
        }

        private struct Pnt
        {
            public readonly int X;
            public readonly int Y;

            public Pnt(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
