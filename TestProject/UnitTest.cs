namespace TestProject
{
    using System;
    using System.Diagnostics;
    using contest.submission.contract;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void Buchmann()
        {
            this.Teste(typeof(Dnp012015.Buchmann.Solution));
        }

        [TestMethod]
        public void Beetz()
        {
            this.Teste(typeof(Dnp012015.Beetz.Solution));
        }

        [TestMethod]
        public void Mojado()
        {
            this.Teste(typeof(Dnp012015.Mojado.Solution));
        }

        [TestMethod]
        public void Buchmann_Gegen_Beetz()
        {
            this.VergleicheTeilnehmer(typeof(Dnp012015.Buchmann.Solution), typeof(Dnp012015.Beetz.Solution), 20);
        }


        public void Teste(Type teilnehmer)
        {
            Trace.WriteLine("Teilnehmer: " + teilnehmer.Namespace);
            Trace.WriteLine("");

            foreach (var board in TestboardGenerator.Boards())
            {
                Teste(teilnehmer, board);
            }
        }

        private static void Teste(Type teilnehmer, Testboard testboard)
        {
            var ergebnis = RunTest(testboard.Board, testboard.Start, testboard.Ende, (IDnp1501Solution)Activator.CreateInstance(teilnehmer));
            Trace.WriteLine(testboard.Name + "   Dauer:  " + ergebnis.Item2);
            Assert.AreEqual(testboard.Schritte, ergebnis.Item1, " Board: "+ testboard.Name);
        }


        public void VergleicheTeilnehmer(Type teilnehmer1, Type teilnehmer2, int durchläufe)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            Trace.WriteLine("Teilnehmer1: " + teilnehmer1.Namespace);
            Trace.WriteLine("Teilnehmer2: " + teilnehmer2.Namespace);
            Trace.WriteLine("");

            foreach (var board in TestboardGenerator.Boards())
            {
                Vergleiche(teilnehmer1, teilnehmer2, durchläufe, board);
            }
        }

        private static void Vergleiche(Type teilnehmer1, Type teilnehmer2, int durchläufe, Testboard testboard)
        {
            double dauer1 = 0;
            double dauer2 = 0;
            for (int i = 0; i < durchläufe; i++)
            {
                var ergebnis1 = RunTest(testboard.Board, testboard.Start, testboard.Ende, (IDnp1501Solution)Activator.CreateInstance(teilnehmer1));
                var ergebnis2 = RunTest(testboard.Board, testboard.Start, testboard.Ende, (IDnp1501Solution)Activator.CreateInstance(teilnehmer2));

                dauer1 += ergebnis1.Item2;
                dauer2 += ergebnis2.Item2;
            }
            Trace.WriteLine(testboard.Name + " Teilnehmer1: " + (dauer1 / durchläufe) + " Teilnehmer2: " + (dauer2 / durchläufe));
        }

        private static Tuple<int, double> RunTest(BoolArray board, Point start, Point ende,
            IDnp1501Solution algorithm)
        {
            var pos = 0;
            var sw = new Stopwatch();
            sw.Start();
            Point current = null;
            algorithm.MakeMove +=
                point =>
                {
                    current = point;
                    pos++;
                };
            algorithm.Start(board, start, ende);
            // sw.Restart();
            while (!current.IsEqual(ende))
            {
                algorithm.NextStep();
            }
            sw.Stop();
            return new Tuple<int, double>(pos, sw.Elapsed.TotalMilliseconds);
        }
    }
}
