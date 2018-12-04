using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Mankalah
{

    public class ck45Player : Player
    {
        private Position us;
        private int timeLimit;
        int maxDepth;

        public ck45Player(Position pos, int timeLimit) : base(pos, "ChanKim", timeLimit)
        {
            this.timeLimit = timeLimit;
            us = pos;
            maxDepth = 50;
        }

        public override string gloat()
        {
            return "ChanKim wins!";
        }

        public override int chooseMove(Board b)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int depth = 1;
            int[] moveToTake = { 0, int.MinValue };

            maxDepth = 50;

            do
            {
                int[] result = minmaxVal(b, depth, float.MinValue, float.MaxValue, ref sw);
                if (result[1] > moveToTake[1])
                    moveToTake = result;
                Console.WriteLine($"Depth{depth}: best move is {moveToTake[0]} with score {moveToTake[1]}");
                depth++;
            } while (sw.ElapsedMilliseconds < (timeLimit - 100) && depth <= maxDepth);

            Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}, time allowed: {timeLimit}");

            return moveToTake[0];

        }

        public override String getImage() { return "skeletronPrime.jpg"; }

        public int[] minmaxVal(Board b, int depth, float alpha, float beta, ref Stopwatch sw)
        {
            if (depth == 0)
                return new int[] { 0, evaluate(b) };
            if (b.gameOver())
            {
                maxDepth = depth;
                return new int[] { 0, endGameEval(b) };
            }
            int[] bestMove = { 0, int.MinValue };

            //our turn -> maximize
            if (b.whoseMove() == us)
            {
                bestMove = new int[] { 0, int.MinValue };
                for (int i = 0; i < 13; i++)
                {
                    if (b.legalMove(i))
                    {
                        if (sw.ElapsedMilliseconds >= timeLimit - 100)
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
                        if (sw.ElapsedMilliseconds >= timeLimit - 100)
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

            if (maxDepth != 50)
                maxDepth = depth;
            return bestMove;
        }
        private Board SimulateBoard(Board original, int move)
        {
            Board modified = new Board(original);
            modified.makeMove(move, false);
            return modified;
        }

        /*Evaluate function used when the end game is not clear
         * Uses heuristic found on github by previous student
         * Code belongs to Chan Kim (ck45@students.calvin.edu)
         */
        public override int evaluate(Board b)
        {
            int score = b.stonesAt(13) - b.stonesAt(6);
            int stonesTotal = 0;
            int goAgainsTotal = 0;
            int capturesTotal = 0;

            for (int i = 7; i <= 12; i++)
            {
                int priority = 0;
                int target = b.stonesAt(i) % (13 - i);
                int targetStonesAt = b.stonesAt(target + 7);
                if (b.whoseMove() == Position.Bottom)
                {
                    stonesTotal -= b.stonesAt(i);

                    if ((b.stonesAt(i) - (13 - i) == 0) || (b.stonesAt(i) - (13 - i)) == 13)
                    {
                        goAgainsTotal -= (1 + priority);
                    }
                    if (targetStonesAt == 0 && b.stonesAt(i) == (13 - i + target + 7))
                    {
                        capturesTotal += (b.stonesAt(i) + b.stonesAt(12 - target));
                    }
                }
                else
                {
                    stonesTotal += b.stonesAt(i);

                    if ((b.stonesAt(i) - (13 - i) == 0) || (b.stonesAt(i) - (13 - i)) == 13)
                    {
                        goAgainsTotal += (1 + priority);
                    }
                    if (targetStonesAt == 0 && b.stonesAt(i) == (13 - i + target + 7))
                    {
                        capturesTotal -= (b.stonesAt(i) + b.stonesAt(12 - target));
                    }
                }
                priority++;
            }

            for (int i = 0; i <= 5; i++)
            {
                int priority = 0;
                int target = b.stonesAt(i) % (13 - i);
                int targetStonesAt = b.stonesAt(target);
                if (b.whoseMove() == Position.Bottom)
                {
                    stonesTotal += b.stonesAt(i);

                    if ((b.stonesAt(i) - (6 - i) == 0) || (b.stonesAt(i) - (6 - i)) == 13)
                    {
                        goAgainsTotal -= (1 + priority);
                    }
                    if (targetStonesAt == 0 && b.stonesAt(i) == (13 - i + target))
                    {
                        capturesTotal -= (b.stonesAt(i) + b.stonesAt(12 - target));
                    }
                }
                else
                {
                    stonesTotal -= b.stonesAt(i);

                    if ((b.stonesAt(i) - (6 - i) == 0) || (b.stonesAt(i) - (6 - i)) == 13)
                    {
                        goAgainsTotal += (1 + priority);
                    }
                    if (targetStonesAt == 0 && b.stonesAt(i) == (13 - i + target))
                    {
                        capturesTotal += (b.stonesAt(i) + b.stonesAt(12 - target));
                    }
                }
                priority++;
            }

            score += stonesTotal + capturesTotal + goAgainsTotal;
            return score;

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
