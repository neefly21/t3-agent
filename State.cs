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

        public static int CheckMovesForEachPlayer(List<int> state)
        {
            List<int> p1 = new List<int>(), p2 = new List<int>();
            for (int i = 0; i < state.Count; i++)
            {
                if (i % 2 == 0) p1.Add(state[i]);
                else            p2.Add(state[i]);
            }

            foreach (var winningPattern in winningStates)
            {
                if (!winningPattern.Except(p1).Any())      return 1;
                else if (!winningPattern.Except(p2).Any()) return 2;
            }

            return 0;
        }

        public static List<int> PossibleActions(List<int> state) => possibleStates.Except(state).ToList();

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

        private static int GetSimplifiedAction(List<int> state, List<int> possibleActions, Player actionPlayer)
        {
            if (actionPlayer.IsHuman) 
            {
                PrintGameStateBoard(state);
                return actionPlayer.HumanChooseAction(possibleActions);
            }
            else return actionPlayer.ChooseAction(possibleActions);
        }
        
        private static SimpleGame SimplifiedGame(Player p1, Player p2)
        {
            int activePlayerAction = -1, stateProduct = 0;
            List<int> state = new List<int>();
            bool isTerminalState = false;
            int winner = 0;

            while (!isTerminalState)
            {
                var possibleActions = PossibleActions(state);

                if (possibleActions.Count == 0) 
                {
                    isTerminalState = true;
                }

                activePlayerAction = state.Count % 2 == 0 ? GetSimplifiedAction(state, possibleActions, p1) : GetSimplifiedAction(state,possibleActions, p2);

                if (activePlayerAction > -1) { 
                    state.Add(activePlayerAction);

                    if (state.Count - 1 % 2 == 0 && activePlayerAction > 0) p1.States.Add(activePlayerAction);
                    else p2.States.Add(activePlayerAction);
                }

                winner = CheckMovesForEachPlayer(state);
                if (winner > 0) isTerminalState = true;
            }

            if (winner > 0) Console.WriteLine($"p{winner} wins! - {string.Join(",", state)}");
            else Console.WriteLine("Draw!");

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

                if (simpleSims.Last().Winner == 1) p1Wins++;
                else p2Wins++;
            }

            Console.WriteLine($"P1 wins: {p1Wins} | P2 wins: {p2Wins}\n{gamesToTrain} games ran in {timer.Elapsed}");

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

        public static void PlayGame(int gamesToTrain, Player p1, Player p2)
        {
            int p1Wins = 0, p2Wins = 0;
            List<int> state = new List<int>();
            bool isTerminalState = false;
            Random rand = new Random();
            int latestAction = 0;
            int winner = 0;

            while (!isTerminalState)
            {
                var possibleActions = PossibleActions(state);

                //if (state.Count % 2 == 0) { latestAction =  }
                //else { }

                state.Add(possibleActions[rand.Next(possibleActions.Count())]);

                if (state.Count > 4) winner = CheckMovesForEachPlayer(state);
                if (state.Count == 8) isTerminalState = true;
            }

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

        public static void PrintGameStateBoard(List<int> state) => DisplayGameState(state);

        private static List<Tuple<int, string>> ConvertStateToTuple(List<int> state)
        {
            var displayBoard = new List<Tuple<int, string>>();
            for (int i = 0; i < state.Count; i++) displayBoard.Add(Tuple.Create(state[i], i % 2 == 0 ? "X" : "O"));
            displayBoard.Sort((x, y) => y.Item1.CompareTo(x.Item1));
            return displayBoard;
        }

        private static void DisplayGameState(List<int> state)
        {
            var displayBoard = ConvertStateToTuple(state);
            for (int i = 0; i < 9; i++)
            {
                char boardPos = '_';
                foreach (var move in displayBoard) 
                {
                    if (move.Item1 == i) boardPos = move.Item2[0]; continue;
                }

                Console.Write($" {boardPos} ");

                if (i == 2 || i == 5) Console.WriteLine();
            }
            
        }
    }
}
