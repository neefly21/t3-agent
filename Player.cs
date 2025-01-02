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

        public List<List<int>> States { get; set; } = new List<List<int>>();

        public Dictionary<int, double> StatesValue { get; set; } = new Dictionary<int, double>();

        public void SetName(string name) => Name = name;

        public void SetIsHuman() => IsHuman = true;

        public void ResetState()
        {
            States = new List<List<int>>();
        }

        public void ReceiveReward(double reward)
        {
            States.Reverse();
            foreach (var st in States)
            {
                var stateValue = State.BoardToStateHash(st);
                if (!StatesValue.Any(x => x.Key == stateValue)) StatesValue.Add(stateValue, 0.0);
                else StatesValue[stateValue] += LearningRate * (DecayGamma * reward - StatesValue[stateValue]);

                reward = StatesValue[stateValue];
            }
        }

        public int ChooseAction(List<int> state, List<int> possibleActions)
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
                    var tempState = state.ToList();
                    tempState.Add(nextAction);

                    var nextBoardHash = State.BoardToStateHash(tempState);
                    //Does the current board + next action have a strategy? if not, set to 0
                    if (!StatesValue.Any(x => x.Key == nextBoardHash)) tempValue = 0;
                    else tempValue = StatesValue[nextBoardHash];

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
