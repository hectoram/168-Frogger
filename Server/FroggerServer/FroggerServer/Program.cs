using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//networking
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FroggerServer
{
    class Program
    {
        
            public static int Main(String[] args)
            {
                NetworkHandler.Instance.init();
                //Since the server is always running do this
                while(true)
                {
                   NetworkHandler.Instance.update();
                }

                return 0;
            }
        }
    }

