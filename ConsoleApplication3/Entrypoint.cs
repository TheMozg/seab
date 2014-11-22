using System;
using System.Collections.Generic;

namespace SEab
{
    /*
    class vCardWrapper
    {
        Dictionary<String, String> dict = new Dictionary<String, String>();
        vCardWrapper()
        {
            dict.Add("name", "FN");
            dict.Add("name", "FN");

        }
        void saveToFile(List<Contact> list, String path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("BEGIN:VCARD");
                sw.WriteLine("VERSION:3.0");

                sw.WriteLine("END:VCARD");
            }
        }
    }*/

    class EntryPoint
    {
        public static void Main(string[] args)
        {
            CLI cli = new CLI();
            cli.startActionLoop();
        }
    }
}

