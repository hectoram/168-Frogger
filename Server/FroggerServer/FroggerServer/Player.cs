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
        private Socket connection;

        public Player(string name,Socket myConnection ) 
        {
            username = name;
            connection = myConnection;
        }

        public bool send(string toSend)
        {
            return true;
        }

    }
}
