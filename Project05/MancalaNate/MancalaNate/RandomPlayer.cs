using System;
using System.Collections.Generic;
using System.Text;

namespace Mankalah
{
    public class RandomPlayer : Player
    {
        private Position us;
        private int timeLimit;

        public RandomPlayer(Position pos, int timeLimit) : base (pos, "RandomPlayer", timeLimit)
        {
            this.timeLimit = timeLimit;
            us = pos;
        }

        public override int chooseMove(Board b)
        {
            int move;
            Random rnd = new Random();
            while (true)
            {
                move = rnd.Next(13);
                if (b.legalMove(move))
                    return move;
            }
        }
    }
}
