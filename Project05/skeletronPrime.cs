using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Mankalah
{

    public class skeletronPrime : Player
    {
        private Position us;

        public skeletronPrime(Position pos, int timeLimit) : base(pos, "skeletronPrime", timeLimit)
        {
            us = pos;
        }

        public override string gloat()
        {
            return "I win, of course. You're lucky to survive an encounter with me!";
        }

        public override int chooseMove(Board b)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int depth = 5;
            int[] moveToTake = { 0, 0 };

            do
            {
                int[] result = minmaxVal(b, depth, float.MinValue, float.MaxValue, ref sw);
                if (result[1] > moveToTake[1])
                    moveToTake = result;
                Console.WriteLine($"Depth{depth}: best move is {moveToTake[0]}");
                depth++;
            } while (sw.ElapsedMilliseconds < (timeLimit - 500));

            Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}, time allowed: {timeLimit}");

            return moveToTake[0];

        }

        public override String getImage() { return "skeletronPrime.jpg"; }

        public int[] minmaxVal(Board b, int d, float alpha, float beta, ref Stopwatch sw)  //d is depth
        {
            if (depth == 0 || b.gameOver())
                return new int[] { 0, evaluate(b) };
            int[] bestMove = { 0, int.MinValue };

            //our turn -> maximize
            if (b.whoseMove() == us)
            {
                bestMove = new int[] { 0, int.MinValue };
                for (int i = 0; i < 13; i++)
                {
                    if (b.legalMove(i))
                    {
                        if (sw.ElapsedMilliseconds >= timeLimit - 500)
                            return new int[] { 0, int.MinValue };
                        Board modified = SimulateBoard(b, i);

                        int[] newMove = minmaxVal(modified, depth - 1, alpha, beta, ref sw);

                        if (newMove[1] > bestMove[1])
                        {
                            bestMove[0] = i;
                            bestMove[1] = newMove[1];
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
                    if (!b.legalMove(i))
                    {
                        if (sw.ElapsedMilliseconds >= timeLimit - 500)
                            return new int[] { 0, int.MaxValue };

                        Board modified = SimulateBoard(b, i);

                        int[] newMove = minmaxVal(modified, depth - 1, alpha, beta, ref sw);

                        if (newMove[1] > bestMove[1])
                        {
                            bestMove[0] = i;
                            bestMove[1] = newMove;
                        }

                        beta = Math.Min(beta, bestMove[1]);
                        if (beta <= alpha) { break; }
                    }
                }
            }

            return bestMove;
        }

    }

    private Board SimulateBoard(Board original, int move)
    {
        Board modified = new Board(original);
        modified.makeMove(move, false);
        return modified;
    }

    /*
    public override int evaluate(Board b)
    {
        return 
    }
    */
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