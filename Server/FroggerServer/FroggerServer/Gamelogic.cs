using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;

namespace FroggerServer
{
    class GameLogic
    {
        //Add some type of datastructure to hold messages of the players. Map maybe?
        public Player first;
        public Player second;
        Player third;
        Player fourth;

        int[] positions = new int[8];

        private bool fourPlayer;

        private bool gameHasStarted;
        private bool gameIsOver;
        private bool timerStarted;

        private int playerCount = 0;

        //Scores being set
        public int playerOneScore;
        public int playerTwoScore;
        private bool winnerSet = false;
        public int winner = 0;
        bool firstScore = false;
        bool secondScore = false;
        public bool bothScoresSet;

        //In Lobby ready
        public bool p1Ready = false;
        public bool p2Ready = false;
        
        //In game loaded ready
        public bool p1Loaded = false;
        public bool p2Loaded = false;

        //Timer
        Stopwatch stopWatch = new Stopwatch();

        public GameLogic() 
        {
            gameHasStarted = false;
            gameIsOver = false;
            timerStarted = false;
            setDefaultPosition(1);
            setDefaultPosition(2);
            setDefaultPosition(3);
            setDefaultPosition(4);
        }

        public GameLogic(Player one, Player two, Player three, Player four) 
        {
            first = one;
            second = two;
            third = three;
            fourth = four;
            fourPlayer = true;
            setDefaultPosition(1);
            setDefaultPosition(2);
            setDefaultPosition(3);
            setDefaultPosition(4);
            gameHasStarted = false;
            gameIsOver = false;
            timerStarted = false;
        }

        public GameLogic(Player one, Player two)
        {
            first = one;
            second = two;
            fourPlayer = false;
            setDefaultPosition(1);
            setDefaultPosition(2);
            setDefaultPosition(3);
            setDefaultPosition(4);
            gameHasStarted = false;
            gameIsOver = false;
            timerStarted = false;
        }

        public void setTimer()
        {
            stopWatch.Reset();
            stopWatch.Start();
            //Reading elapsed time:
            //TimeSpan ts = stopWatch.Elapsed;
        }

        public int getCurrentTime()
        {
            TimeSpan time = stopWatch.Elapsed;
            return time.Seconds;
        }

        public bool playerIsInGame(string IP) 
        {
            try
            {
                if (first.IP.Equals(IP))
                    return true;
                else if (second.IP.Equals(IP))
                    return true;
                else if (third.IP.Equals(IP))
                    return true;
                else if (fourth.IP.Equals(IP))
                    return true;
            }
            catch (Exception e)
            {
                return false;
            }
            return false;
        }

        public void setPlayerPosition(string IP, int x, int y)
        {
            int position = getPlayerNumber(IP);
            if (x <= 7 && x >= -7 && y >= -8 && y <= 5)
            {
                if (position == 1)
                {
                    positions[0] = x;
                    positions[1] = y;
                }
                else if (position == 2)
                {
                    positions[2] = x;
                    positions[3] = y;
                }
                else if (position == 3)
                {
                    positions[4] = x;
                    positions[5] = y;
                }
                else if (position == 4)
                {
                    positions[6] = x;
                    positions[7] = y;
                }
            }
            else 
            {
                setDefaultPosition(position);
            }
        }

        private void setDefaultPosition(int playerNumber)
        {
            if (playerNumber == 1)
            {
                positions[0] = -3;
                positions[1] = -8;
            }
            else if (playerNumber == 2)
            {
                positions[2] = 3;
                positions[3] = -8;
            }
            else if (playerNumber == 3)
            {
                positions[4] = 0;
                positions[5] = -8;
            }
            else if (playerNumber == 4)
            {
                positions[6] = 0;
                positions[7] = -8;
            }
        }

        public Player getPlayer(int player)
        {
            if (player == 1)
                return first;
            else if (player == 2)
                return second;
            else if (player == 3)
                return third;
            else if (player == 4)
                return fourth;

            return null;
        }

