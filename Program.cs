namespace TicTacToe
{
    public class Program
    {

        public static void TrainAndSaveWeights(int gamesToTrain)
        {
            Player p1 = new Player();
            p1.SetName($"p1_{gamesToTrain}");

            Player p2 = new Player();
            p2.SetName($"p2_{gamesToTrain}");

            State.TrainAgent(gamesToTrain, p1, p2);
            State.SaveWeights(p1, p2);
        }

        /*
         *  Debugging - print game boards like this:
            var badStateTest = new List<int>() { 6, 4, 1, 8, 7, 0 };
            State.PrintGameStateBoard(badStateTest);        

            Board hashes for a slightly better understanding of the game?
        
            var p2Win = new List<int>() { 6, 4, 1, 8, 7, 0 };
            Console.WriteLine($"State Hash: {State.BoardToStateHash(p2Win)}");
        */

        public static void PlayAgainstTrainedAgent() 
        {
            Player p1 = new Player();
            p1.SetName("noah");
            p1.SetIsHuman();
            Player p2 = State.LoadWeight(); 

            State.PlayAgainstAgent(p1, p2);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("TicTacToe Agent v0.02");

            TrainAndSaveWeights(15000);
            PlayAgainstTrainedAgent();
        }
    }
}
