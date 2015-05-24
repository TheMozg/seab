using System;
using System.Collections.Generic;

namespace Client
{
    class EntryPoint
    {
        public static void Main(string[] args)
        {
            CLI cli = new CLI();
            if(args.Length == 1)
                cli.BaseAdress = args[0];
            cli.startActionLoop();
        }
    }
}

