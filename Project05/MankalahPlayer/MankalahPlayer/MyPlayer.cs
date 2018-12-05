using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Mankalah
{

    public class Weighted2Player : Player
    {
        private Position us;
        private int timeLimit;

        public Weighted2Player(Position pos, int timeLimit) : base(pos, "Weighted2", timeLimit)
        {
            this.timeLimit = timeLimit;
            us = pos;
        }

        public override string gloat()
        {
            return "Weighted 2 Wins!";
        }

        public override int chooseMove(Board b)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int depth = 5;
            int[] moveToTake = { 0, int.MinValue };


            do
            {
                int[] result = minmaxVal(b, depth, float.MinValue, float.MaxValue, ref sw);
                if (result[1] > moveToTake[1])
                    moveToTake = result;
                Console.WriteLine($"Depth{depth}: best move is {moveToTake[0]} with score {moveToTake[1]}");
                depth++;
            } while (sw.ElapsedMilliseconds < (timeLimit) && depth <= 50);

            Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}, time allowed: {timeLimit}");

            return moveToTake[0];

        }

        public override String getImage() { return "skeletronPrime.jpg"; }

        public int[] minmaxVal(Board b, int depth, float alpha, float beta, ref Stopwatch sw)
        {
            if (depth == 0)
                return new int[] { 0, evaluate(b) };
            if (b.gameOver())
                return new int[] { 0, endGameEval(b) };
            int[] bestMove = { 0, int.MinValue };

            //our turn -> maximize
            if (b.whoseMove() == us)
            {
                bestMove = new int[] { 0, int.MinValue };
                for (int i = 0; i < 13; i++)
                {
                    if (b.legalMove(i))
                    {
                        if (sw.ElapsedMilliseconds >= timeLimit - 20)
                            //return new int[] { 0, int.MinValue };
                            return new int[] { i, evaluate(b) };
                        Board modified = SimulateBoard(b, i);

                        int[] newMove = minmaxVal(modified, depth - 1, alpha, beta, ref sw);

                        if (newMove[1] > bestMove[1])
                        {
                            bestMove[0] = i;
                            bestMove[1] = newMove[1];
                            //Console.WriteLine($"Changed bestMove in max to move {i} with value {bestMove[1]}");
                        }

                        alpha = Math.Max(alpha, bestMove[1]);
                        if (beta <= alpha) { break; }
                    }
                }
            }
            //Their turn -> minimize
            else
            {
                bestMove = new int[] { 0, int.MaxValue };

                for (int i = 0; i < 13; i++)
                {
                    if (b.legalMove(i))
                    {
                        if (sw.ElapsedMilliseconds >= timeLimit - 20)
                            //return new int[] { 0, int.MaxValue };
                            return new int[] { i, evaluate(b) };

                        Board modified = SimulateBoard(b, i);

                        int[] newMove = minmaxVal(modified, depth - 1, alpha, beta, ref sw);

                        if (newMove[1] < bestMove[1])
                        {
                            bestMove[0] = i;
                            bestMove[1] = newMove[1];
                            //Console.WriteLine($"Changed bestMove in min to move {i} with value {bestMove[1]}");
                        }

                        beta = Math.Min(beta, bestMove[1]);
                        if (beta <= alpha) { break; }
                    }
                }
            }

            return bestMove;
        }
        private Board SimulateBoard(Board original, int move)
        {
            Board modified = new Board(original);
            modified.makeMove(move, false);
            return modified;
        }

        /*Evaluate function used when the end game is not clear
         * Uses 5 heuristics found by researchers at the University of Kansas to be the best
         * Heuristic research found at https://fiasco.ittc.ku.edu/publications/documents/Gifford_ITTC-FY2009-TR-03050-03.pdf
         * h1 is my score - opponent's score
         * h2 is how close I am to winning
         * h3 is how close the opponent is to winning
         * h4 is the number of stones close to my home
         * h5 is the number of stones far from my home (on my side of the board)
         */
        public override int evaluate(Board b)
        {
            int h1, h2, h3, h4, h5, sum;
            if (us == Position.Top)
            {
                h2 = b.stonesAt(13);
                h3 = b.stonesAt(6);
                h1 = h2 - b.scoreBot();
                h4 = b.stonesAt(11) + b.stonesAt(12);
                h5 = b.stonesAt(7) + b.stonesAt(8);
            }
            else
            {
                h2 = b.stonesAt(6);
                h3 = b.stonesAt(13);
                h1 = h2 - b.scoreTop();
                h4 = b.stonesAt(4) + b.stonesAt(5);
                h5 = b.stonesAt(0) + b.stonesAt(1);
            }
            sum = (h1 + 10) + (h2 + 8) - (h3 + 8) - (h4 + 4) + (h5 + 4);
            if (h2 > 24)    //If we have at least half of the stones
                sum += 500;
            else if (h3 > 24)   //If opponent has at least half of the stones
                sum -= 500;
            return sum;

            //int score;
            //if (us == Position.Top)
            //    score = b.scoreTop() - b.scoreBot();
            //else
            //    score = b.scoreBot() - b.scoreTop();
            //return score;
        }

        public int endGameEval(Board b)
        {
            if (us == Position.Top)
                return (b.scoreTop() - b.scoreBot()) * 1000;
            else
                return (b.scoreBot() - b.scoreTop()) * 1000;
        }

    }
}



/* Notes:
 * Use Alpha Beta pruning if want to win (6 lines of code, confusing)
 * Find online heuristics for Mancala
 * Use 2 evaluate functions
 *      1 for when you can't see the end of the game
 *      1 for when you can
 *          choose moves based on raw score (multiply by 1000 to make sure it's chosen over the heuristic evaluations)
 * Always choose best move
 */
