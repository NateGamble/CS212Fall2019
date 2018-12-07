using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.Diagnostics;

namespace Mankalah
{
    /*****************************************************************/
    /*
    /* A Dumb Mankalah player.  This player always takes the first
    /* first available go-again, if there is one. If not, it takes 
    /* the first available move.
    /*
    /*****************************************************************/
    public class stz4Player : Player
    {
        Position us;
        int max_time;
        public stz4Player(Position pos, int timeLimit) : base(pos, "Sebrina", timeLimit)
        {

            max_time = timeLimit;
            Position us = pos;

        }

        public override string gloat()
        {
            return "Sebrina wins!";
        }
        public override int chooseMove(Board b)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int depth = 5;
            //int[] move = { 0, 0 };

            //if(us == Position.Top)
            //{
            //    move[0] = 7;
            //}
            int[] result = minmaxVal(b, depth, float.MinValue, float.MaxValue, ref timer);
            // int[] result1 = result;

            //while (timer.ElapsedMilliseconds < (max_time - 200))
            // {
            // result = minmaxVal(b, depth, ref timer);
            //Console.WriteLine(result[1]);
            //if (us == Position.Top)
            //{
            // Console.WriteLine(result1[1]);
            // Console.WriteLine("hi");
            //if (result[1] > result1[1])
            //{
            //    result1[0] = result[0];
            //    result1[1] = result[1];
            //}
            //Console.WriteLine($"Depth {depth}: best move is {result1[0]}");
            //}
            //if (us == Position.Bottom)
            //{
            //    //   Console.WriteLine("hi");

            //    if (result[1] < result1[1])
            //    {
            //        result1[0] = result[0];
            //        result1[1] = result[1];
            //    }
            //}
            //  depth++;
            //}
            //Console.WriteLine(result1[0]);
            return result[0];
        }


        private int[] minmaxVal(Board b, int d, float alpha, float beta, ref Stopwatch timer)
        {
            if (d == 0 || b.gameOver())
            {
                return new int[] { 0, evaluate(b) };
            }
            int[] bestMove = { 0, int.MinValue };
            if (b.whoseMove() == us)
            {
                int bestVal = int.MinValue;
                for (int move = 0; move <= 13; move++)
                {
                    if (b.legalMove(move) && timer.ElapsedMilliseconds < (max_time - 200))
                    {
                        Board b1 = new Board(b);
                        b1.makeMove(move, false);
                        int[] val = minmaxVal(b1, d - 1, alpha, beta, ref timer);
                        if (val[1] > bestVal)
                        {
                            bestVal = val[1];
                            bestMove[0] = move;
                            bestMove[1] = bestVal;
                        }

                        alpha = Math.Max(alpha, bestMove[1]);
                        if (beta <= alpha) { break; }
                    }
                }
            }
            else
            {

                int bestVal = int.MaxValue;
                bestMove = new int[] { 0, bestVal };
                for (int move = 0; move <= 13; move++)
                {
                    if (b.legalMove(move) && timer.ElapsedMilliseconds < (max_time - 200))
                    {
                        Board b1 = new Board(b);
                        b1.makeMove(move, false);
                        int[] val = minmaxVal(b1, d - 1, alpha, beta, ref timer);
                        if (val[1] < bestVal)
                        {
                            bestVal = val[1];
                            bestMove[0] = move;
                            bestMove[1] = bestVal;
                        }

                        beta = Math.Min(beta, bestMove[1]);
                        if (beta <= alpha) { break; }
                    }
                }
            }
            return bestMove;
        }

        public override int evaluate(Board b)
        {
            //    //return b.stonesAt(13) - b.stonesAt(6);
            //    int goAgain = 0;
            //    int catches = 0;
            //    if (us == Position.Top)
            //    {
            //        for (int pos = 7; pos<=12; pos++)
            //        {
            //            int numStone = b.stonesAt(pos);
            //            if(pos+numStone == 13)
            //            {
            //                goAgain++;

            //            }

            //            //int nextPos = pos + numStone;
            //            //if((nextPos % 12) - 1 <=5 && (nextPos % 12) - 1 >= 5 )
            //            //{
            //            //    if (b.stonesAt(nextPos) == 0)
            //            //    {
            //            //        catches++;
            //            //    }
            //            //}

            //        }
            //        Console.WriteLine(b.stonesAt(6) - b.stonesAt(13) + 3 * (goAgain) + 5 * (catches));

            //       // return b.stonesAt(6) - b.stonesAt(13) + 3 * (goAgain) + 5 * (catches);
            //    }
            //    else if (us == Position.Bottom)
            //    {
            //        for (int pos = 0; pos <= 5; pos++)
            //        {
            //            int numStone = b.stonesAt(pos);
            //            if (pos + numStone == 6)
            //            {
            //                goAgain++;
            //            }

            //            //int nextPos = pos + numStone;
            //            //if ((nextPos % 12) - 1 <= 12 && (nextPos % 12) - 1 >= 7 && b.stonesAt(nextPos) == 0)
            //            //{
            //            //    catches++;
            //            //}
            //        }
            //        return   -3 * (goAgain) + -5 * (catches);
            //    }
            //    return 1;
            int catch1 = NumCatch(b, us);
            int move = nextMove(b, us);
            int counter = CounterInAllPit(b, us);
            int Count_pit_opp = CounterInAllPit_opponent(b, us);
            int store = Store(b, us);
            int opp_store = store_oponent(b, us);
            return (int)(catch1 + move + counter + store - opp_store - Count_pit_opp);

            //int sub = Sub(b, us);
            //int Count_pit = CounterInAllPit(b, us);

            //int Close = NoStoneClose(b, us);
            //int away = Away(b, us);
            //int middle = Middle(b, us);


            //return (int)( 0.5 * catch1 + 0.4 * move + 0.84375 * sub + 0.5625 * Count_pit - 0.5625 * Count_pit_opp + 0.4 * Close + 0.46875 * away + 0.5 * middle);
            //if (us == Position.Top)
            //{
            //    return (int)(0.5 * catch1 + 0.4 * move + 0.555556 * sub + 0.66667 * Count_pit - 0.666667 * Count_pit_opp + 0.4444 * Close + 0.555556 * away + 0.333333 * middle);
            //}
            //else{
            //    return (int)(0.5 * catch1 + 0.4 * move + 0.84375 * sub + 0.5625 * Count_pit - 0.5625 * Count_pit_opp + 0.5 * Close + 0.46875 * away + 0.5 * middle);
            //}
        }