        public string getPlayerPositions(int pos)
        {
            if (pos== 1)
                return positions[0] + "," + positions[1];
            else if (pos == 2)
                return positions[2] + "," + positions[3];
            else if (pos == 3)
                return positions[4] + "," + positions[5];
            else if (pos == 4)
                return positions[6] + "," + positions[7];

            return "";
        }

        public void setScore(string IP, string myScore)
        {
            if (first.IP.Equals(IP))
            {
                playerOneScore = int.Parse(myScore);
                firstScore = true;
            }
            else if (second.IP.Equals(IP))
            {
                playerTwoScore = int.Parse(myScore);
                secondScore = true;
            }

            if (!winnerSet && firstScore && secondScore) 
            {
                winnerSet = !winnerSet;
                bothScoresSet = true;

                if (playerOneScore > playerTwoScore)
                    winner = 1;
                else if (playerTwoScore > playerOneScore)
                    winner = 2;
                else
                    winner = 3;
            }
            //else if (third.IP.Equals(IP)) 
           // else if (fourth.IP.Equals(IP))
                
        }

        public void setReady(string IP) 
        {
            try
            {
                if (first.IP.Equals(IP))
                    p1Ready = true;
                else if (second.IP.Equals(IP))
                    p2Ready = true;
            }
            catch (Exception e)
            { 
                //Do nothing. Second player probably isn't connected. 
            }
        }

        public void setLoadReady(string IP)
        {
            try
            {
                if (first.IP.Equals(IP))
                    p1Loaded = true;
                else if (second.IP.Equals(IP))
                    p2Loaded = true;
            }
            catch (Exception e)
            {
                //Do nothing. Second player probably isn't connected. 
            }
        }

        public bool allPlayersLoaded()
        {
            return p2Loaded && p1Loaded;
        }

        public bool addPlayerToGame(Player toAdd) 
        {
            if (playerCount != 3)
            {
                if (playerCount == 0)
                    first = toAdd;
                else if (playerCount == 1)
                    second = toAdd;
                else if (playerCount == 2)
                    third = toAdd;
                else if (playerCount == 3)
                    fourth = toAdd;

                if (playerCount < 3)
                    playerCount++;
                
                return true;
            }
            else
                return false;

        }

        public int getPlayerNumber(string IP)
        {
            if (first.IP.Equals(IP))
                return 1;
            else if (second.IP.Equals(IP))
                return 2;
            else if (third.IP.Equals(IP))
                return 3;
            else if (fourth.IP.Equals(IP))
                return 4;

            return 0;
        }

        public void sendGameOver()
        {
            try
            {
                if (first != null)
                    NetworkHandler.Instance.sendMessage(first.IP, "messageGoesHere");

                if (second != null)
                    NetworkHandler.Instance.sendMessage(second.IP, "messageGoesHere");

                if (third != null)
                    NetworkHandler.Instance.sendMessage(third.IP, "messageGoesHere");

                if (fourth != null)
                    NetworkHandler.Instance.sendMessage(first.IP, "messageGoesHere");
            }
            catch(Exception e)
            { 
                //Do nothing because what can you do
            }

        }

        public bool isGameOver()
        {
            return gameIsOver;
        }

        public int getPlayerCount()
        {
            return playerCount;
        }

        public void update() 
        {
            if (gameHasStarted)
            {
                if (!timerStarted)
                {
                    setTimer();
                    timerStarted = !timerStarted;
                }
                else
                { 
                    long totalMilliseconds = stopWatch.ElapsedMilliseconds;
                    if (totalMilliseconds >= 60000)
                    {
                        gameIsOver = true;
                        gameHasStarted = !gameHasStarted;
                        sendGameOver();
                        timerStarted = !timerStarted;
                    }
                }
                    
            }
        }

    }
}
