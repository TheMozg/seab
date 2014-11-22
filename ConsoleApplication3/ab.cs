using System;
using System.Collections.Generic;

namespace SEab
{
    class AdressBook
    {
        public enum SearchType
        {
            Name,
            NameAndSurname,
            Phone,
            All
        }
        List<Contact> masterList = new List<Contact>();
        public List<Contact> MasterList
        {
            get
            {
                return masterList;
            }
        }
        public void Add(String name, String surname, String number, String mail)
        {
            masterList.Add(new Contact { name = name, surname = surname, number = number, mail = mail });
        }

        public List<Contact> Search(String query, SearchType st)
        {
            Predicate<Contact> predicate;
            switch (st)
            {
                case SearchType.Name:
                    {
                        predicate = contact => contact.name.Contains(query);
                        break;
                    }
                case SearchType.NameAndSurname:
                    {
                        predicate = contact => contact.name.Contains(query) || contact.surname.Contains(query);
                        break;
                    }
                case SearchType.Phone:
                    {
                        predicate = contact => contact.number.Contains(query);
                        break;
                    }
                case SearchType.All:
                    {
                        predicate = contact => contact.number.Contains(query) || contact.surname.Contains(query)
                            || contact.name.Contains(query) || contact.mail.Contains(query);
                        break;
                    }
                default:
                    {
                        predicate = contact => false;
                        break;
                    }
            }
            return masterList.FindAll(predicate);
        }
    }
}
