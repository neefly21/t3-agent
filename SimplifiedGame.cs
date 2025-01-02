using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class SimpleGame
    {
        public SimpleGame(List<int> gameActions, int winner)
        {
            Winner = winner;
            GameActions = gameActions;
        }
        public int Winner { get; set; }
        public List<int> GameActions { get; set; } = new List<int>();
    }
}
