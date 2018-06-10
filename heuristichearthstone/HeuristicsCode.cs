using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks;
using SabberStoneCore.Tasks.PlayerTasks;
using heuristichearthstone.Meta;
using heuristichearthstone.Nodes;
using heuristichearthstone.Score;
namespace heuristichearthstone
{
    class HeuristicsCode
    {
        public List<Game> forward_gameStates;

        public HeuristicsCode()
        {
            forward_gameStates = new List<Game>();
        }

        private string IfThenElse(bool condition, string true_result, string false_result)

        {

            if (condition)

                return true_result;



            return false_result;

        }

        public object MapStringToFunction(string function_name, List<object> parameters)

        {

            switch (function_name)

            {

                case "forward_gameStates":

                    return this.forward_gameStates;


                case "IfThenElse":

                    return this.IfThenElse((bool)parameters[0], (string)parameters[1], (string)parameters[2]);

                //break;

                case "chooseMaxNumMinionPlayerFromForward_GameStates":

                    return this.chooseMaxNumMinionPlayerFromForward_GameStates(forward_gameStates);

                //break;

                case "chooseMinNumMinionPlayerFromForward_GameStates":

                    return this.chooseMinNumMinionPlayerFromForward_GameStates(forward_gameStates);

                //break;

                case "chooseMaxNumMinionOpponentFromForward_GameStates":

                    return this.chooseMaxNumMinionOpponentFromForward_GameStates(forward_gameStates);
                    
                //break;

                default:

                    if (Char.IsNumber(function_name[0]))

                        return Int32.Parse(function_name);

                    return function_name;

                    //return Int32.Parse(function_name);

                    //break;

            }

        }
        public object Compile(HeuristicNode heuristic, List<Game> forward_gameStates)

        {

            this.forward_gameStates = forward_gameStates;





            List<object> parameters = new List<object>();



            for (int i = 0; i < heuristic.children.Length; i++)

            {

                if (heuristic.children[i] != null)

                {

                    parameters.Add(this.Compile(heuristic.children[i], forward_gameStates));

                }

            }



