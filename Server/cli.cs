using System;
using System.Collections.Generic;
using System.Net;
using Core;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using System.Threading;

namespace Server
{
    class CLI
    {
        AdressBook ab = new AdressBook();
        HttpListener listener = new HttpListener();
        public CLI()
        {
            Console.WriteLine("App started");
            listener.Prefixes.Add("http://+:8080/");
        }
        public void Start()
        {
            listener.Start();
            while (true)
            {
                var res = listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
                res.AsyncWaitHandle.WaitOne();
            }
        }
        void ListenerCallback(IAsyncResult result)
        {
            Console.WriteLine("Callback method was called");

            HttpListenerContext context = listener.EndGetContext(result);
            processRequest(context);
        }
        void processPostOneRequest(HttpListenerContext context)
        {
            Console.WriteLine("Processing POST request (create)");
            Thread.Sleep(5000);

            var parser = new HttpMultipartParser(context.Request.InputStream);
            if (parser.Success)
            {
                Contact contact = new Contact();
                contact.name = parser.Parameters[Strings.KeyContactName];
                contact.surname = parser.Parameters[Strings.KeyContactSnam];
                contact.number = parser.Parameters[Strings.KeyContactNumb];
                contact.mail = parser.Parameters[Strings.KeyContactMail];
                ab.Add(contact);
            }
            context.Response.Close();
            Console.WriteLine("Processed request (create)");
        }
        void processRequest(HttpListenerContext context)
        {
            String rawUrl = context.Request.RawUrl;
            Console.WriteLine("Requested uri: {0}", rawUrl);
            if (rawUrl.StartsWith("/"))
                rawUrl = rawUrl.Remove(0, 1);
            if (rawUrl == Strings.UriPostOne)
            {
                processPostOneRequest(context);
            }
            if (rawUrl == Strings.UriGetAll)
            {
                processGetAllRequest(context);
            }
            if (rawUrl == Strings.UriSearch)
            {
                processSearchRequest(context);
            }
        }
        private void processSearchRequest(HttpListenerContext context)
        {
            Console.WriteLine("Processing POST request (search)");

            string searchString = "";
            string searchBy = "";
            var parser = new HttpMultipartParser(context.Request.InputStream);
            if (parser.Success)
            {
                searchString = parser.Parameters[Strings.KeySearchString];
                searchBy = parser.Parameters[Strings.KeySearchType];
            }
            List<Contact> searchResults = new List<Contact>();
            switch (searchBy)
            {
                case Strings.SearchTypeName:
                    searchResults = ab.Search(searchString, SearchType.Name);
                    break;
                case Strings.SearchTypeNplS:
                    searchResults = ab.Search(searchString, SearchType.NameAndSurname);
                    break;
                case Strings.SearchTypeNumb:
                    searchResults = ab.Search(searchString, SearchType.Phone);
                    break;
                case Strings.SearchTypeAll:
                    searchResults = ab.Search(searchString, SearchType.All);
                    break;
            }

            Misc.Serialize(context.Response.OutputStream, searchResults);
            context.Response.Close();

            Console.WriteLine("Processed request (search)");
        }

        private void processGetAllRequest(HttpListenerContext context)
        {
            Console.WriteLine("Processing GET request (getall)");

            Misc.Serialize(context.Response.OutputStream, ab.MasterList);
            context.Response.Close();

            Console.WriteLine("Processed request (getall)");
        }
    }
}
