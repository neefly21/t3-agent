﻿namespace TicTacToe
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
            Console.WriteLine("TicTacToe Agent v0");
            PlayAgainstTrainedAgent();
        }
    }
}