            return this.MapStringToFunction(heuristic.name, parameters);

        }
        public List<Game> chooseMaxNumMinionPlayerFromForward_GameStates(List<Game> forward_gameStates)
        {
            int max = 0;
            foreach (Game game in forward_gameStates)
            {
                if (max < game.Player1.BoardZone.Count)
                {
                    max = game.Player1.BoardZone.Count;
                }
            }
            return (List<Game>)forward_gameStates.Where(p => p.Player1.BoardZone.Count == max);
        }
        public List<Game> chooseMinNumMinionPlayerFromForward_GameStates(List<Game> forward_gameStates)
        {
            int min = 999;
            foreach (Game game in forward_gameStates)
            {
                if (min > game.Player1.BoardZone.Count)
                {
                    min = game.Player1.BoardZone.Count;
                }
            }
            return (List<Game>)forward_gameStates.Where(p => p.Player1.BoardZone.Count == min);
        }
        public List<Game> chooseMaxNumMinionOpponentFromForward_GameStates(List<Game> forward_gameStates)
        {
            int max = 0;
            foreach (Game game in forward_gameStates)
            {
                if (max < game.Player1.BoardZone.Count)
                {
                    max = game.Player1.BoardZone.Count;
                }
            }
            return (List<Game>)forward_gameStates.Where(p => p.Player2.BoardZone.Count == max);
        }
        public List<Game> chooseMinNumMinionOpponentFromForward_GameStates(List<Game> forward_gameStates)
        {
            int min = 999;
            foreach (Game game in forward_gameStates)
            {
                if (min > game.Player1.BoardZone.Count)
                {
                    min = game.Player1.BoardZone.Count;
                }
            }
            return (List<Game>)forward_gameStates.Where(p => p.Player2.BoardZone.Count == min);
        }
        public static List<Game> chooseMaxSumAttackMinionPlayerFromForward_GameStates(List<Game> forward_gameStates)
        {
            int max = 0;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int attackOfThisGameSoFar = 0;
                var minionList = game.Player1.BoardZone.GetAll.ToList();
                foreach (SabberStoneCore.Model.Entities.IPlayable minion in minionList)
                {
                    var tags = minion.Card.Tags;

                    attackOfThisGameSoFar += tags[GameTag.ATK];
                    //currentCard.a
                }
                if (max < attackOfThisGameSoFar)
                {
                    max = attackOfThisGameSoFar;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }

        public List<Game> chooseMinSumAttackMinionPlayerFromForward_GameStates(List<Game> forward_gameStates)
        {
            int min = 999;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int attackOfThisGameSoFar = 0;
                var minionList = game.Player1.BoardZone.GetAll.ToList();
                foreach (SabberStoneCore.Model.Entities.IPlayable minion in minionList)
                {
                    var tags = minion.Card.Tags;

                    attackOfThisGameSoFar += tags[GameTag.ATK];
                    //currentCard.a
                }
                if (min > attackOfThisGameSoFar)
                {
                    min = attackOfThisGameSoFar;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }
        public List<Game> chooseMaxSumAttackMinionOpponentromForward_GameStates(List<Game> forward_gameStates)
        {
            int max = 0;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int attackOfThisGameSoFar = 0;
                var minionList = game.Player2.BoardZone.GetAll.ToList();
                foreach (SabberStoneCore.Model.Entities.IPlayable minion in minionList)
                {
                    var tags = minion.Card.Tags;

                    attackOfThisGameSoFar += tags[GameTag.ATK];
                    //currentCard.a
                }
                if (max < attackOfThisGameSoFar)
                {
                    max = attackOfThisGameSoFar;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }
        public List<Game> chooseMinSumAttackMinionOpponentFromForward_GameStates(List<Game> forward_gameStates)
        {
            int min = 999;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int attackOfThisGameSoFar = 0;
                var minionList = game.Player2.BoardZone.GetAll.ToList();
                foreach (SabberStoneCore.Model.Entities.IPlayable minion in minionList)
                {
                    var tags = minion.Card.Tags;

                    attackOfThisGameSoFar += tags[GameTag.ATK];
                    //currentCard.a
                }
                if (min > attackOfThisGameSoFar)
                {
                    min = attackOfThisGameSoFar;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }
        public List<Game> chooseMaxManaPlayerFromForward_GameStates(List<Game> forward_gameStates)
        {
            int max = 0;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int remainingMana = game.Player1.RemainingMana;
                if (max < remainingMana)
                {
                    max = remainingMana;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }
        public List<Game> chooseMinManaPlayerFromForward_GameStates(List<Game> forward_gameStates)
        {
            int min = 999;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int remainingMana = game.Player1.RemainingMana;
                if (min > remainingMana)
                {
                    min = remainingMana;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }
        public List<Game> chooseMaxHealthPlayerFromForward_GameStates(List<Game> forward_gameStates)
        {
            int max = 0;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int remainingMana = game.Player1.Hero.Health;
                if (max < remainingMana)
                {
                    max = remainingMana;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }
        public List<Game> chooseMinHealthPlayerFromForward_GameStates(List<Game> forward_gameStates)
        {
            int min = 999;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int remainingMana = game.Player1.Hero.Health;
                if (min > remainingMana)
                {
                    min = remainingMana;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }
        public List<Game> chooseMaxHealthOpponentFromForward_GameStates(List<Game> forward_gameStates)
        {
            int max = 0;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int remainingMana = game.Player2.Hero.Health;
                if (max < remainingMana)
                {
                    max = remainingMana;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }
        public List<Game> chooseMinHealthOpponentFromForward_GameStates(List<Game> forward_gameStates)
        {
            int min = 999;
            List<Game> returnedGameState = new List<Game>();
            foreach (Game game in forward_gameStates)
            {
                int remainingMana = game.Player2.Hero.Health;
                if (min > remainingMana)
                {
                    min = remainingMana;
                    returnedGameState = forward_gameStates;
                }
            }
            return returnedGameState;
        }
    }

    class HeuristicNode

    {

        private int _numberOfChildren = 3;

        public String name;

        public HeuristicNode[] children;



        public HeuristicNode()

        {

            this.name = "";

            this.children = new HeuristicNode[_numberOfChildren];



            for (int i = 0; i < _numberOfChildren; i++)

            {

                this.children[i] = null;

            }

        }



        override public string ToString()

        {

            if (this.children[0] == null)

                return this.name;



            string result = "";



            result += this.name + "(";



            for (int i = 0; i < this.children.Length; i++)

            {

                if (this.children[i] != null)

                    result += this.children[i].ToString();

                else

                    break;



                if (i < this.children.Length - 1)

                {

                    if (this.children[i + 1] != null)

                        result += ", ";

                }

            }



            return result + ")";

        }





    }



    class MainClass

    {

        public static string[] getParameters(string input)

        {

            string[] parameters = new string[3];

            for (int i = 0; i < parameters.Length; i++)

            {

                parameters[i] = "";

            }



            int indexOfOpenParenthesis = input.IndexOf('(');

            int indexOfCloseParenthesis = input.IndexOf(')');

            int indexOfComma = input.IndexOf(',');



            if (indexOfComma == -1)

            {

                parameters[0] = input;

            }

            else

            {

                if (indexOfComma < indexOfCloseParenthesis)

                    parameters[0] = input.Substring(0, indexOfCloseParenthesis + 1);

                else

                    parameters[0] = input.Substring(0, indexOfComma);



                if (parameters[0].Length < input.Length)

                {

                    indexOfComma = input.Substring(parameters[0].Length + 2, input.Length - parameters[0].Length - 2).IndexOf(',');



                    if (indexOfComma == -1)

                    {

                        parameters[1] = input.Substring(parameters[0].Length + 2, input.Length - parameters[0].Length - 2);

                    }

                    else

                    {

                        parameters[1] = input.Substring(parameters[0].Length + 2, indexOfComma);

                    }



                    if (parameters[0].Length + parameters[1].Length < input.Length - 2)

                    {

                        parameters[2] = input.Substring(parameters[0].Length + 2 + parameters[1].Length + 2, input.Length - parameters[1].Length - parameters[0].Length - 4);

                    }

                }

            }



            return parameters;

        }



        public static void parseStringToTree(string input, HeuristicNode node)

        {

            int indexOfFirstOpenParenthesis = input.IndexOf('(');

            int individual_length = indexOfFirstOpenParenthesis;



            if (indexOfFirstOpenParenthesis > -1)

            {

                node.name = input.Substring(0, individual_length);



                int indexOfLastCloseParenthesis = input.LastIndexOf(')');

                int substring_length = indexOfLastCloseParenthesis - indexOfFirstOpenParenthesis - 1;



                string substring = input.Substring(indexOfFirstOpenParenthesis + 1, substring_length);



                string[] temp = getParameters(substring);



                int i = 0;

                foreach (var word in temp)

                {

                    if (word != "")

                    {

                        node.children[i] = new HeuristicNode();

                        parseStringToTree(word, node.children[i]);

                        i++;

                    }

                }

            }

            else

            {

                node.name = input;

            }

        }



        public static void Main(string[] args)

        {

            HeuristicNode ht = new HeuristicNode();

            string toParse = "IfThenElse(Function3(ARG1, ARG2), 'none', IfThenElse(Function2(ARG1, 14, ARG2), 'summon', IfThenElse(Function1(ARG0), 'play', 'pass')))";

            HeuristicsCode hp = new HeuristicsCode();



            parseStringToTree(toParse, ht);

            Console.WriteLine(ht.ToString());

            Console.WriteLine(hp.Compile(ht, 9, 1, 1));

        }

    }

}
