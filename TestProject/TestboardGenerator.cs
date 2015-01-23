namespace TestProject
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using contest.submission.contract;
    using Properties;
    using Point = contest.submission.contract.Point;

    public static class TestboardGenerator
    {
        public static IEnumerable<Testboard> Boards()
        {
            yield return GenerateLeeresBoard(new BoolArray());
            yield return GenerateEinSchritt(new BoolArray());
            yield return GenerateZufall1(new BoolArray());
            yield return GenerateZufall2(new BoolArray());
            yield return GenerateZufall3(new BoolArray());
            yield return GenerateZufallsweg(new BoolArray());
            yield return GenerateAufgabe(new BoolArray());
            yield return GenerateLabyrinth(new BoolArray());
            yield return GenerateVerschmutztesLabyrinth(new BoolArray());
            yield return GenerateVerschmutztesLabyrinth1(new BoolArray());
            yield return GenerateMaximalerWeg(new BoolArray());
            yield return GenerateBoardLabyrinth1(new BoolArray());
            yield return GenerateBoardLabyrinthTricky(new BoolArray());
            yield return GenerateBoardLabyrinthTricky2(new BoolArray());
            yield return GenerateTestboard(new BoolArray());
        }

        private static readonly Point Startpunkt = new Point { x = 0, y = 0 };
        private static readonly Point Endpunkt = new Point { x = 1023, y = 1023 };

        public static Testboard GenerateLeeresBoard(BoolArray board)
        {
            return new Testboard { Name = "Leeres Board", Board = board, Start = Startpunkt, Ende = Endpunkt, Schritte = 1023 };
        }

        public static Testboard GenerateEinSchritt(BoolArray board)
        {
            return new Testboard { Name = "Ein Schritt", Board = board, Start = Startpunkt, Ende = new Point { x = 1, y = 1 }, Schritte = 1 };
        }

        public static Testboard GenerateAufgabe(BoolArray board)
        {
            var p = new Point { x = 900, y = 900 };
            const int sizex = 10;
            const int sizey = 10;

            for (int i = p.x; i < p.x + sizex; i++)
            {
                for (int j = p.y; j < p.y + sizey; j++)
                {
                    board.Data[i, j] = true;
                }
            }
            return new Testboard { Name = "Aufgabe", Board = board, Start = Startpunkt, Ende = Endpunkt, Schritte = 1033 };
        }

        public static Testboard GenerateTestboard(BoolArray board)
        {
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    for (int k = 0; k < 30; k++)
                    {
                        for (int l = 0; l < 30; l++)
                        {
                            board.Data[i * 32 + k + 1, j * 32 + l + 1] = true;
                        }
                    }
                }
            }
            board.Data[1023, 1023] = false;
            return new Testboard { Name = "Testboard", Board = board, Start = Startpunkt, Ende = Endpunkt, Schritte = 1921 };
        }

        private static BoolArray GetBoardZufall(BoolArray board)
        {
            var random = new Random(17);
            for (var i = 0; i < 60; i++) // Anzahl der Hindernsse
            {
                int x = random.Next(1024);
                int y = random.Next(1024);
                for (int j = 0; j < 20000; j++) // Länge der Linie
                {
                    board.Data[x, y] = true;
                    if (random.Next(2) == 0)
                    {
                        x = x + random.Next(3) - 1;
                    }
                    else
                    {
                        y = y + random.Next(3) - 1;
                    }
                    if (x < 0) x = 1023;
                    if (y < 0) y = 1023;
                    if (x > 1023) x = 0;
                    if (y > 1023) y = 0;
                }
            }
            return board;
        }

        public static Testboard GenerateZufall1(BoolArray board)
        {
            return new Testboard { Name = "Zufall1", Board = GetBoardZufall(board), Start = Startpunkt, Ende = Endpunkt, Schritte = 1489 };
        }

        public static Testboard GenerateZufall2(BoolArray board)
        {
            return new Testboard { Name = "Zufall2", Board = GetBoardZufall(board), Start = new Point { x = 120, y = 900 }, Ende = new Point { x = 823, y = 183 }, Schritte = 1356 };
        }

        public static Testboard GenerateZufall3(BoolArray board)
        {
            return new Testboard { Name = "Zufall3", Board = GetBoardZufall(board), Start = new Point { x = 590, y = 500 }, Ende = new Point { x = 488, y = 333 }, Schritte = 1107 };
        }

        public static Testboard GenerateZufallsweg(BoolArray board)
        {
            for (int i = 0; i < 1024; i++)
            {
                for (int j = 0; j < 1024; j++)
                {
                    board.Data[i, j] = true;
                }
            }
            var random = new Random(379);
            for (var i = 0; i < 10; i++) // Anzahl der Hindernisse
            {
                int x = random.Next(1024);
                int y = random.Next(1024);
                int x1 = random.Next(3) - 1;
                int y1 = random.Next(3) - 1;
                for (int j = 0; j < 10000; j++) // Länge der Linie
                {
                    board.Data[x, y] = false;
                    if (random.Next(5) == 0)
                    {
                        x1 = random.Next(3) - 1;
                        y1 = random.Next(3) - 1;
                    }
                    x += x1;
                    y += y1;

                    if (x < 0) x = 1023;
                    if (y < 0) y = 1023;
                    if (x > 1023) x = 0;
                    if (y > 1023) y = 0;
                }
            }

            return new Testboard { Name = "Zufallsweg", Board = board, Start = Startpunkt, Ende = Endpunkt, Schritte = 2172 };
        }

        public static Testboard GenerateMaximalerWeg(BoolArray board)
        {
            for (int x = 1; x < 1024 - 2; x += 4)
            {
                for (int y = 0; y < 1024 - 1; y++)
                {
                    board.Data[x, y] = true;
                    board.Data[x + 2, y + 1] = true;
                }
            }
            return new Testboard { Name = "Maximaler Weg", Board = board, Start = Startpunkt, Ende = new Point { x = 1023, y = 0 }, Schritte = 523776 };
        }

        public static Testboard GenerateLabyrinth(BoolArray board)
        {
            var b = Resources.labyrinth;

            const int scale = 20;
            int shiftX = (1024 - b.Width * scale) / 2;
            int shiftY = (1024 - b.Height * scale) / 2;

            for (int i = 0; i < b.Width; i++)
            {
                for (int k = 0; k < b.Height; k++)
                {
                    var pixel = b.GetPixel(i, k);

                    if (pixel.R < 128 && pixel.G < 128 && pixel.B < 128)
                    {
                        for (int x = 0; x < scale; x++)
                        {
                            for (int y = 0; y < scale; y++)
                            {
                                board.Data[shiftX + i * scale + x, shiftY + k * scale + y] = true;
                            }
                        }
                    }
                }
            }
            return new Testboard { Name = "Labyrinth", Board = board, Start = new Point { x = 511, y = 0 }, Ende = new Point { x = 511, y = 511 }, Schritte = 20020 };
        }

        public static Testboard GenerateVerschmutztesLabyrinth(BoolArray board)
        {
            var b = Resources.labyrinth;

            const int scale = 20;
            int shiftX = (1024 - b.Width * scale) / 2;
            int shiftY = (1024 - b.Height * scale) / 2;

            for (int i = 0; i < b.Width; i++)
            {
                for (int k = 0; k < b.Height; k++)
                {
                    var pixel = b.GetPixel(i, k);

                    if (pixel.R < 128 && pixel.G < 128 && pixel.B < 128)
                    {
                        for (int x = 0; x < scale; x++)
                        {
                            for (int y = 0; y < scale; y++)
                            {
                                board.Data[shiftX + i * scale + x, shiftY + k * scale + y] = true;
                            }
                        }
                    }
                }
            }

            var r = new Random(42);

            for (int i = 0; i < 500000; i++)
            {
                board.Data[r.Next(0, 1024), r.Next(0, 1024)] = true;
            }
            return new Testboard { Name = "Verschmutztes Labyrinth", Board = board, Start = new Point { x = 511, y = 0 }, Ende = new Point { x = 511, y = 511 }, Schritte = 20853 };
        }

        public static Testboard GenerateVerschmutztesLabyrinth1(BoolArray board)
        {
            var b = Resources.labyrinth;

            const int scale = 20;
            int shiftX = (1024 - b.Width * scale) / 2;
            int shiftY = (1024 - b.Height * scale) / 2;

            for (int i = 0; i < b.Width; i++)
            {
                for (int k = 0; k < b.Height; k++)
                {
                    var pixel = b.GetPixel(i, k);

                    if (pixel.R < 128 && pixel.G < 128 && pixel.B < 128)
                    {
                        for (int x = 0; x < scale; x++)
                        {
                            for (int y = 0; y < scale; y++)
                            {
                                board.Data[shiftX + i * scale + x, shiftY + k * scale + y] = true;
                            }
                        }
                    }
                }
            }

            var r = new Random(42);

            for (int i = 0; i < 500000; i++)
            {
                board.Data[r.Next(0, 1024), r.Next(0, 1024)] = true;
            }
            return new Testboard { Name = "Verschmutztes Labyrinth 1", Board = board, Start = new Point { x = 1, y = 1 }, Ende = new Point { x = 1022, y = 1022 }, Schritte = 2001 };
        }

        public static Testboard GenerateBoardLabyrinth1(BoolArray board)
        {
            return new Testboard { Name = "Labyrinth1", Board = GenerateObstacleFromBitmap(Resources.Labyrinth1, board, 1), Start = Startpunkt, Ende = Endpunkt, Schritte = 1623 };
        }

        public static Testboard GenerateBoardLabyrinthTricky(BoolArray board)
        {
            return new Testboard { Name = "Labyrinth Tricky", Board = GenerateObstacleFromBitmap(Resources.LabyrinthTricky, board, 1), Start = Startpunkt, Ende = Endpunkt, Schritte = 3501 };
        }

        public static Testboard GenerateBoardLabyrinthTricky2(BoolArray board)
        {
            return new Testboard { Name = "Labyrinth Tricky2", Board = GenerateObstacleFromBitmap(Resources.LabyrinthTricky2, board, 1), Start = Startpunkt, Ende = Endpunkt, Schritte = 1896 };
        }

        public static BoolArray GenerateObstacleFromBitmap(Bitmap b, BoolArray ground, int scale)
        {

            int shiftX = (1024 - b.Width * scale) / 2;
            int shiftY = (1024 - b.Height * scale) / 2;


            for (int i = 0; i < b.Width; i++)
            {
                for (int k = 0; k < b.Height; k++)
                {
                    var pixel = b.GetPixel(i, k);


                    if (pixel.R >= 128 || pixel.G >= 128 || pixel.B >= 128) continue;
                    for (int x = 0; x < scale; x++)
                    {
                        for (int y = 0; y < scale; y++)
                        {
                            ground.Data[shiftX + i * scale + x, shiftY + k * scale + y] = true;
                        }
                    }
                }
            }
            return ground;
        }
    }

    public class Testboard
    {
        public string Name { get; set; }
        public BoolArray Board { get; set; }

        public Point Start { get; set; }
        public Point Ende { get; set; }
        public int Schritte { get; set; }
    }
}