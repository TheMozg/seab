using System;
using System.Collections.Generic;

namespace Server
{
    class EntryPoint
    {
        public static void Main(string[] args)
        {
            CLI cli = new CLI();
            cli.Start();
        }
    }
}

