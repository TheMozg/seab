using System;
using System.Collections.Generic;
using Core;
using System.Net;

namespace Client
{
    class CLI
    {
        NetworkClient client = new NetworkClient("http://localhost:8080/");
        Int32 getValidatedInt(List<Int32> options)
        {
            Int32 res = 0;
            for (; ; )
            {
                Console.Write("> ");
                if (Int32.TryParse(Console.ReadLine(), out res))
                {
                    if (options.Contains(res))
                        break;
                }
                Console.WriteLine("Please enter valid NUMBER.");
            }
            return res;
        }
        public void startActionLoop()
        {
            Console.WriteLine("Enter the number of action and press [Enter]. Then follow instructions.");
            bool exit = false;
            while (!exit)
            {
                try
                {
                    Console.WriteLine("Menu:\n1. View all contacts\n2. Search\n3. New contact\n4. Exit");
                    int choice = getValidatedInt(new List<Int32> { 1, 2, 3, 4 });
                    switch (choice)
                    {
                        case 1:
                            displayAllContacts();
                            break;
                        case 2:
                            search();
                            break;
                        case 3:
                            AddContact();
                            break;
                        case 4:
                            exit = true;
                            break;
                    }
                }
                catch(AggregateException ex)
                {
                      Console.WriteLine("Error connecting to the server");
                }
            }
        }
        void displayContact(Contact contact)
        {
            Console.WriteLine("Contact:");
            Console.WriteLine("Name: {0}", contact.name);
            Console.WriteLine("Surname: {0}", contact.surname);
            Console.WriteLine("Phone: {0}", contact.number);
            Console.WriteLine("Mail: {0}", contact.mail);
            Console.WriteLine();
        }
        void search()
        {
            Console.WriteLine("Search by:");
            Console.WriteLine("1. Name\n2. Name and surname\n3. Phone number\n4. All fields");
            int choice = getValidatedInt(new List<Int32> { 1, 2, 3, 4 });
            Console.Write("Request: ");
            String str = Console.ReadLine();
            List<Contact> searchResults = new List<Contact>();
            switch (choice)
            {
                case 1:
                    searchResults = client.searchContacts(str, Strings.SearchTypeName);
                    break;
                case 2:
                    searchResults = client.searchContacts(str, Strings.SearchTypeNplS);
                    break;
                case 3:
                    searchResults = client.searchContacts(str, Strings.SearchTypeNumb);
                    break;
                case 4:
                    searchResults = client.searchContacts(str, Strings.SearchTypeAll);
                    break;
            }
            if (searchResults.Count == 0)
            {
                Console.WriteLine("No matches found.");
            }
            else
            {
                Console.WriteLine("Found {0} matches", searchResults.Count);
                displayContacts(searchResults);
            }
        }
        void displayContacts(List<Contact> list)
        {
            if (list.Count == 0)
                Console.WriteLine("Nothing to display.");
            else
                Console.WriteLine("All contacts ({0}):", list.Count);
            foreach (Contact contact in list)
            {
                displayContact(contact);
            }
        }
        void displayAllContacts()
        {
            List<Contact> list = client.getAllContacts();
            displayContacts(list);
        }
        void AddContact()
        {
            Contact contact = new Contact();
            Console.Write("Name: ");
            contact.name = Console.ReadLine();
            Console.Write("Surname: ");
            contact.surname = Console.ReadLine();
            Console.Write("Phone number: ");
            contact.number = Console.ReadLine();
            Console.Write("Mail: ");
            contact.mail = Console.ReadLine();
            Console.WriteLine("Contact created");

            client.postContact(contact);
        }
    }
}
