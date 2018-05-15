using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks;
using SabberStoneCore.Tasks.PlayerTasks;
using heuristichearthstone.Meta;
using heuristichearthstone.Nodes;
using heuristichearthstone.Score;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
//using System.Data.DataSet;
//For Paladin
namespace heuristichearthstone
{
    internal class Program
    {//this is the heavily modified code by me, Aditya. see comments in the code for clarity. If any more questions, then contact me on the email thread.
        private static readonly Random Rnd = new Random();
        static Dictionary<string, int> cardname = new Dictionary<string, int>();
        //private static string locOfMaster = "D:\\GameInnovationLab\\SabberStone-master";//most important line, this indicates location of the project in your machine. edit here only, exactly as shown.
        private static int maxDepth = 13;//maxDepth = 10 and maxWidth = 500 is optimal 
        private static int maxWidth = 4;//keep maxDepth high(around 13) and maxWidth very low (4) for maximum speed
        private static Dictionary<string, int> cardStats = new Dictionary<string, int>(); //Store the amount of times each card is played PER GAME
        private static int parallelThreads = 1;// number of parallel running threads//not important
        private static int testsInEachThread = 1;//number of games in each thread//ae ere
                                                 //you are advised not to set more than 3 parallel threads if you are doing this on your laptop, otherwise the laptop will not survive
        private static int parallelThreadsInner = 1;//this his what is important
        private static int testsInEachThreadInner = 1;//linearly

        private static bool parallelOrNot = true;

