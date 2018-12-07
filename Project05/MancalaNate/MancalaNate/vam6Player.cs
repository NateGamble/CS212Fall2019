/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *                                                       * * * * *
 *                                                       * * * * *
 *  Valeria Martinez                                     * * * * *
 *  CS 212- A                                            * * * * *
 *  12/07/2018                                           * * * * *
 *                                                       * * * * *
 *  Player subclass. This class implements               * * * * *
 *  a minimax search to find its next move               * * * * *
 *  within the alotted time.                             * * * * *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 */


using System;
using Mankalah;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.Generic;


namespace Mankalah
{
    public class vam6Player : Player

    {
        Position us;


        //Diagnostics library creates a stopwatch to keep track of time
        Stopwatch stop_watch = new Stopwatch();

        //call the player constructor with name "Valeria Martinez"
        public vam6Player(Position pos, int timeLimit) : base(pos, "Valeria Martinez", timeLimit)
        {
            us = pos;
        }

        //returns string in case of victory 
        public override string gloat()
        {
            return "You just got D-D-D D DENIS-ED";
        }

        //provides photo of myself for the tournament
        public override String getImage()
        {
            return "pic.jpg";
        }

        /* Evaluate: override --> consider at least 3 more factors
         * The University of Kansas researched that there are 7 
         * factors that play an important role for winning in Mancala
         * for the sake of time, I will focus on 3 of them. 
         * total stones, go agains, and captures total
         */


        public override int evaluate(Board b) // ---> change this 
        {
            int score = b.stonesAt(13) - b.stonesAt(6); //followed from example of player class
                                                        //  score = (b.whoseMove() == Position.Top) ? score : -1 * score;
            int totalStones = 0;
            int goAgainTotal = 0;
            int capturesTotal = 0;


            if (b.whoseMove() == Position.Top)
            {
                for (int i = 7; i <= 12; i++) //position == TOP 
                {
                    totalStones += b.stonesAt(i); //add all of the stones in the top row
                    if (b.stonesAt(i) - (13 - i) == 0) //add the times you can go again 
                        goAgainTotal += 1;
                    int target = i + b.stonesAt(i);
                    if (target < 13)
                    {
                        int tStones = b.stonesAt(target);
                        if (b.whoseMove() == Position.Top)
                        {
                            if (tStones == 0 && b.stonesAt(13 - target - 1) != 0)
                                capturesTotal += b.stonesAt(13 - target - 1);

                        }
                    }
                }

            }
            else
            {
                for (int i = 0; i <= 5; i++)
                { //position == BOTTOM
                    totalStones -= b.stonesAt(i); //subtract all the stones from the bottom row
                    if (b.stonesAt(i) - (6 - i) == 0)
                    { //for the bottom row
                        goAgainTotal -= 1; //subtract go agains
                    }
                    int target = i + b.stonesAt(i);
                    if (target < 6)
                    {
                        int tStones = b.stonesAt(target);
                        if (b.whoseMove() == Position.Bottom)
                        {
                            if (tStones == 0 && b.stonesAt(13 - target - 1) != 0)
                                capturesTotal -= b.stonesAt(13 - target - 1);
                        }

                    }

                }

            }

            capturesTotal = (b.whoseMove() == Position.Top) ? capturesTotal : -1 * capturesTotal;
            totalStones = (b.whoseMove() == Position.Top) ? totalStones : -1 * totalStones;
            goAgainTotal = (b.whoseMove() == Position.Top) ? goAgainTotal : -1 * goAgainTotal;


            score += totalStones + capturesTotal + goAgainTotal;
            return score;
        }



        /* chooseMove()
         * @param: b, a Board type
         * chooseMove() is overriden,
         * calls minimax function
         */


        public override int chooseMove(Board b) // ---> change this 
        {
            Stopwatch stop_watch = new Stopwatch();
            stop_watch.Start();
            int i = 1;
            Result move = new Result(0, 0);
            while (stop_watch.ElapsedMilliseconds < getTimePerMove())
            {
                move = minimaxVal(b, i++, Int32.MinValue, Int32.MaxValue);
            }
            return move.GetMove();

            //Result topVal = new Result(0, 10000000);
            //Result val = new Result(0, -10000000);
            //// start a stopwatch
            //stop_watch.Start();

            //if (stop_watch.ElapsedMilliseconds < getTimePerMove())
            //{ //while the time is inside the legal framework 

            //    //DFS of depth 5
            //    for (int i = 1; i <= 5; i++)
            //    {
            //        topVal = minimaxVal(b, i); //call minimax on the topVal 
            //        if (topVal.GetScore() > val.GetScore())
            //        {  //while topVal is greater than val 
            //            val = topVal;
            //        }
            //    }
            //    stop_watch.Stop(); //stop time
            //    stop_watch.Reset(); //reset timer
            //    //return best move --> return val.GetMove();

            //}

            //return val.GetMove(); // I just added this para que se callara --> used to be return 0;

        }

        /* minimax()
         * @param: b, a Board type && d, and int type
         * returns new Result w/ bestMove && bestValue
         * this code follows the pseudocode given in class
         */
        private Result minimaxVal(Board b, int d, int alpha, int beta) //d is depth
        {

            int bestValue = 0;
            int bestMove = 0;

            if (b.gameOver() || d == 0) //if the game is over or depth is 0
            {
                return new Result(0, evaluate(b));
            }
            if (b.whoseMove() == Position.Top)
            { //if TOP is max

                bestValue = Int32.MinValue;
                for (int move = 7; move <= 12 && alpha < beta; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b); //duplicate board
                        b1.makeMove(move, false);
                        Result val = minimaxVal(b1, d - 1, alpha, beta);
                        if (val.GetScore() > bestValue)
                        {
                            bestValue = val.GetScore(); bestMove = move;
                        }
                        if (bestValue > alpha)
                            alpha = bestValue;
                    }
                }
                return new Result(bestMove, bestValue);
            }
            else
            {
                bestValue = int.MaxValue;
                for (int move = 0; move <= 5 && alpha < beta; move++) //bottom row
                {
                    if (b.legalMove(move))
                    {

                        Board b1 = new Board(b);
                        b1.makeMove(move, false);
                        Result val = minimaxVal(b1, d - 1, alpha, beta);
                        if (val.GetScore() < bestValue)
                        {
                            bestValue = val.GetScore(); bestMove = move;
                        }
                        if (bestValue < beta)
                            beta = bestValue;
                    }

                }


            }
            return new Result(bestMove, bestValue);

        }




        /* class Result 
         * stores: minimax's result
         * contains setters and getters 
         */


        class Result
        {

            private int best_move;
            private int best_score;

            public Result(int move, int score)
            {
                best_move = move;
                best_score = score;

            }

            public int GetMove() { return best_move; } // move getter 

            public int GetScore() { return best_score; }  //score getter

            public void SetMove(int move) { best_move = move; } //move setter

            public void SetScore(int score) { best_score = score; } //score setter 
        }


    }
}

