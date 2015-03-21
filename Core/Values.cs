using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public enum SearchType
    {
        Name,
        NameAndSurname,
        Phone,
        All
    }
    public static class Strings
    {
        public const string UriGetAll = "getall";
        public const string UriSearch = "search";
        public const string UriPostOne = "postone";
        public const string KeySearchString = "searchstr";
        public const string KeySearchType = "searchtype";
        public const string KeyContactName = "name";
        public const string KeyContactSnam = "surname";
        public const string KeyContactMail = "email";
        public const string KeyContactNumb = "number";
        public const string SearchTypeName = "sname";
        public const string SearchTypeNplS = "snpluss";
        public const string SearchTypeNumb = "snumber";
        public const string SearchTypeAll = "sall";

    }
}
