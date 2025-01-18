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

        private static void DictionarySearchValidation() 
        {
            var newDict1 = new List<int>() { 4, 3, 8 };
            var newDict2 = new List<int>() { 3, 2, 8 };

            Dictionary<string, double> BoardPositionToPrime = new Dictionary<string, double>()
            {
                { string.Join(',', newDict1), 0.5 },
                { string.Join(',', newDict2), .75 }
            };

            var testSearch = new List<int>() { 4 };

            if (BoardPositionToPrime.ContainsKey(string.Join(',', testSearch)))
            {
                var blah = BoardPositionToPrime[string.Join(',', testSearch)];

                Console.WriteLine(blah);
            }
            else
            {
                Console.WriteLine("Key doesn't exist, value set to 0!");
            }
        }

        public static void DisplayBoard(int[] board) 
        {
            for (var i = 0; i < 9; i++)
            {
                Console.Write(board[i]);
                if (i == 2 || i == 5) Console.WriteLine();
            }
        }

        static List<List<int>> winningStates = new List<List<int>>() { 
            //Horizontal
            new List<int>(){ 0, 1, 2 }, new List<int>() { 3, 4, 5 }, new List<int>() { 6, 7, 8 },
            //Vertical
            new List<int>(){ 0, 3, 6 }, new List<int>() { 1, 4, 7 }, new List<int>() { 2, 5, 8 },
            //Diagnal
            new List<int>(){ 0, 4, 8 }, new List<int>() { 2, 4, 6 }
        };

        public static void Main(string[] args)
        {
            Console.WriteLine("TicTacToe Agent v0.03");

            //New State representation + storage

            //Storage
            PlayAgainstTrainedAgent();
        }
    }
}
