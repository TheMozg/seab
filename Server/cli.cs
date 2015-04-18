using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        ConcurrentDictionary<String, AdressBook> abs = new ConcurrentDictionary<string, AdressBook>();
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
                Console.WriteLine("Waiting for request");
                var context = listener.GetContext();
                new Thread(() => processRequest(context)).Start();
                Console.WriteLine("New thread despatched");
            }
        }
        void processPostOneRequest(HttpListenerContext context, AdressBook ab)
        {
            Console.WriteLine("Processing POST request (create contact)");

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

            Console.WriteLine("Processed request");
        }
        void processRequest(HttpListenerContext context)
        {
            Cookie cookie = context.Request.Cookies["id"];
            String id = "";
            if(cookie != null)
                id = cookie.Value;
            Console.WriteLine("id is {0}", id);
            if (String.IsNullOrEmpty(id))
            {
                Guid guid = Guid.NewGuid();
                id = guid.ToString();
                cookie = new Cookie("id", id);
                cookie.Path = "/";
            }
            context.Response.SetCookie(cookie);
            AdressBook ab = abs.GetOrAdd(id, new AdressBook());

            String rawUrl = context.Request.RawUrl;
            Console.WriteLine("Requested uri: {0}", rawUrl);
            if (rawUrl.StartsWith("/"))
                rawUrl = rawUrl.Remove(0, 1);
            if (rawUrl == Strings.UriPostOne)
            {
                processPostOneRequest(context, ab);
            }
            if (rawUrl == Strings.UriGetAll)
            {
                processGetAllRequest(context, ab);
            }
            if (rawUrl == Strings.UriSearch)
            {
                processSearchRequest(context, ab);
            }
        }
        private void processSearchRequest(HttpListenerContext context, AdressBook ab)
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

            Console.WriteLine("Processed request");
        }

        private void processGetAllRequest(HttpListenerContext context, AdressBook ab)
        {
            Console.WriteLine("Processing GET request (getall)");

            Misc.Serialize(context.Response.OutputStream, ab.MasterList);
            context.Response.Close();

            Console.WriteLine("Processed request");
        }
    }
}
