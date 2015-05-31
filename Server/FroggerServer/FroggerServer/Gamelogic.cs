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
        private string SessionName = "";
        public Player first;
        public Player second;
        Player third;
        Player fourth;

        float[] positions = new float[8];

        private bool fourPlayer;

        public bool gameHasStarted;
        private bool gameIsOver = false;
        private bool timerStarted;

        private int playerCount = 0;

        //Scores being set
        public bool scoresSent = false;
        public int playerOneScore;
        public int playerTwoScore;
        public int playerThreeScore;
        public int playerFourScore;
        private bool winnerSet = false;
        public int winner = 0;
        bool firstScore = false;
        bool secondScore = false;
        bool thirdScore = false;
        bool fourthScore = false;
        public bool bothScoresSet;

        //In Lobby ready
        public bool p1Ready = false;
        public bool p2Ready = false;
        public bool p3Ready = false;
        public bool p4Ready = false;
        
        //In game loaded ready
        public bool p1Loaded = false;
        public bool p2Loaded = false;
        public bool p3Loaded = false;
        public bool p4Loaded = false;

        //Timer
        Stopwatch stopWatch = new Stopwatch();

        public GameLogic() 
        {
            gameHasStarted = false;
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
            timerStarted = false;
        }

        public void setSessionName(string mySession)
        {
            SessionName = mySession;
        }

        public string getSessionName()
        {
            return SessionName;
        }

        public void setTimer()
        {
            stopWatch.Reset();
            stopWatch.Start();
        }

        public void handleDissconnectedPlayer(string IP)
        {
            try
            {
                if (first.IP.Equals(IP))
                    playerOneScore = -1;
                else if (second.IP.Equals(IP))
                    playerTwoScore = -1;
                else if (third.IP.Equals(IP))
                    playerThreeScore = -1;
                else if (fourth.IP.Equals(IP))
                    playerFourScore = -1;
            }
            catch (Exception e)
            {
                
            }
        }

        public void registerFinalScore(string IP,string myScore)
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
            else if (third.IP.Equals(IP))
            {
                playerThreeScore = int.Parse(myScore);
                thirdScore = true;
            }
            else if (fourth.IP.Equals(IP))
            {
                playerFourScore = int.Parse(myScore);
                fourthScore = true;
            }

            if (playerOneScore > playerTwoScore)
                winner = 1;
            else if (playerTwoScore > playerOneScore)
                winner = 2;
            else
                winner = 3;

            if (firstScore && secondScore)
            {
                sendGameOver();
            }
            //else if (third.IP.Equals(IP)) 
            // else if (fourth.IP.Equals(IP))
        }


        public int getCurrentTime()
        {
            TimeSpan time = stopWatch.Elapsed;
            if (!gameIsOver)
                return 60 - time.Seconds;
            else
                return 0;

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

        public void setPlayerPosition(string IP, float x, float y)
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
                positions[4] = 2;
                positions[5] = -8;
            }
            else if (playerNumber == 4)
            {
                positions[6] = 6;
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
            }
            else if (second.IP.Equals(IP))
            {
                playerTwoScore = int.Parse(myScore);
            }
            else if (third.IP.Equals(IP))
            {
                playerThreeScore = int.Parse(myScore);
            }
            else if (fourth.IP.Equals(IP))
            {
                playerFourScore = int.Parse(myScore);
            }

                winnerSet = !winnerSet;
                bothScoresSet = true;

                if (playerOneScore > playerTwoScore)
                    winner = 1;
                else if (playerTwoScore > playerOneScore)
                    winner = 2;
                else
                    winner = 3;
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
                else if (third.IP.Equals(IP))
                    p3Ready = true;
                else if (fourth.IP.Equals(IP))
                    p4Ready = true;
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
                else if (second.IP.Equals(IP))
                    p3Loaded = true;
                else if (second.IP.Equals(IP))
                    p4Loaded = true;
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

                if (playerCount <= 3)
                    playerCount++;
                
                return true;
            }
            else
                return false;

        }

        public string getPlayerName(int playerNumber)
        {
            if (playerNumber == 1)
            {
                try
                {
                    return first.getUserName();
                }
                catch (Exception e) { return "empty"; }
            }else if(playerNumber == 2)
            {
                try
                {
                    return second.getUserName();
                }
                catch (Exception e) { return "empty"; }
            }else if(playerNumber == 3)
            {
                try
                {
                    return third.getUserName();
                }
                catch (Exception e) { return "empty"; }
            }else if(playerNumber == 4)
            {
                try
                {
                    return fourth.getUserName();
                }
                catch (Exception e) { return "empty"; }
            }

            return "empty";
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
                //gameOver,result,score1,score2,score3,score4<EOF>
                string toSendP1 = "";
                string toSendP2 = "";

                if(winner == 1)
                    toSendP1 =  "gameOver,won," + playerOneScore +"," + playerTwoScore + "," + playerThreeScore + "," + playerFourScore + "<EOF>";
                else if(winner == -1)
                    toSendP1 =  "gameOver,tie," + playerOneScore +"," + playerTwoScore + "," + playerThreeScore + "," + playerFourScore + "<EOF>";
                else
                    toSendP1 = "gameOver,lost," + playerOneScore + "," + playerTwoScore + "," + playerThreeScore + "," + playerFourScore + "<EOF>";

                if(winner == 2)
                    toSendP2 = "gameOver,won," + playerOneScore + "," + playerTwoScore + "," + playerThreeScore + "," + playerFourScore + "<EOF>";
                else if(winner == -1)
                    toSendP2 =  "gameOver,tie," + playerOneScore +"," + playerTwoScore + "," + playerThreeScore + "," + playerFourScore + "<EOF>";
                else
                    toSendP2 = "gameOver,lost," + playerOneScore + "," + playerTwoScore + "," + playerThreeScore + "," + playerFourScore + "<EOF>";


                if (first != null)
                    NetworkHandler.Instance.sendMessage(first.IP, toSendP1);

                if (second != null)
                    NetworkHandler.Instance.sendMessage(second.IP, toSendP2);

                if (third != null)
                    NetworkHandler.Instance.sendMessage(third.IP, "gameOver,result," + playerOneScore + "," + playerTwoScore + "," + playerThreeScore + "," + playerFourScore + "<EOF>");

                if (fourth != null)
                    NetworkHandler.Instance.sendMessage(fourth.IP, "gameOver,result," + playerOneScore + "," + playerTwoScore + "," + playerThreeScore + "," + playerFourScore + "<EOF>");
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
                    TimeSpan time = stopWatch.Elapsed;

                    if (time.Minutes >= 1)
                    {
                        gameIsOver = true;
                        gameHasStarted = !gameHasStarted;
                        //GameHandler.Instance.endGame(SessionName);
                    }               
                }
                    
            }
        }

    }
}
