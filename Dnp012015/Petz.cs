
using System;
using contest.submission.contract;

namespace Dnp012015.Petz
{
    using Point = contest.submission.contract.Point;

    [Serializable]
    public unsafe class Solution : IDnp1501Solution
    {
        private static Point currentposition;

        private readonly int[,] intArray = new int[1024, 1024];

        private const int GrenzeMin = -1;

        private const int GrenzeMax = 1024;

        private int aktX;

        private int aktY;

        private int altX;

        private int altY;

        private int aktWertig;

        public void Start(BoolArray ground, Point startpoint, Point endpoint)
        {
            currentposition = startpoint;
            var endx = endpoint.x;
            var endy = endpoint.y;
            aktX = startpoint.x;
            aktY = startpoint.y;
            intArray[endx, endy] = 1;
            this.HöhenEintragen(endx, endy, ground);

            this.NextStep();
        }

        private void HöhenEintragen(int x, int y, BoolArray ground)
        {
            var anzahlNodes = 1;
            var pointArray = new System.Drawing.Point[1024 * 1024 * 10];
            fixed (bool* gr = &ground.Data[0, 0])
            {
                fixed (System.Drawing.Point* pA = &pointArray[0])
                {
                    fixed (int* iA = &intArray[0, 0])
                    {

                        *(pA) = new System.Drawing.Point(x, y);
                        for (var i = 0; ; i++)
                        {
                            var p = *(pA + i);
                            x = p.X;
                            y = p.Y;

                            var xm1 = x - 1;
                            var xp1 = x + 1;
                            var ym1 = y - 1;
                            var yp1 = y + 1;
                            var offset = (x * 1024 + y);
                            var zeigerGr = gr + offset;
                            var zeiger = iA + offset;
                            var höhe = *(zeiger) + 1;
                            int vergleich;
                            // NORD
                            if (ym1 > GrenzeMin)
                            {
                                if (!*(zeigerGr - 1))
                                {
                                    vergleich = *(zeiger - 1);
                                    if (vergleich == 0)
                                    {
                                        *(zeiger - 1) = höhe;
                                        *(pA + anzahlNodes++) = new System.Drawing.Point(x, ym1);
                                    }
                                    else
                                    {
                                        if (vergleich > höhe)
                                        {
                                            *(zeiger - 1) = höhe;
                                            *(pA + anzahlNodes++) = new System.Drawing.Point(x, ym1);
                                        }
                                    }
                                }
                                //OST
                                if (xp1 < GrenzeMax)
                                {
                                    if (!*(zeigerGr + 1024 - 1))
                                    {
                                        vergleich = *(zeiger + 1024 - 1);
                                        if (vergleich == 0)
                                        {
                                            *(zeiger + 1024 - 1) = höhe;
                                            *(pA + anzahlNodes++) = new System.Drawing.Point(xp1, ym1);
                                        }
                                        else
                                        {
                                            if (vergleich > höhe)
                                            {
                                                *(zeiger + 1024 - 1) = höhe;
                                                *(pA + anzahlNodes++) = new System.Drawing.Point(xp1, ym1);
                                            }
                                        }
                                    }
                                }
                                ////WEST
                                if (xm1 > GrenzeMin)
                                {
                                    if (!*(zeigerGr - 1024 - 1))
                                    {
                                        vergleich = *(zeiger - 1024 - 1);
                                        if (vergleich == 0)
                                        {
                                            *(zeiger - 1024 - 1) = höhe;
                                            *(pA + anzahlNodes++) = new System.Drawing.Point(xm1, ym1);
                                        }
                                        else
                                        {
                                            if (vergleich > höhe)
                                            {
                                                *(zeiger - 1024 - 1) = höhe;
                                                *(pA + anzahlNodes++) = new System.Drawing.Point(xm1, ym1);
                                            }
                                        }
                                    }
                                }
                            }
                            // SÜD 
                            if (yp1 < GrenzeMax)
                            {
                                if (!*(zeigerGr + 1))
                                {
                                    vergleich = *(zeiger + 1);
                                    if (vergleich == 0)
                                    {
                                        *(zeiger + 1) = höhe;
                                        *(pA + anzahlNodes++) = new System.Drawing.Point(x, yp1);
                                    }
                                    else
                                    {
                                        if (vergleich > höhe)
                                        {
                                            *(zeiger + 1) = höhe;
                                            *(pA + anzahlNodes++) = new System.Drawing.Point(x, yp1);
                                        }
                                    }

                                }
                                //OST
                                if (xp1 < GrenzeMax)
                                {
                                    if (!*(zeigerGr + 1024 + 1))
                                    {
                                        vergleich = *(zeiger + 1024 + 1);
                                        if (vergleich == 0)
                                        {
                                            *(zeiger + 1024 + 1) = höhe;
                                            *(pA + anzahlNodes++) = new System.Drawing.Point(xp1, yp1);
                                        }
                                        else
                                        {
                                            if (vergleich > höhe)
                                            {
                                                *(zeiger + 1024 + 1) = höhe;
                                                *(pA + anzahlNodes++) = new System.Drawing.Point(xp1, yp1);
                                            }
                                        }

                                    }
                                }
                                //WEST
                                if (xm1 > GrenzeMin)
                                {
                                    if (!*(zeigerGr - 1024 + 1))
                                    {
                                        vergleich = *(zeiger - 1024 + 1);
                                        if (vergleich == 0)
                                        {
                                            *(zeiger - 1024 + 1) = höhe;
                                            *(pA + anzahlNodes++) = new System.Drawing.Point(xm1, yp1);
                                        }
                                        else
                                        {
                                            if (vergleich > höhe)
                                            {
                                                *(zeiger - 1024 + 1) = höhe;
                                                *(pA + anzahlNodes++) = new System.Drawing.Point(xm1, yp1);
                                            }
                                        }
                                    }
                                }
                            }

                            //OST
                            if (xp1 < GrenzeMax)
                            {
                                if (!*(zeigerGr + 1024))
                                {
                                    vergleich = *(zeiger + 1024);
                                    if (vergleich == 0)
                                    {
                                        *(zeiger + 1024) = höhe;
                                        *(pA + anzahlNodes++) = new System.Drawing.Point(xp1, y);
                                    }
                                    else
                                    {
                                        if (vergleich > höhe)
                                        {
                                            *(zeiger + 1024) = höhe;
                                            *(pA + anzahlNodes++) = new System.Drawing.Point(xp1, y);
                                        }
                                    }

                                }
                            }
                            //WEST
                            if (xm1 > GrenzeMin)
                            {
                                if (!*(zeigerGr - 1024))
                                {
                                    vergleich = *(zeiger - 1024);
                                    if (vergleich == 0)
                                    {
                                        *(zeiger - 1024) = höhe;
                                        *(pA + anzahlNodes++) = new System.Drawing.Point(xm1, y);
                                    }
                                    else
                                    {
                                        if (vergleich > höhe)
                                        {
                                            *(zeiger - 1024) = höhe;
                                            *(pA + anzahlNodes++) = new System.Drawing.Point(xm1, y);
                                        }
                                    }
                                }
                            }
                            if (i == anzahlNodes - 1) break;
                        }
                    }
                }
            }
        }

