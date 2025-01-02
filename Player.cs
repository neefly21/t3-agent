using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class Player
    {

        public string Name { get; set; }

        public bool IsHuman { get; set; } = false;

        public double DecayGamma { get; set; } = 0.9;

        public double LearningRate { get; set; } = 0.2;

        public double ExplorationRate { get; set; } = 0.3;

        public List<int> States { get; set; } = new List<int>();

        public Dictionary<int, double> StatesValue { get; set; } = new Dictionary<int, double>();

        public void SetName(string name) => Name = name;

        public void SetIsHuman() => IsHuman = true;

        public void ResetState()
        {
            States = new List<int>();
        }

        public void ReceiveReward(double reward)
        {
            States.Reverse();
            foreach (var st in States)
            {
                if (!StatesValue.Any(x => x.Key == st)) StatesValue.Add(st, 0.0);
                else StatesValue[st] += LearningRate * (DecayGamma * reward - StatesValue[st]);

                reward = StatesValue[st];
            }
        }

        public int ChooseAction(List<int> possibleActions)
        {
            if (possibleActions.Count == 0) return -1;
            double max_value = 0.0;
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
                    if (!StatesValue.Any(x => x.Key == nextAction)) tempValue = 0;
                    else tempValue = StatesValue[nextAction];

                    if (tempValue > max_value)
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
