namespace TicTacToe
{
    public class SimpleGame
    {
        public SimpleGame(int[] board, int winner)
        {
            Winner = winner;
            GameActions = board;
        }
        public int Winner { get; set; }
        public int[] GameActions { get; set; }
    }
}