        private void Nächsterschritt()
        {
            aktWertig = int.MaxValue;
            var x = aktX;
            var y = aktY;
            var xm1 = x - 1;
            var xp1 = x + 1;
            var ym1 = y - 1;
            var yp1 = y + 1;
            var tempX = 0;
            var tempY = 0;
            var wertig = aktWertig;

            fixed (int* iA = &intArray[0, 0])
            {
                int wertigNeu;
                if (ym1 > GrenzeMin)
                {
                    if (xm1 > GrenzeMin)
                    {
                        wertigNeu = *(iA + (xm1 << 10) + ym1);
                        if (wertigNeu < wertig & wertigNeu > 0)
                            if (xm1 != altX | ym1 != altY)
                            {
                                wertig = wertigNeu;
                                tempX = xm1;
                                tempY = ym1;
                            }
                    }
                    if (xp1 < GrenzeMax)
                    {
                        wertigNeu = *(iA + (xp1 << 10) + ym1);
                        if (wertigNeu < wertig & wertigNeu > 0)
                            if (xp1 != altX | ym1 != altY)
                            {
                                wertig = wertigNeu;
                                tempX = xp1;
                                tempY = ym1;
                            }
                    }
                    wertigNeu = *(iA + (x << 10) + ym1);
                    if (wertigNeu < wertig & wertigNeu > 0)
                        if (x != altX | ym1 != altY)
                        {
                            wertig = wertigNeu;
                            tempX = x;
                            tempY = ym1;
                        }
                }
                if (yp1 < GrenzeMax)
                {
                    if (xm1 > GrenzeMin)
                    {
                        wertigNeu = *(iA + (xm1 << 10) + yp1);
                        if (wertigNeu < wertig & wertigNeu > 0)
                            if (xm1 != altX | yp1 != altY)
                            {
                                wertig = wertigNeu;
                                tempX = xm1;
                                tempY = yp1;
                            }
                    }
                    if (xp1 < GrenzeMax)
                    {
                        wertigNeu = *(iA + (xp1 << 10) + yp1);
                        if (wertigNeu < wertig & wertigNeu > 0)
                            if (xp1 != altX | yp1 != altY)
                            {
                                wertig = wertigNeu;
                                tempX = xp1;
                                tempY = yp1;
                            }
                    }
                    wertigNeu = *(iA + (x << 10) + yp1);
                    if (wertigNeu < wertig & wertigNeu > 0)
                        if (x != altX | yp1 != altY)
                        {
                            wertig = wertigNeu;
                            tempX = x;
                            tempY = yp1;
                        }
                }
                if (xm1 > GrenzeMin)
                {
                    wertigNeu = *(iA + (xm1 << 10) + y);
                    if (wertigNeu < wertig & wertigNeu > 0)
                        if (xm1 != altX | y != altY)
                        {
                            wertig = wertigNeu;
                            tempX = xm1;
                            tempY = y;
                        }
                }

                if (xp1 < GrenzeMax)
                {
                    wertigNeu = *(iA + (xp1 << 10) + y);
                    if (wertigNeu < wertig & wertigNeu > 0)
                        if (xp1 != altX | y != altY)
                        {
                            wertig = wertigNeu;
                            tempX = xp1;
                            tempY = y;
                        }
                }
            }
            altX = aktX;
            altY = aktY;
            aktWertig = wertig;
            aktX = tempX;
            aktY = tempY;
        }

        public void NextStep()
        {
            this.Nächsterschritt();
            currentposition.x = aktX;
            currentposition.y = aktY;
            MakeMove(currentposition);
        }

        public event Action<Point> MakeMove;
    }
}

