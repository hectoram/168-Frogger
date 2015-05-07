using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Networking
using System.Net;
using System.Net.Sockets;


namespace FroggerServer
{
    class Player
    {
        private string username;
        private string IP;
        private Socket connection;

        public Player(string name,Socket myConnection, string myIp) 
        {
            username = name;
            connection = myConnection;
            IP = myIp;
        }

        public bool send(string toSend)
        {
            return true;
        }

    }
}