        public int Sub(Board b, Position p)
        {
            if (p == Position.Top)
            {
                return b.stonesAt(13) + b.stonesAt(6);
            }
            else
            {
                return b.stonesAt(6) - b.stonesAt(13);
            }
        }
        public int NumCatch(Board b, Position p)
        {
            int bestCatch = 0;
            if (p == Position.Top)
            {
                for (int i = 7; i <= 12; i++)
                {
                    int stone = b.stonesAt(i);
                    if (((i + stone) % 13) <= 12 && ((i + stone) % 13 >= 7))
                    {
                        if (b.stonesAt((i + stone) % 13) == 0)
                        {
                            bestCatch = bestCatch + b.stonesAt(13 - i - 1);
                        }
                    }
                }
                return bestCatch;
            }
            else
            {
                for (int i = 0; i <= 5; i++)
                {
                    int stone = b.stonesAt(i);
                    if (((i + stone) % 13) <= 5 && ((i + stone) % 13 >= 0))
                    {
                        if (b.stonesAt((i + stone) % 13) == 0)
                        {
                            bestCatch = bestCatch + b.stonesAt(13 - i - 1);
                        }
                    }
                }
                return bestCatch;
            }
        }

        public int nextMove(Board b, Position p)
        {
            int nextMove = 0;
            if (p == Position.Top)
            {
                for (int i = 7; i <= 12; i++)
                {
                    int stone = b.stonesAt(i);
                    //if (((i + stone) % 13) <= 5 && ((i + stone) >= 0))
                    //{
                    if ((i + stone) == 13)
                    {
                        nextMove = nextMove + 8;
                    }
                    //}


                }
                return nextMove;
            }
            else
            {
                for (int i = 0; i <= 5; i++)
                {
                    int stone = b.stonesAt(i);
                    if ((i + stone) == 6)
                    {
                        nextMove = nextMove + 8;
                    }
                }
                return nextMove;
            }
        }

        public int CounterInAllPit(Board b, Position p)
        {
            int counters = 0;
            if (p == Position.Top)
            {
                for (int i = 7; i <= 12; i++)
                {
                    counters = counters + b.stonesAt(i) + 4;
                }
            }
            else
            {
                for (int i = 0; i <= 5; i++)
                {
                    counters = counters + b.stonesAt(i) + 4;
                }
            }
            return counters;
        }

        public int CounterInAllPit_opponent(Board b, Position p)
        {
            int counters = 0;
            if (p == Position.Top)
            {
                for (int i = 0; i <= 5; i++)
                {
                    counters = counters + b.stonesAt(i) + 4;
                }
            }
            else
            {
                for (int i = 7; i <= 12; i++)
                {
                    counters = counters + b.stonesAt(i) + 4;
                }
            }
            return counters;
        }

        public int NoStoneClose(Board b, Position p)
        {
            int counters = 0;
            if (p == Position.Top)
            {

                counters = b.stonesAt(12) + b.stonesAt(11) + 4;

            }
            else
            {

                counters = b.stonesAt(5) + b.stonesAt(4) + 4;

            }
            return counters;
        }

        public int Away(Board b, Position p)
        {
            int counters = 0;
            if (p == Position.Top)
            {

                counters = b.stonesAt(7) + b.stonesAt(8) + 4;

            }
            else
            {

                counters = b.stonesAt(0) + b.stonesAt(1) + 4;

            }
            return counters;
        }

        public int Middle(Board b, Position p)
        {
            int counters = 0;
            if (p == Position.Top)
            {
                counters = b.stonesAt(9) + b.stonesAt(10);

            }
            else
            {

                counters = b.stonesAt(2) + b.stonesAt(3);

            }
            return counters;
        }


        public int Store(Board b, Position p)
        {
            if (p == Position.Top)
            {
                return b.stonesAt(13) + 6;
            }
            else
            {
                return b.stonesAt(6) + 6;
            }
        }

        public int store_oponent(Board b, Position p)
        {
            if (p == Position.Top)
            {
                return b.stonesAt(6) + 6;
            }
            else
            {
                return b.stonesAt(13) + 6;
            }
        }
    }
}