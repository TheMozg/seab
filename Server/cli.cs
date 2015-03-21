using System;
using System.Collections.Generic;
using System.Net;
using Core;
using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace Server
{
    class CLI
    {
        AdressBook ab = new AdressBook();
        public void startActionLoop()
        {
            Console.WriteLine("App started");
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/" + Strings.UriGetAll + "/");
            listener.Prefixes.Add("http://localhost:8080/" + Strings.UriPostOne + "/");
            listener.Prefixes.Add("http://localhost:8080/" + Strings.UriSearch + "/");
            listener.Start();
            Console.WriteLine("Service started");
            Console.WriteLine();
            while (true)
            {
                Console.WriteLine("Waiting for request");
                var context = listener.GetContext();
                Console.WriteLine("Received request");
                String str = context.Request.RawUrl;
                if (str.StartsWith("/"))
                    str = str.Remove(0, 1);
                Console.WriteLine("Requested uri: {0}", str);
                if(str == Strings.UriPostOne)
                {
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
                    Console.WriteLine("Processed POST request");
                }
                if (str == Strings.UriGetAll)
                {
                    var list = ab.MasterList;
                    XmlSerializer xmls = new XmlSerializer(list.GetType());
                    var resp = context.Response.OutputStream;
                    xmls.Serialize(context.Response.OutputStream, list);
                    resp.Close();
                    Console.WriteLine("Processed GET request");
                }
                if (str == Strings.UriSearch)
                {
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

                    XmlSerializer xmls = new XmlSerializer(searchResults.GetType());
                    var resp = context.Response.OutputStream;
                    xmls.Serialize(context.Response.OutputStream, searchResults);
                    resp.Close();
                    Console.WriteLine("Processed POST request");
                }
                Console.WriteLine();
            }
        }
    }
}
