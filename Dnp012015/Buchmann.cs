namespace Dnp012015.Buchmann
{
    using System;
    using System.Runtime.InteropServices;
    using contest.submission.contract;

    public unsafe class Solution : IDnp1501Solution
    {
        public Solution()
        {
            // Queue initialisieren. Bringt nicht viel, aber CLR muss Speicher nicht mit 0 initialisieren 
            this.Queue = (int*)Marshal.AllocHGlobal(1048576 << 2);
        }

        ~Solution()
        {
            // Speicher der Queue wieder frei geben
            Marshal.FreeHGlobal((IntPtr)this.Queue);
            GC.Collect();
        }

        private BoolArray spielfeld;
        public int* Queue;
        // ClosedList muss initialisiert werden
        public readonly int[] ClosedList = new int[1048576];

        public void Start(BoolArray spielfeldPara, Point startPunkt, Point endPunkt)
        {
            this.spielfeld = spielfeldPara;
            this.queueStart = this.queueEnde = this.Queue;
            // Startpunkt und Endpunkt zur Queue hinzufügen
            // Punkte werden als int dargestellt. Der x-Wert wird um 10 Bits nach links verschoben.
            *this.queueEnde = startPunkt.x << 10 | startPunkt.y;
            this.queueEnde++;
            this.ClosedList[startPunkt.x << 10 | startPunkt.y] = -1048576; // Markierung mit negativen Werten
            *this.queueEnde = endPunkt.x << 10 | endPunkt.y;
            this.queueEnde++;
            this.ClosedList[endPunkt.x << 10 | endPunkt.y] = 1; // Markierung mit positiven Werten

            this.Suche();
            this.BerechnePfad();
            this.NextStep();
        }

        private void Suche()
        {
            fixed (bool* data = &this.spielfeld.Data[0, 0])
            {
                // modifizierter A* Algorithmus
                do
                {
                    // Für jeden Punkt des Kreis um den aktuellen Punkt herum testen. Entfernung eintragen und zur Queue hinzufügen
                    var current = *this.queueStart++;
                    int entfernung = this.ClosedList[current] + 1;

                    // Expand node
                    int y = current & 1023;
                    int x1Y = current - 1024;
                    int x2Y = current + 1024;
                    bool y1Ok = y > 0;
                    bool y2Ok = y < 1023;

                    if (x1Y >= 0)
                    {
                        if (y1Ok)
                        {
                            int x1Y1 = x1Y - 1;
                            if (!data[x1Y1])
                            {
                                int cx1Y1 = this.ClosedList[x1Y1];
                                if (cx1Y1 == 0)
                                {
                                    *this.queueEnde = x1Y1;
                                    this.queueEnde++;
                                    this.ClosedList[x1Y1] = entfernung;
                                }
                                else
                                {
                                    // Test auf Ende, ClosedList Einträge müssen unterschiedliches Vorzeichen
                                    // haben. Bei mir ist dies schneller als (cx1Y1 ^ entfernung) < 0
                                    if (((cx1Y1 ^ entfernung) & 0x40000000) == 0x40000000)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        if (!data[x1Y])
                        {
                            int cx1Y = this.ClosedList[x1Y];
                            if (cx1Y == 0)
                            {
                                *this.queueEnde = x1Y;
                                this.queueEnde++;
                                this.ClosedList[x1Y] = entfernung;
                            }
                            else
                            {
                                if (((cx1Y ^ entfernung) & 0x40000000) == 0x40000000)
                                {
                                    return;
                                }
                            }
                        }

                        if (y2Ok)
                        {
                            int x1Y2 = x1Y + 1;
                            if (!data[x1Y2])
                            {
                                int cx1Y2 = this.ClosedList[x1Y2];
                                if (cx1Y2 == 0)
                                {
                                    *this.queueEnde = x1Y2;
                                    this.queueEnde++;
                                    this.ClosedList[x1Y2] = entfernung;
                                }
                                else
                                {
                                    if (((cx1Y2 ^ entfernung) & 0x40000000) == 0x40000000)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    if (y1Ok)
                    {
                        int xY1 = current - 1;
                        if (!data[xY1])
                        {
                            int cxY1 = this.ClosedList[xY1];
                            if (cxY1 == 0)
                            {
                                *this.queueEnde = xY1;
                                this.queueEnde++;
                                this.ClosedList[xY1] = entfernung;
                            }
                            else
                            {
                                if (((cxY1 ^ entfernung) & 0x40000000) == 0x40000000)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    if (y2Ok)
                    {
                        int xY2 = current + 1;
                        if (!data[xY2])
                        {
                            int cxY2 = this.ClosedList[xY2];
                            if (cxY2 == 0)
                            {
                                *this.queueEnde = xY2;
                                this.queueEnde++;
                                this.ClosedList[xY2] = entfernung;
                            }
                            else
                            {
                                if (((cxY2 ^ entfernung) & 0x40000000) == 0x40000000)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    if (x2Y >= 1048576)
                    {
                        continue;
                    }
                    if (y1Ok)
                    {
                        int x2Y1 = x2Y - 1;
                        if (!data[x2Y1])
                        {
                            int cx2Y1 = this.ClosedList[x2Y1];
                            if (cx2Y1 == 0)
                            {
                                *this.queueEnde = x2Y1;
                                this.queueEnde++;
                                this.ClosedList[x2Y1] = entfernung;
                            }
                            else
                            {
                                if (((cx2Y1 ^ entfernung) & 0x40000000) == 0x40000000)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    if (!data[x2Y])
                    {
                        int cx2Y = this.ClosedList[x2Y];
                        if (cx2Y == 0)
                        {
                            *this.queueEnde = x2Y;
                            this.queueEnde++;
                            this.ClosedList[x2Y] = entfernung;
                        }
                        else
                        {
                            if (((cx2Y ^ entfernung) & 0x40000000) == 0x40000000)
                            {
                                return;
                            }
                        }
                    }

                    if (y2Ok)
                    {
                        int x2Y2 = x2Y + 1;
                        if (!data[x2Y2])
                        {
                            int cx2Y2 = this.ClosedList[x2Y2];
                            if (cx2Y2 == 0)
                            {
                                *this.queueEnde = x2Y2;
                                this.queueEnde++;
                                this.ClosedList[x2Y2] = entfernung;
                            }
                            else
                            {
                                if (((cx2Y2 ^ entfernung) & 0x40000000) == 0x40000000)
                                {
                                    return;
                                }
                            }
                        }
                    }
                } while (this.queueStart <= this.queueEnde);
            }
        }

        private int* queueEnde;
        private int* queueStart;
        private readonly Point point = new Point();
        private int posMemory;
        private int entfernung;
        private int next;

        public void BerechnePfad()
        {
            // Da der Algorithmus von beiden Seiten vorgeht muss die Strecke vom Anfang bis zum Treffpunkt in der Mitte
            // zuerst umgedreht werden. Speicherung wieder in der selben Queue.
            int position;
            this.queueStart--;
            this.next = *this.queueStart;
            this.posMemory = this.next;
            this.queueStart = this.Queue;
            this.queueEnde = this.Queue - 1;

            this.entfernung = this.ClosedList[this.next];
            if (this.entfernung > 0)
            {
                position = this.next;
                this.queueEnde++;
                *this.queueEnde = this.next;

                int y = position & 1023;
                int x1Y = position - 1024;
                int x2Y = position + 1024;
                bool x1Ok = x1Y >= 0;
                bool x2Ok = x2Y < 1048576;
                bool y1Ok = y > 0;
                bool y2Ok = y < 1023;

                if (y1Ok)
                {
                    int xY1 = position - 1;
                    if (this.ClosedList[xY1] < this.entfernung)
                    {
                        this.entfernung = this.ClosedList[xY1];
                        this.next = xY1;
                    }
                }

                if (y2Ok)
                {
                    int xY2 = position + 1;
                    if (this.ClosedList[xY2] < this.entfernung)
                    {
                        this.entfernung = this.ClosedList[xY2];
                        this.next = xY2;
                    }
                }

                if (x1Ok && this.ClosedList[x1Y] < this.entfernung)
                {
                    this.entfernung = this.ClosedList[x1Y];
                    this.next = x1Y;
                }

                if (x2Ok && this.ClosedList[x2Y] < this.entfernung)
                {
                    this.entfernung = this.ClosedList[x2Y];
                    this.next = x2Y;
                }

                if (x1Ok)
                {
                    if (y1Ok)
                    {
                        int x1Y1 = x1Y - 1;
                        if (this.ClosedList[x1Y1] < this.entfernung)
                        {
                            this.entfernung = this.ClosedList[x1Y1];
                            this.next = x1Y1;
                        }
                    }

                    if (y2Ok)
                    {
                        int x1Y2 = x1Y + 1;
                        if (this.ClosedList[x1Y2] < this.entfernung)
                        {
                            this.entfernung = this.ClosedList[x1Y2];
                            this.next = x1Y2;
                        }
                    }
                }

                if (x2Ok)
                {
                    if (y1Ok)
                    {
                        int x2Y1 = x2Y - 1;
                        if (this.ClosedList[x2Y1] < this.entfernung)
                        {
                            this.entfernung = this.ClosedList[x2Y1];
                            this.next = x2Y1;
                        }
                    }

                    if (y2Ok)
                    {
                        int x2Y2 = x2Y + 1;
                        if (this.ClosedList[x2Y2] < this.entfernung)
                        {
                            this.entfernung = this.ClosedList[x2Y2];
                            this.next = x2Y2;
                        }
                    }
                }
            }

            while (this.entfernung != -1048576)
            {
                position = this.next;
                this.queueEnde++;
                *this.queueEnde = this.next;

                int y = position & 1023;
                int x1Y = position - 1024;
                int x2Y = position + 1024;
                bool x1Ok = x1Y >= 0;
                bool x2Ok = x2Y < 1048576;
                bool y1Ok = y > 0;
                bool y2Ok = y < 1023;
                this.entfernung--;

                if (y1Ok)
                {
                    int xY1 = position - 1;
                    if (this.ClosedList[xY1] == this.entfernung)
                    {
                        this.next = xY1;
                        continue;
                    }
                }

                if (y2Ok)
                {
                    int xY2 = position + 1;
                    if (this.ClosedList[xY2] == this.entfernung)
                    {
                        this.next = xY2;
                        continue;
                    }
                }

                if (x1Ok && this.ClosedList[x1Y] == this.entfernung)
                {
                    this.next = x1Y;
                    continue;
                }

                if (x2Ok && this.ClosedList[x2Y] == this.entfernung)
                {
                    this.next = x2Y;
                    continue;
                }

                if (x1Ok)
                {
                    if (y1Ok)
                    {
                        int x1Y1 = x1Y - 1;
                        if (this.ClosedList[x1Y1] == this.entfernung)
                        {
                            this.next = x1Y1;
                            continue;
                        }
                    }

                    if (y2Ok)
                    {
                        int x1Y2 = x1Y + 1;
                        if (this.ClosedList[x1Y2] == this.entfernung)
                        {
                            this.next = x1Y2;
                            continue;
                        }
                    }
                }

                if (x2Ok)
                {
                    if (y1Ok)
                    {
                        int x2Y1 = x2Y - 1;
                        if (this.ClosedList[x2Y1] == this.entfernung)
                        {
                            this.next = x2Y1;
                            continue;
                        }
                    }

                    if (y2Ok)
                    {
                        int x2Y2 = x2Y + 1;
                        if (this.ClosedList[x2Y2] == this.entfernung)
                        {
                            this.next = x2Y2;
                        }
                    }
                }
            }

            if (this.next != *this.queueEnde)
            {
                this.queueEnde++;
                *this.queueEnde = this.next;
            }

            this.entfernung = this.ClosedList[this.posMemory];
        }

        public void NextStep()
        {
            // Nun zuerst den umgedrehten ersten Teil vom Startpunkt bis zum Treffpunkt ausgeben
            if (this.queueEnde > this.Queue)
            {
                this.queueEnde--;
                var xy = *this.queueEnde;
                this.point.x = xy >> 10;
                this.point.y = xy & 1023;
                this.MakeMove(this.point);
                return;
            }

            // dann den zweiten Teil vom Treffpunktpunkt bis zum Zielpunkt ausgeben
            int x1Y = this.posMemory - 1024;
            int x2Y = this.posMemory + 1024;
            int y = this.posMemory & 1023;
            bool x1Ok = x1Y >= 0;
            bool x2Ok = x2Y < 1048576;
            bool y1Ok = y > 0;
            bool y2Ok = y < 1023;

            if (this.entfernung < 0)
            {
                this.entfernung = 1048576;

                if (y1Ok)
                {
                    int xY1 = this.posMemory - 1;
                    if (this.ClosedList[xY1] < this.entfernung && this.ClosedList[xY1] > 0)
                    {
                        this.entfernung = this.ClosedList[xY1];
                        this.next = xY1;
                    }
                }

                if (y2Ok)
                {
                    int xY2 = this.posMemory + 1;
                    if (this.ClosedList[xY2] < this.entfernung && this.ClosedList[xY2] > 0)
                    {
                        this.entfernung = this.ClosedList[xY2];
                        this.next = xY2;
                    }
                }

                if (x1Ok && this.ClosedList[x1Y] < this.entfernung && this.ClosedList[x1Y] > 0)
                {
                    this.entfernung = this.ClosedList[x1Y];
                    this.next = x1Y;
                }

                if (x2Ok && this.ClosedList[x2Y] < this.entfernung && this.ClosedList[x2Y] > 0)
                {
                    this.entfernung = this.ClosedList[x2Y];
                    this.next = x2Y;
                }

                if (x1Ok)
                {
                    if (y1Ok)
                    {
                        int x1Y1 = x1Y - 1;
                        if (this.ClosedList[x1Y1] < this.entfernung && this.ClosedList[x1Y1] > 0)
                        {
                            this.entfernung = this.ClosedList[x1Y1];
                            this.next = x1Y1;
                        }
                    }

                    if (y2Ok)
                    {
                        int x1Y2 = x1Y + 1;
                        if (this.ClosedList[x1Y2] < this.entfernung && this.ClosedList[x1Y2] > 0)
                        {
                            this.entfernung = this.ClosedList[x1Y2];
                            this.next = x1Y2;
                        }
                    }
                }

                if (x2Ok)
                {
                    if (y1Ok)
                    {
                        int x2Y1 = x2Y - 1;
                        if (this.ClosedList[x2Y1] < this.entfernung && this.ClosedList[x2Y1] > 0)
                        {
                            this.entfernung = this.ClosedList[x2Y1];
                            this.next = x2Y1;
                        }
                    }

                    if (y2Ok)
                    {
                        int x2Y2 = x2Y + 1;
                        if (this.ClosedList[x2Y2] < this.entfernung && this.ClosedList[x2Y2] > 0)
                        {
                            this.entfernung = this.ClosedList[x2Y2];
                            this.next = x2Y2;
                        }
                    }
                }

                this.posMemory = this.next;
                this.point.x = this.next >> 10;
                this.point.y = this.next & 1023;
                this.MakeMove(this.point);
                return;
            }

            this.entfernung--;

            if (y1Ok)
            {
                int xY1 = this.posMemory - 1;
                if (this.ClosedList[xY1] == this.entfernung)
                {
                    this.posMemory = xY1;
                    this.point.x = this.posMemory >> 10;
                    this.point.y = this.posMemory & 1023;
                    this.MakeMove(this.point);
                    return;
                }
            }

            if (y2Ok)
            {
                int xY2 = this.posMemory + 1;
                if (this.ClosedList[xY2] == this.entfernung)
                {
                    this.posMemory = xY2;
                    this.point.x = this.posMemory >> 10;
                    this.point.y = this.posMemory & 1023;
                    this.MakeMove(this.point);
                    return;
                }
            }

            if (x1Ok && this.ClosedList[x1Y] == this.entfernung)
            {
                this.posMemory = x1Y;
                this.point.x = this.posMemory >> 10;
                this.point.y = this.posMemory & 1023;
                this.MakeMove(this.point);
                return;
            }

            if (x2Ok && this.ClosedList[x2Y] == this.entfernung)
            {
                this.posMemory = x2Y;
                this.point.x = this.posMemory >> 10;
                this.point.y = this.posMemory & 1023;
                this.MakeMove(this.point);
                return;
            }

            if (x1Ok)
            {
                if (y1Ok)
                {
                    int x1Y1 = x1Y - 1;
                    if (this.ClosedList[x1Y1] == this.entfernung)
                    {
                        this.posMemory = x1Y1;
                        this.point.x = this.posMemory >> 10;
                        this.point.y = this.posMemory & 1023;
                        this.MakeMove(this.point);
                        return;
                    }
                }

                if (y2Ok)
                {
                    int x1Y2 = x1Y + 1;
                    if (this.ClosedList[x1Y2] == this.entfernung)
                    {
                        this.posMemory = x1Y2;
                        this.point.x = this.posMemory >> 10;
                        this.point.y = this.posMemory & 1023;
                        this.MakeMove(this.point);
                        return;
                    }
                }
            }

            if (x2Ok)
            {
                if (y1Ok)
                {
                    int x2Y1 = x2Y - 1;
                    if (this.ClosedList[x2Y1] == this.entfernung)
                    {
                        this.posMemory = x2Y1;
                        this.point.x = this.posMemory >> 10;
                        this.point.y = this.posMemory & 1023;
                        this.MakeMove(this.point);
                        return;
                    }
                }

                if (y2Ok)
                {
                    int x2Y2 = x2Y + 1;
                    if (this.ClosedList[x2Y2] == this.entfernung)
                    {
                        this.posMemory = x2Y2;
                        this.point.x = this.posMemory >> 10;
                        this.point.y = this.posMemory & 1023;
                        this.MakeMove(this.point);
                        return;
                    }
                }
            }
        }

        public event Action<Point> MakeMove;
    }
}
