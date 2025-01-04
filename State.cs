using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class State
    {
        private static Random _rand = new Random();

        static List<int> possibleStates = new List<int>()
        { 0, 1, 2,
          3, 4, 5,
          6, 7, 8 };

        static List<List<int>> winningStates = new List<List<int>>() { 
            //Horizontal
            new List<int>(){ 0, 1, 2 }, new List<int>() { 3, 4, 5 }, new List<int>() { 6, 7, 8 },
            //Vertical
            new List<int>(){ 0, 3, 6 }, new List<int>() { 1, 4, 7 }, new List<int>() { 2, 5, 8 },
            //Diagnal
            new List<int>(){ 0, 4, 8 }, new List<int>() { 2, 4, 6 }
        };

        static Dictionary<int, int> BoardPositionToPrime = new Dictionary<int, int>() 
        {{0, 2},{1, 3},{2, 5},{3, 7},{4, 11},{5, 13},{6, 17},{7, 19},{8, 23}};

        public static string BoardToStateHash(int[] board) => string.Join(',', board);
        
        public static int CheckMovesForEachPlayer(int[] board)
        {
            List<int> p1 = new List<int>(), p2 = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == 1) p1.Add(i);
                else if (board[i] == 2) p2.Add(i);
            }

            foreach (var winningPattern in winningStates)
            {
                if (!winningPattern.Except(p1).Any()) return 1;
                else if (!winningPattern.Except(p2).Any()) return 2;
            }

            return 0;
        }

        public static List<int> PossibleActions(int[] board)
        {
            var possibleActions = new List<int>();

            for (var i = 0; i < 9; i++) if (board[i] == 0) possibleActions.Add(i);

            return possibleActions;

        }

        private static int GetSimplifiedAction(int[] board, List<int> possibleActions, Player actionPlayer)
        {
            if (actionPlayer.IsHuman) 
            {
                DisplayBoard(board);
                return actionPlayer.HumanChooseAction(possibleActions);
            }
            else return actionPlayer.ChooseAction(board, possibleActions);
        }
        
        private static SimpleGame SimplifiedGame(Player p1, Player p2)
        {
            int[] board = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int activePlayerAction = -1, stateProduct = 0;
            List<int> state = new List<int>();
            var isFirstPlayer = state.Count() % 2 == 0;
            bool isTerminalState = false;
            int winner = 0;

            while (!isTerminalState)
            {
                isFirstPlayer = state.Count() % 2 == 0;
                var possibleActions = PossibleActions(board);

                if (possibleActions.Count() == 0) isTerminalState = true;

                activePlayerAction = isFirstPlayer ? GetSimplifiedAction(board, possibleActions, p1) : GetSimplifiedAction(board, possibleActions, p2);

                if (activePlayerAction > -1) { 
                    state.Add(activePlayerAction);
                    board[activePlayerAction] = isFirstPlayer ? 1 : 2;

                    if (isFirstPlayer && activePlayerAction >= 0) p1.States.Add(string.Join(',', board));
                    else p2.States.Add(string.Join(',', board));
                }

                winner = CheckMovesForEachPlayer(board);
                if (winner > 0) isTerminalState = true;
            }

            if (!p1.IsHuman && !p2.IsHuman) 
            {
                if (winner == 1)
                {
                    p1.ReceiveReward(1);
                    p1.ResetState();
                    p2.ReceiveReward(0);
                    p2.ResetState();
                }
                else if (winner == 2)
                {
                    p1.ReceiveReward(0);
                    p1.ResetState();
                    p2.ReceiveReward(1);
                    p2.ResetState();
                }
            }

            return new SimpleGame(state, winner);
        }

        public static void TrainAgent(int gamesToTrain, Player p1, Player p2)
        {
            int p1Wins = 0, p2Wins = 0;
            List<SimpleGame> simpleSims = new List<SimpleGame>();
            Stopwatch timer = new Stopwatch();

            timer.Start();
            while (simpleSims.Count() < gamesToTrain)
            {
                simpleSims.Add(SimplifiedGame(p1, p2));

                if (simpleSims.Count() % 50000 == 0) Console.WriteLine($"{simpleSims.Count()} of {gamesToTrain} | P1: {p1Wins} v P2: {p2Wins}");
            }

            Console.WriteLine($"{gamesToTrain} games ran in {timer.Elapsed}");

            foreach (var player in new[] { p1, p2 })
            {
                Console.WriteLine($"{player.Name} strategies:");

                foreach (var state_value in player.StatesValue)
                {
                    Console.WriteLine($"{state_value.Key} : {state_value.Value}");
                }

                Console.WriteLine();
            }
        }

        public static void PlayAgainstAgent(Player p1, Player p2)
        {
            Console.Write("How many games would you like to play:");
            var gamesToPlay = Console.ReadLine();

            p1.IsHuman = true;
            var gamesPlayed = 0;
            int p1Wins = 0, p2Wins = 0, ties = 0;

            while (gamesPlayed < Convert.ToInt32(gamesToPlay))
            {
                var gameVsHuman = SimplifiedGame(p1, p2);

                if (gameVsHuman.Winner == 1) p1Wins++;
                else if (gameVsHuman.Winner == 2) p2Wins++;
                else ties++;

                gamesPlayed++;
            }

            Console.WriteLine($"Session Win/Loss: P1:{p1Wins} P2:{p2Wins}");
        }

        private static List<Tuple<int, string>> ConvertStateToTuple(List<int> state)
        {
            var displayBoard = new List<Tuple<int, string>>();
            for (int i = 0; i < state.Count; i++) displayBoard.Add(Tuple.Create(state[i], i % 2 == 0 ? "X" : "O"));
            displayBoard.Sort((x, y) => y.Item1.CompareTo(x.Item1));
            return displayBoard;
        }

        public static void PrintGameStateBoard(List<int> state) => DisplayGameState(state);

        public static void DisplayBoard(int[] board)
        {
            for (var i = 0; i < 9; i++)
            {
                var playerSymbol = "-";
                if(board[i] > 0) playerSymbol = board[i] == 1 ? "X" : "O";
                Console.Write($"{playerSymbol}\t");
                if (i == 2 || i == 5) Console.WriteLine();
            }
        }

        private static void DisplayGameState(List<int> state)
        {
            var displayBoard = ConvertStateToTuple(state);
            for (int i = 0; i < 9; i++)
            {
                char boardPos = '_';
                for (var j = 0; j < displayBoard.Count; j++) 
                {
                    if (displayBoard[j].Item1 == i)
                    {
                        boardPos = displayBoard[j].Item2[0];
                        continue;
                    }
                }

                Console.Write($" {boardPos} ");

                if (i == 2 || i == 5) Console.WriteLine();
            }
            
        }

        public static void SaveWeights(Player p1, Player p2)
        {
            var players = new List<Player>() { p1, p2 };
            var weightsPath = "C:\\projects\\DotNetCasino\\TicTacToe\\Weights";
            foreach (var player in players)
            {
                var playerWeightsString = JsonSerializer.Serialize(player);
                File.WriteAllText($"{weightsPath}\\{player.Name}", playerWeightsString);
            }
        }

        public static Player LoadWeight()
        {
            Console.Write("Please enter weight file name: ");
            var weightName = Console.ReadLine();
            var weightsPath = $"C:\\projects\\DotNetCasino\\TicTacToe\\Weights\\{weightName}";
            var json = File.ReadAllText(weightsPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<Player>(json, options);
        }

    }
}