        private static Stopwatch stopwatch2 = new Stopwatch();
        static string friendDeckClass = "";
        static string enemyDeckClass = "";
        //you are advised not to set more than 3 parallel threads if you are doing this on your laptop, otherwise the laptop will not survive
        private static void Main(string[] args)
        {
            //10ktests

            //Console.WriteLine(Cards.FromName("This is an invalid name") == Cards.FromName("Default"));-ignore this, this was for testing
            //  Console.WriteLine("Starting test setup. v7.6251test: Parallel=" + parallelOrNot + "in parallel " + parallelThreads + "x and in each parallel, no of tasks:" + testsInEachThread + " and inner parallel:" + parallelThreadsInner + " and each within inner parallel, inner tasks:" + testsInEachThreadInner + " times, different decks, get winrates and time avg of each and print max depth =" + maxDepth + " , max width = " + maxWidth + "");
            heuristichearthstone.CreateAndMutate createMutateObj = new CreateAndMutate();//this is the class I added which contains all functions1
                                                                               //this above object will help you mutate or create a deck, without worrying about underlying code.
            Dictionary<int, Dictionary<int, List<Card>>> victoryMany = new Dictionary<int, Dictionary<int, List<Card>>>();
            //  Dictionary<string, int> cardStats = new Dictionary<string, int>();
            Dictionary<int, float> winRates = new Dictionary<int, float>();
            //OneTurn();-ignore this
            Dictionary<int, string> allCards = getAllCards();//important function, must be done in main before anything else, this will get all the valid hearthstone cards (1300+ cards in total) from the data file
                                                             // string[] results = new string[100];//max 15 tests, can be increased/changed without any problems, make sure max j * max i <= size of results array
            Dictionary<int, string> results = new Dictionary<int, string>();
            Dictionary<int, List<Card>> resultsMutated = new Dictionary<int, List<Card>>();
            // List<int,List<Card>> mutations = new List<List<Card>>();
            //List<Card> playerDeck = createMutateObj.createRandomDeck(allCards, cardname);//this liine returns randomly created deck from all cards in hearthsone, which is passed as parameter 
            //List<Card> playerDeck = Decks.AggroPirateWarrior;
            bool end = false;
            // Decks.PaladinBasic;

            stopwatch2.Start();
            //DateTime start = new DateTime();
            //DateTime stop = new DateTime();
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = parallelThreads;
            //string folderName = Console.ReadLine();
            //int GPUID = 1;
            //int numGames = 10;

            int GPUID = int.Parse(args[0]) - 1;//1;//1;// -1 for Ms hoover0;// 0;//

            string folderName = args[1].ToString();//"22-02-2018-23-54-44";// "starter-0";/"starter-0";//
            int numGames = int.Parse(args[2]);//200;// 100;// 100;//100;//
            int deckIncrement = int.Parse(args[3]); //2;// 0;//  0;//

            int deckID = GPUID / numGames;
            int remainder = GPUID % numGames;
            // int gamesPerCPUs = int.Parse(args[4]);//25

            //sample: GamePlayer.exe array_ID generation-0 400 1

            // int numGames = 0;
            Console.WriteLine("here for Hunter Deck GPUID=" + GPUID + "numgames=" + numGames + "folderanme=" + folderName);
            Console.WriteLine("here GPUID=" + GPUID + "numgames=" + numGames + "folderanme=" + folderName);
            string path = folderName + "/Decks.txt";
            int level = int.Parse(folderName.Split('-')[1]);
            enemyDeckClass = getClassFromFile(folderName + "/EnemyDeck.txt");
            friendDeckClass = getClassFromFile(folderName + "/FriendDeck.txt");
            List<Card> playerDeck2 = getDeckFromFile(folderName + "/EnemyDeck.txt");
            while (true)
            {

                while (Directory.Exists(folderName))
                {

                    Console.WriteLine(" this code is for 10k to run on  HPC count of results=" + results.Count);
                    Console.WriteLine("Found " + folderName);
                    Thread.Sleep(10000);
                    path = folderName + "/Decks.txt";
                    Dictionary<int, List<Card>> nDecks = getDecksFromFile(path);

                    int j = 0;
                    int currDeckID = deckID;
                    while (j < nDecks.Count)
                    {

                        List<Card> playerDeck = nDecks[currDeckID];
                        Console.WriteLine("Enemy deck:-size=" + playerDeck2.Count);
                        createMutateObj.print(playerDeck2);
                        Console.WriteLine("Friend deck:- size=" + playerDeck.Count);
                        createMutateObj.print(playerDeck);
                        string gameLogAddr = folderName + "/Deck" + currDeckID;

                        Console.WriteLine("currently on deck =" + currDeckID);
                        gameLogAddr += "/" + GPUID + "-" + j + ".txt";

                        createMutateObj.printToFile(playerDeck, gameLogAddr);//printed once in begining
                        Stopwatch stopwatch = new Stopwatch();
                        if (!results.ContainsKey(j))
                        {
                            string winRate_timeMean = "";
                            stopwatch.Start();
                            bool retry = true;
                            int tries = 0;
                            while (retry)
                            {
                                try
                                {
                                    var thread = new Thread(() =>
                                    {
                                        winRate_timeMean = "GPUID:" + GPUID + "-" + j + " Game Deck:" + currDeckID + ": " + getWinRateTimeMean(playerDeck, j, playerDeck2, gameLogAddr);
                                    });
                                    thread.Start();
                                    bool finished = thread.Join(600000);
                                    if (!finished)
                                    {
                                        //thread.Abort();
                                        retry = true;
                                        Console.WriteLine("had to continue here for deck=" + currDeckID);
                                        //  winRateLater = float.Parse(winRate_timeMean.Split('%')[0].Split('=')[1]);
                                        tries++;
                                        continue;
                                    }
                                    else
                                    {
                                        retry = false;

                                    }

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                if (tries > 3)
                                {
                                    break;
                                }
                            }
                            //FullGame(playerDeck, i, playerDeck2);//
                            Console.WriteLine("here after game is played GPUID=" + GPUID + "-" + j);

                            stopwatch.Stop();

                            //  winRateLater = float.Parse(winRate_timeMean.Split('%')[0].Split('=')[1]);
                            long seconds = (stopwatch.ElapsedMilliseconds / 1000);//(stop - start).ToString();//
                            TimeSpan t = TimeSpan.FromSeconds(seconds);

                            if (!results.ContainsKey(j))
                            {
                                results.Add(j, winRate_timeMean + "Time taken:" + t.ToString());
                                // winRates.Add(j, winRateLater);
                            }

                            // Console.WriteLine(results[j]);
                            string cardStatisticsForPrint = produceCardStatsString();

                            stopwatch.Reset();
                            Console.WriteLine("here just before file print GPUID=" + GPUID + "-" + j + "numgames=" + numGames + "folderanme=" + folderName);
                            string overallGameStat = folderName + "/Overall/Deck" + currDeckID;
                            if (!Directory.Exists(overallGameStat))
                            {
                                Directory.CreateDirectory(overallGameStat);
                            }
                            try
                            {
                                path = overallGameStat + "/" + GPUID + "-" + j + ".txt";
                                //createMutateObj.print(playerDeck);
                                createMutateObj.printToFile(playerDeck, path);
                                using (StreamWriter tw = File.AppendText(path))
                                {

                                    tw.WriteLine(results[j]);
                                    tw.WriteLine(cardStatisticsForPrint + "/n");

                                    //print card stats here.
                                    tw.Close();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            foreach (int key in results.Keys)
                            {
                                // Console.WriteLine("Game " + key + " : " + results[key] + "\n");
                                if (resultsMutated.ContainsKey(key))
                                {
                                    //  createMutateObj.print(resultsMutated[key]);
                                }
                            }
                            j++;
                            currDeckID += deckIncrement;
                            GC.Collect();
                            cardStats.Clear();
                            long memory = GC.GetTotalMemory(false);
                            Console.WriteLine("Memory usage here:" + memory);

                        }



                    }
                    level++;
                    folderName = folderName.Split('-')[0] + "-" + level.ToString();
                    results.Clear();
                    nDecks.Clear();
                    Console.WriteLine("count of nDecks=" + nDecks.Count);
                    Console.WriteLine("trying to find " + folderName);
                }
            }
            foreach (int key in results.Keys)
            {
                Console.WriteLine("Game " + key + " : " + results[key] + "\n");
                if (resultsMutated.ContainsKey(key))
                {
                    //createMutateObj.print(resultsMutated[key]);
                }
                stopwatch2.Stop();
                TimeSpan tempeForOverall = TimeSpan.FromSeconds(stopwatch2.ElapsedMilliseconds / 1000);
                Console.WriteLine("Overall time taken:" + tempeForOverall.ToString());
            }

        }

        public static Dictionary<int, List<Card>> getDecksFromFile(string path)
        {
            Dictionary<int, List<Card>> nDecks = new Dictionary<int, List<Card>>();

            string[] textLines = System.IO.File.ReadAllLines(path);
            //int arg = 3;
            int currDeckInd = 0;
            Console.WriteLine("lines size=" + textLines.Length);
            while (currDeckInd <= (textLines.Length - 1))
            {
                if (textLines[currDeckInd].Length < 3)
                {
                    Console.WriteLine("textLines length was less than 3");
                    break;
                }
                if (!nDecks.ContainsKey(currDeckInd))
                {
                    List<Card> currDeck = new List<Card>();
                    string[] cards = textLines[currDeckInd].Split('*');
                    for (int i = 1; i < 31; i++)
                    {
                        currDeck.Add(Cards.FromName(cards[i]));
                    }
                    // Console.WriteLine("currDeck=" + currDeckInd + " size of deck=" + currDeck.Count);
                    nDecks.Add(currDeckInd, currDeck);
                }
                currDeckInd++;
            }


            return nDecks;
        }

        //create
        /*
		public static Dictionary<int, int> createDeck(Dictionary<int, string> allcards)
		{
			Dictionary<int, int> chosenDeck = new Dictionary<int, int>();
			Random rand = new Random();
			int Count = 0;
			while (Count != 30)
			{
				int oneRand = rand.Next(0, allcards.Count + 1);
				if (allcards.ContainsKey(oneRand)) //&& (Cards.FromName(allcards[oneRand])))
				{
					if (chosenDeck.ContainsKey(oneRand) && (chosenDeck[oneRand] == 1))
					{
						chosenDeck[oneRand] = 2;
						Count++;
					}
					else if (!chosenDeck.ContainsKey(oneRand))
					{
						chosenDeck.Add(oneRand, 1);
						Count++;
					}
				}
			}
			//List<Card> playerDeck = convertDictToList(chosenDeck, allcards);
			Console.WriteLine("Count:" + Count);
			return chosenDeck;
		}
		*/
        //build allcards in dictionary from txt file


        public static string getWinRateTimeMean(List<Card> player1Deck, int where, List<Card> player2Deck, string gameLogAddr)
        {

            int[] wins = Enumerable.Repeat(0, 1000).ToArray();
            long sum_Timetaken = 0;
            int winss = 0;
            object[] temp()
            {
                object[] obj = new object[1002];
                for (int i = 0; i < parallelThreadsInner * testsInEachThreadInner; i++)
                {
                    obj[i] = new Stopwatch();
                }
                return obj;
            }
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 8;// parallelThreads;// Environment.ProcessorCount;//parallelThreadsInner+10;
                                                       // Console.WriteLine(Environment.ProcessorCount);
            object[] stopwatches = temp();
            string res = "";
            Parallel.For(0, parallelThreadsInner * testsInEachThreadInner, parallelOptions, j =>
            {
                // int i = 0;
                // long max = 0;
                // while (!end)

                //for (int i = testsInEachThreadInner * j; i < (j + 1) * testsInEachThreadInner; i++)//(int i = 0; i < 10 ; i++) //

                int i = j;
                //Console.WriteLine("Environment:" + Environment.ProcessorCount);
                // Console.WriteLine("Inner i, or here inside getWinRateTimeMean at here= " + i);
                ((Stopwatch)stopwatches[i]).Start();
                // start = DateTime.Now;
                string s = "";
                bool retry = true;
                while (retry)
                {
                    try
                    {
                        s = FullGame(player1Deck, i, player2Deck, gameLogAddr);
                        Console.WriteLine(s);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        s = e.Message.ToString();
                        //CreateAndMutate createAndMutate = new CreateAndMutate();
                        //   Console.WriteLine("Player 1 deck that caused issue:");
                        // createAndMutate.print(player1Deck);
                    }
                    //stop = DateTime.Now;
                    if (s.ToLower().Contains("present") || s.ToLower().Contains("instance") || s.ToLower().Contains("zone"))
                    {
                        Console.WriteLine("this was s=" + s + "retrying right here");

                        retry = true;
                    }
                    else
                    {
                        retry = false;
                    }
                }
                ((Stopwatch)stopwatches[i]).Stop();
                long seconds = (((Stopwatch)stopwatches[i]).ElapsedMilliseconds / 1000);//(stop - start).ToString();//
                                                                                        // Console.WriteLine("secondes:" + seconds);
                                                                                        //  Console.Write("Seconds: " + seconds);
                                                                                        // TimeSpan tempe = TimeSpan.FromSeconds(seconds);
                                                                                        // Console.WriteLine("time taken for "+ i +":" + tempe);
                sum_Timetaken = sum_Timetaken + seconds;
                // Console.WriteLine("sum_TimeTaken in loop:" + sum_Timetaken);
                //((Stopwatch)stopwatches[i]).Reset();


                if (s.Contains("Player1: WON"))
                {
                    wins[i]++;
                    // winss++;
                    // Console.WriteLine("Winss:" + winss);
                }

                // Console.WriteLine("Max was:" + max);
                // max = 0;
                res = s;
            });
            // Console.WriteLine("Starting test setup. v6.7: Not running in Parallel at ALL run in parallel " + parallelThreads + "x and in each parallel, no of tasks:" + testsInEachThread + " and inner parallel:" + parallelThreadsInner + " and each within inner parallel, inner tasks:" + testsInEachThreadInner + " times, different decks, get winrates and time avg of each and print max depth =" + maxDepth + " , max width = " + maxWidth + "");
            //Console.WriteLine("New sum_timetaken=" + sum_Timetaken);
            TimeSpan t = TimeSpan.FromSeconds(sum_Timetaken / (parallelThreadsInner * testsInEachThreadInner));

            //return "Win rate =" + winrateDiv * 100 + "% and average time of each round (hh:mm:ss) = " + t.ToString();
            return res + " and average time of game (hh:mm:ss) = " + t.ToString();
        }

        public static string getWinRateTimeMeanLinear(List<Card> player1Deck, int where, List<Card> player2Deck)
        {
            int[] wins = Enumerable.Repeat(0, 1000).ToArray();
            long sum_Timetaken = 0;
            int winss = 0;
            int numOfTests = 70;
            object[] temp()
            {
                object[] obj = new object[1002];
                for (int i = 0; i < numOfTests; i++)
                {
                    obj[i] = new Stopwatch();
                }
                return obj;
            }

            object[] stopwatches = temp();
            //Parallel.For(0, parallelThreadsInner, parallelOptions, j =>
            {
                // int i = 0;
                long max = 0;
                // while (!end)
                for (int i = 0; i < numOfTests; i++) ////
                {
                    Console.WriteLine("Inner i, or here inside getWinRateTimeMean at here= " + i);
                    ((Stopwatch)stopwatches[i]).Start();
                    // start = DateTime.Now;
                    string s = "";
                    try
                    {
                        //s = FullGame(player1Deck, i, player2Deck);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        CreateAndMutate createAndMutate = new CreateAndMutate();
                        Console.WriteLine("Player 1 deck that caused issue:");
                        createAndMutate.print(player1Deck);
                    }
                    //stop = DateTime.Now;
                    ((Stopwatch)stopwatches[i]).Stop();
                    long seconds = (((Stopwatch)stopwatches[i]).ElapsedMilliseconds / 1000);//(stop - start).ToString();//
                    Console.WriteLine("secondes:" + seconds);
                    if (max < seconds)
                    {
                        max = seconds;
                    }
                    TimeSpan tempe = TimeSpan.FromSeconds(seconds);
                    Console.WriteLine("time taken for " + i + ":" + tempe);
                    sum_Timetaken = sum_Timetaken + seconds;
                    Console.WriteLine("sum_TimeTaken in loop:" + sum_Timetaken);
                    //((Stopwatch)stopwatches[i]).Reset();
                    if (s.Contains("Player1: WON"))
                    {
                        wins[i]++;
                        winss++;
                        Console.WriteLine("Winss:" + winss);
                    }
                }
                Console.WriteLine("Max was:" + max);
                max = 0;
            }//);
            Console.WriteLine("Starting test setup. v6.7: all linear, 10 and in each 50 linear all randomly gen decks Not running in Parallel at ALL, this is in Linear Method run in parallel " + parallelThreads + "x and in each parallel, no of tasks:" + testsInEachThread + " and inner parallel:" + parallelThreadsInner + " and each within inner parallel, inner tasks:" + testsInEachThreadInner + " times, different decks, get winrates and time avg of each and print max depth =" + maxDepth + " , max width = " + maxWidth + "");

            for (int i = 0; i < (numOfTests); i++)
                Console.WriteLine("i:" + i + " wins:" + wins[i]);
            sum_Timetaken = 0;
            for (int i = 0; i < (numOfTests); i++)
            {
                Console.WriteLine("i:" + i + " Times:" + ((Stopwatch)stopwatches[i]).ElapsedMilliseconds / 1000);
                sum_Timetaken = sum_Timetaken + ((Stopwatch)stopwatches[i]).ElapsedMilliseconds / 1000;
            }
            Console.WriteLine("New sum_timetaken=" + sum_Timetaken);
            TimeSpan t = TimeSpan.FromSeconds(sum_Timetaken / (numOfTests));
            float winsSum = 0;
            for (int i = 0; i < (numOfTests); i++)
                if (wins[i] > 0)
                    winsSum++;
            //winsSum = winss;
            Console.WriteLine(winsSum + "is winsSum");
            Console.WriteLine(winss + "is winss");
            float winrateDiv = (float)((float)winsSum / ((float)numOfTests));
            return "Win rate =" + winrateDiv * 100 + "% and average time of each round (hh:mm:ss) = " + t.ToString();
        }

        public static Dictionary<int, string> getAllCards()
        {
            Dictionary<int, string> allcards = new Dictionary<int, string>();

            string fileName = "CardDefs.xml";
            if (System.IO.File.Exists(fileName))
            {
                int i = 1;
                XDocument doc = XDocument.Load(fileName);
                var authors = doc.Descendants("Entity").Descendants("Tag").Where(x => x.Attribute("name").Value == "CARDNAME").Elements("enUS");
                // byte[] byteArray = Encoding.UTF8.GetBytes("D:\\GameInnovationLab\\xmldata test.txt");
                //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                //MemoryStream stream = new MemoryStream(byteArray);
                //System.IO.StreamWriter file = new System.IO.StreamWriter(stream);
                foreach (var author in authors)
                {

                    if (!cardname.ContainsKey(author.Value) && (Cards.FromName(author.Value) != Cards.FromName("Default"))
                        && Cards.FromName(author.Value).Implemented &&
                        (Cards.FromName(author.Value).Set == CardSet.CORE)
                        && Cards.FromName(author.Value).Collectible && (Cards.FromName(author.Value).Type != CardType.HERO) &&
                        (Cards.FromName(author.Value).Type != CardType.ENCHANTMENT) && (Cards.FromName(author.Value).Type != CardType.INVALID)
                        && (Cards.FromName(author.Value).Type != CardType.HERO_POWER) && (Cards.FromName(author.Value).Type != CardType.TOKEN))
                    {
                        if (checkClass(author.Value))
                        {
                            //  Console.WriteLine(Cards.FromName(author.Value).Class);
                            // Console.WriteLine(Cards.FromName(author.Value).Set);
                            cardname.Add(author.Value, i);
                            allcards.Add(i, author.Value);
                            i++;
                            //Console.WriteLine(author.Value);
                            // Console.WriteLine(allcards.Count);
                        }
                    }

                }
                /*XmlReader xmlReader = XmlReader.Create(fileName);
			while (xmlReader.Read())
			{
				if ( (xmlReader.Name == "CARDNAME"))
				{
					if (xmlReader.HasAttributes)
					{

						allcards.Add(i, xmlReader.GetAttribute("enUS"));
						i++;
					}
				}
			}*/
                //D:\\GameInnovationLab\\xmldata test.txt


            }
            else
            {
                Console.WriteLine("File not found");
            }
            return allcards;
        }

        public static bool checkClass(string nameOfCard)
        {
            //|| Cards.FromName(author.Value).Class == CardClass.PALADIN
            return true;

        }


        public static void RandomGames()
        {
            const int total = 100;
            var watch = Stopwatch.StartNew();

            int turns = 0;
            int[] wins = new[] { 0, 0 };
            for (int i = 0; i < total; i++)
            {
                var game = new Game(new GameConfig
                {
                    StartPlayer = 1,
                    Player1HeroClass = CardClass.MAGE,
                    Player2HeroClass = CardClass.MAGE,
                    FillDecks = true,
                    Logging = false
                });
                game.StartGame();

                while (game.State != State.COMPLETE)
                {
                    List<PlayerTask> options = game.CurrentPlayer.Options();
                    PlayerTask option = options[Rnd.Next(options.Count)];
                    //Console.WriteLine(option.FullPrint());
                    game.Process(option);


                }
                turns += game.Turn;
                if (game.Player1.PlayState == PlayState.WON)
                    wins[0]++;
                if (game.Player2.PlayState == PlayState.WON)
                    wins[1]++;

            }
            watch.Stop();

            Console.WriteLine($"{total} games with {turns} turns took {watch.ElapsedMilliseconds} ms => " +
                              $"Avg. {watch.ElapsedMilliseconds / total} per game " +
                              $"and {watch.ElapsedMilliseconds / (total * turns)} per turn!");
            Console.WriteLine($"playerA {wins[0] * 100 / total}% vs. playerB {wins[1] * 100 / total}%!");
        }

        public static void OneTurn()
        {
            var game = new Game(
                new GameConfig()
                {
                    StartPlayer = 1,
                    Player1Name = "FitzVonGerald",
                    Player1HeroClass = CardClass.WARRIOR,
                    Player1Deck = Decks.AggroPirateWarrior,
                    Player2Name = "RehHausZuckFuchs",
                    Player2HeroClass = CardClass.SHAMAN,
                    Player2Deck = Decks.MidrangeJadeShaman,
                    FillDecks = false,
                    Shuffle = false,
                    SkipMulligan = false
                });
            game.Player1.BaseMana = 10;
            game.StartGame();

            var aiPlayer1 = new AggroScore();
            var aiPlayer2 = new AggroScore();

            game.Process(ChooseTask.Mulligan(game.Player1, aiPlayer1.MulliganRule().Invoke(game.Player1.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList())));
            game.Process(ChooseTask.Mulligan(game.Player2, aiPlayer2.MulliganRule().Invoke(game.Player2.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList())));

            game.MainReady();

            while (game.CurrentPlayer == game.Player1)
            {
                Console.WriteLine($"* Calculating solutions *** Player 1 ***");

                List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player1.Id, aiPlayer1, 10, 500);

                var solution = new List<PlayerTask>();
                solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
                Console.WriteLine($"- Player 1 - <{game.CurrentPlayer.Name}> ---------------------------");

                foreach (PlayerTask task in solution)
                {
                    Console.WriteLine(task.FullPrint());
                    game.Process(task);
                    if (game.CurrentPlayer.Choice != null)
                        break;
                }
            }

            Console.WriteLine(game.Player1.HandZone.FullPrint());
            Console.WriteLine(game.Player1.BoardZone.FullPrint());
        }


        public static List<Card> getDeckFromFile(string path)
        {
            List<Card> opponent = new List<Card>();
            string[] textLines = System.IO.File.ReadAllLines(path);
            Console.WriteLine("lines size=" + textLines.Length + "in path=" + path);
            string[] cards = textLines[1].Split('*');
            for (int i = 0; i < 30; i++)
            {
                opponent.Add(Cards.FromName(cards[i]));
            }
            return opponent;
        }

        public static string getClassFromFile(string path)
        {
            List<Card> opponent = new List<Card>();
            string[] textLines = System.IO.File.ReadAllLines(path);
            Console.WriteLine("lines size=" + textLines.Length + "in path=" + path);
            string Class = textLines[0].Trim().ToLower();
            return Class;
        }

        public static Game getGame(List<Card> player1Deck, List<Card> player2Deck)
        {
            var friendClass = CardClass.HUNTER;
            var enemyClass = CardClass.HUNTER;
            if (friendDeckClass.ToLower().StartsWith("paladin"))
            { friendClass = CardClass.PALADIN; Console.WriteLine("friend(player1) is paladin"); }
            else if (friendDeckClass.ToLower().StartsWith("hunter"))
            { friendClass = CardClass.HUNTER; Console.WriteLine("friend(player1) is hunter"); }
            else if (friendDeckClass.ToLower().StartsWith("warlock"))
            { friendClass = CardClass.WARLOCK; Console.WriteLine("friend(player1) is warlock"); }
            else if (friendDeckClass.ToLower().StartsWith("shaman"))
            { friendClass = CardClass.SHAMAN; Console.WriteLine("friend(player1) is shaman"); }
            else if (friendDeckClass.ToLower().StartsWith("druid"))
            { friendClass = CardClass.DRUID; Console.WriteLine("friend(player1) is druid"); }
            else if (friendDeckClass.ToLower().StartsWith("mage"))
            { friendClass = CardClass.MAGE; Console.WriteLine("friend(player1) is mage"); }
            else if (friendDeckClass.ToLower().StartsWith("priest"))
            { friendClass = CardClass.PRIEST; Console.WriteLine("friend(player1) is priest"); }
            else if (friendDeckClass.ToLower().StartsWith("rogue"))
            { friendClass = CardClass.ROGUE; Console.WriteLine("friend(player1) is rogue"); }
            else if (friendDeckClass.ToLower().StartsWith("warrior"))
            { friendClass = CardClass.WARRIOR; Console.WriteLine("friend(player1) is warrior"); }

            if (enemyDeckClass.ToLower().StartsWith("paladin"))
            { enemyClass = CardClass.PALADIN; Console.WriteLine("enemy(player2) is paladin"); }
            else if (enemyDeckClass.ToLower().StartsWith("hunter"))
            { enemyClass = CardClass.HUNTER; Console.WriteLine("enemy(player2) is hunter"); }
            else if (enemyDeckClass.ToLower().StartsWith("warlock"))
            { enemyClass = CardClass.WARLOCK; Console.WriteLine("enemy(player2) is warlock"); }
            else if (enemyDeckClass.ToLower().StartsWith("shaman"))
            { enemyClass = CardClass.SHAMAN; Console.WriteLine("enemy(player2) is shaman"); }
            else if (enemyDeckClass.ToLower().StartsWith("druid"))
            { enemyClass = CardClass.DRUID; Console.WriteLine("enemy(player2) is druid"); }
            else if (enemyDeckClass.ToLower().StartsWith("mage"))
            { enemyClass = CardClass.MAGE; Console.WriteLine("enemy(player2) is mage"); }
            else if (enemyDeckClass.ToLower().StartsWith("priest"))
            { enemyClass = CardClass.PRIEST; Console.WriteLine("enemy(player2) is priest"); }
            else if (enemyDeckClass.ToLower().StartsWith("rogue"))
            { enemyClass = CardClass.ROGUE; Console.WriteLine("enemy(player2) is rogue"); }
            else if (enemyDeckClass.ToLower().StartsWith("warrior"))
            { enemyClass = CardClass.WARRIOR; Console.WriteLine("enemy(player2) is warrior"); }


            var game = new Game(
            new GameConfig()
            {
                StartPlayer = -1,
                Player1Name = "FitzVonGeraldPaladin",
                Player1HeroClass = friendClass,
                Player1Deck = player1Deck,
                Player2Name = "RehHausZuckFuchsPaladin",
                Player2HeroClass = enemyClass,
                Player2Deck = player2Deck,
                FillDecks = false,
                Shuffle = true,
                SkipMulligan = false
            });
            return game;
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
                foreach(SabberStoneCore.Model.Entities.IPlayable minion in minionList)
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
        //the game we need
        public static string FullGame(List<Card> player1Deck, int where, List<Card> player2Deck, string gameLogAddr)
        {
            string logsbuild = "";
            var game = getGame(player1Deck, player2Deck);
            Console.WriteLine("game heroes:" + game.Heroes[0].ToString() + " and " + game.Heroes[1].ToString());
            game.StartGame();
            string startPlayer = game.CurrentPlayer.Name;
            var aiPlayer1 = new AggroScore();
            var aiPlayer2 = new AggroScore();

            List<int> mulligan1 = aiPlayer1.MulliganRule().Invoke(game.Player1.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList());
            List<int> mulligan2 = aiPlayer2.MulliganRule().Invoke(game.Player2.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList());
            logsbuild += $"Player1: Mulligan {string.Join(",", mulligan1)}";
            logsbuild += "\n";
            logsbuild += $"Player2: Mulligan {string.Join(",", mulligan2)}";
            logsbuild += "\n";
            // Console.WriteLine($"Player1: Mulligan {string.Join(",", mulligan1)}");
            //Console.WriteLine($"Player2: Mulligan {string.Join(",", mulligan2)}");

            game.Process(ChooseTask.Mulligan(game.Player1, mulligan1));
            game.Process(ChooseTask.Mulligan(game.Player2, mulligan2));

            game.MainReady();

            while (game.State != State.COMPLETE)
            {
               
                while (game.State == State.RUNNING && game.CurrentPlayer == game.Player1)
                {
                    var allCardsPossible = game.Player1.HandZone.GetAll.Where(p => p.IsPlayableByPlayer && p.IsPlayableByCardReq).ToList();
                    var result = new List<PlayerTask>();
                    var forward_gameStates = new List<Game>();
                    var controller = game.Player1;
                    foreach (SabberStoneCore.Model.Entities.IPlayable card in allCardsPossible)
                    {
                        IEnumerable<SabberStoneCore.Model.Entities.ICharacter> targets = card.ValidPlayTargets;
                        if (!targets.Any())
                        {
                            result.Add(PlayCardTask.Any(controller, card, null, -1,
                                card.ChooseOne == false ? 0 : allCardsPossible.IndexOf(card) + 1));//allCardsPossible.Count == 1 ? 0 : allCardsPossible.IndexOf(card) + 1));
                        }
                        else
                        {
                            foreach (SabberStoneCore.Model.Entities.ICharacter target in targets)
                            {
                                result.Add(PlayCardTask.Any(controller, card, target, -1,
                                    card.ChooseOne == false ? 0 : allCardsPossible.IndexOf(card) + 1));//allCardsPossible.Count == 1 ? 0 : allCardsPossible.IndexOf(card) + 1));
                            }
                        }

                    }
                    if (game.Player1.Hero.Power.IsPlayable)
                    {
                        IEnumerable<SabberStoneCore.Model.Entities.ICharacter> targets = game.Player1.Hero.Power.ValidPlayTargets;
                        if (targets.Any())
                        {
                            foreach (SabberStoneCore.Model.Entities.ICharacter target in targets)
                                result.Add(HeroPowerTask.Any(controller, target));
                        }
                        else
                        {
                            result.Add(HeroPowerTask.Any(controller));
                        }
                    }


                    for (int i = 0; i < result.Count; i++)
                    {
                        forward_gameStates.Add(game.Clone());
                        forward_gameStates[i].Process(result[i]);
                    }
                    forward_gameStates.Add(game.Clone()); // Index of result.Count + 1 means action is to Do Nothing.
                    List<Game> checkForwardGameStatesGotten = chooseMaxSumAttackMinionPlayerFromForward_GameStates(forward_gameStates);
                    if (result.Count > 0)
                    {
                        game.Process(result[0]);
                    }
                    logsbuild += $"* Calculating solutions *** Player 1 ***" + "\n";
                    //  Console.WriteLine($"* Calculating solutions *** Player 1 ***");//player 1's turn
                    List<OptionNode> solutions = OptionNode.GetSolutionsOurPLayer(game, game.Player1.Id, aiPlayer1, maxDepth, maxWidth);
                    var solution = new List<PlayerTask>();
                    solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
                    //Console.WriteLine($"- Player 1 - <{game.CurrentPlayer.Name}> ---------------------------");
                    logsbuild += $"- Player 1 - <{game.CurrentPlayer.Name}> ---------------------------" + "\n";
                    foreach (PlayerTask task in solution)
                    {
                        logsbuild += task.FullPrint() + "\n";
                        //  Console.WriteLine(task.FullPrint());//important focus point for you. test this by first uncommenting it
                        string printedTask = task.FullPrint();//CONNOR CODE
                        if (printedTask.IndexOf("play") != -1)//CONNOR CODE
                        {//CONNOR CODE
                            string card = task.Source.ToString();//CONNOR CODE
                            //Console.WriteLine("Play: " + printedTask);      //ADDITION: if the tast is PlayCardTask, this code runs
                            //Console.WriteLine("Card is: " + card);//CONNOR CODE
                            CalculateFreqs(card);//CONNOR CODE
                            //CalculateTotalFreqs(card, cardStatistics);//CONNOR CODE
                        }//CONNOR CODE
                        else//CONNOR CODE
                        {//CONNOR CODE
                         // Console.WriteLine("Else: " + printedTask);//CONNOR CODE
                        }//CONNOR CODE
                        game.Process(task);
                        if (game.CurrentPlayer.Choice != null)
                        {
                            logsbuild += $"* Recaclulating due to a final solution ..." + "\n";
                            //    Console.WriteLine($"* Recaclulating due to a final solution ...");
                            break;
                        }
                    }
                }//hello hell0 hello is there anybody in there? Now that you can hear it
                 // GC.Collect();
                 // Random mode for Player 2
                 // Console.WriteLine($"- Player 2 - <{game.CurrentPlayer.Name}> ---------------------------");//player 2's turn
                logsbuild += $"- Player 2 - <{game.CurrentPlayer.Name}> ---------------------------" + "\n";
                while (game.State == State.RUNNING && game.CurrentPlayer == game.Player2)
                {
                    //var options = game.Options(game.CurrentPlayer);
                    //var option = options[Rnd.Next(options.Count)];
                    //Log.Info($"[{option.FullPrint()}]");
                    //game.Process(option);
                    //   Console.WriteLine($"* Calculating solutions *** Player 2 ***");
                    logsbuild += $"* Calculating solutions *** Player 2 ***" + "\n";
                    List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player2.Id, aiPlayer2, maxDepth, maxWidth);
                    var solution = new List<PlayerTask>();
                    solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
                    // Console.WriteLine($"- Player 2 - <{game.CurrentPlayer.Name}> ---------------------------");
                    logsbuild += $"- Player 2 - <{game.CurrentPlayer.Name}> ---------------------------" + "\n";
                    foreach (PlayerTask task in solution)
                    {
                        //   Console.WriteLine(task.FullPrint());//this is what you neeed to focus on right here
                        logsbuild += task.FullPrint() + "\n";
                        game.Process(task);
                        if (game.CurrentPlayer.Choice != null)
                        {
                            //     Console.WriteLine($"* Recaclulating due to a final solution ...");
                            logsbuild += $"* Recaclulating due to a final solution ..." + "\n";
                            break;
                        }
                    }
                }
                GC.Collect();
            }
            //Console.WriteLine($"Game: {game.State}, Player1: {game.Player1.PlayState} / Player2: {game.Player2.PlayState}");
            int healthdiff = game.Player1.Hero.Health - game.Player2.Hero.Health;
            logsbuild += "Game: {game.State}, Player1: " + game.Player1.PlayState + " / Player2:" + game.Player2.PlayState + "healthdiff:" + healthdiff + "& turns:" + game.Turn;
            /*using (StreamWriter tw = File.AppendText(gameLogAddr))
            {
                tw.WriteLine(logsbuild);
                tw.Close();
            }*/
            return "start player=" + startPlayer + ", Game: {game.State}, Player1: " + game.Player1.PlayState + " / Player2:" + game.Player2.PlayState + "healthdiff:" + healthdiff + "& turns:" + game.Turn;
        }
        static Dictionary<int, Dictionary<string, int>> calcuated = new Dictionary<int, Dictionary<string, int>>();//global
        static void calcukateCardFreq(string printed, int idgame, Dictionary<string, int> cardnames)
        {

            string namecard = "";
            //int count = 0;
            if (cardnames.ContainsKey(namecard))
            {
                calcuated[idgame].Add(namecard, 0);
            }
        }
        public static void CalculateFreqs(string thisCard)
        {
            String[] cardDetails = thisCard.Split('[');
            string cardNeeded = cardDetails[0].Remove(0, 1);
            cardNeeded = "[" + cardNeeded + "]";
            if (cardStats.ContainsKey(cardNeeded))
            {
                cardStats[cardNeeded] += 1;
                //Console.WriteLine("Updated frequency for: " + cardNeeded + " to: " + cardStats[cardNeeded].ToString());
            }
            else
            {
                cardStats.Add(cardNeeded, 1);
                //Console.WriteLine("Added Card: " + cardNeeded + " with frequency 1");
            }
        }

        public static string produceCardStatsString()
        {
            string build = "";
            var ordered = cardStats.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (string key in cardStats.Keys)
            {
                build += key + "*" + ordered[key] + "**";
            }

            return build;
        }

    }
}

