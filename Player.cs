namespace TicTacToe
{
    public class Player
    {

        public string Name { get; set; }

        public bool IsHuman { get; set; } = false;

        public double DecayGamma { get; set; } = 0.9;

        public double LearningRate { get; set; } = 0.25;

        public double ExplorationRate { get; set; } = 0.05;

        public List<string> States { get; set; } = new List<string>();

        public Dictionary<string, double> StatesValue { get; set; } = new Dictionary<string, double>();

        public void SetName(string name) => Name = name;

        public void SetIsHuman() => IsHuman = true;

        public void ResetState()
        {
            States = new List<string>();
        }

        public void ReceiveReward(double reward)
        {
            States.Reverse();
            foreach (var st in States)
            {
                if (StatesValue.ContainsKey(st)) StatesValue[st] += LearningRate * (DecayGamma * reward - StatesValue[st]);
                else StatesValue.Add(st, 0.0);
                reward = StatesValue[st];
            }
        }

        public int ChooseAction(int[] board, List<int> possibleActions)
        {
            if (possibleActions.Count == 0) return -1;
            double max_value = -999;
            Random rand = new Random();
            int action = possibleActions[rand.Next(possibleActions.Count)];

            //Random/Explore
            if (rand.NextDouble() <= ExplorationRate)
            {
                return possibleActions[rand.Next(possibleActions.Count)];
            }
            //Utilize trained weights
            else
            {
                double tempValue = 0.0;
                //Does not prune excessively deep low-value choices.
                foreach (var nextAction in possibleActions)
                {
                    var tempBoard = board.ToArray();
                    tempBoard[nextAction] = possibleActions.Count() % 2 == 0 ? 1 : -1;//Player 1 or 2

                    if (StatesValue.ContainsKey(string.Join(',', tempBoard)))
                    {
                        tempValue = StatesValue[string.Join(',', tempBoard)];
                    }
                    else
                    {
                        tempValue = 0;
                    }

                    if (tempValue >= max_value)
                    {
                        action = nextAction;
                        max_value = tempValue;
                    }
                }
            }

            return action;
        }

        public int HumanChooseAction(List<int> possibleActions)
        {

            Console.WriteLine($"Possible choices: {string.Join(',', possibleActions)}\n>");

            var userChoice = Console.ReadLine();

            return Convert.ToInt32(userChoice);
        }

    }
}
