using System;
using System.Collections.Generic;
using Core;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace Client
{
    class NetworkClient
    {
        HttpClient Client = new HttpClient();
        public NetworkClient(String baseAdress)
        {
            Client.BaseAddress = new Uri(baseAdress);
        }
        public List<Contact> getAllContacts()
        {
            var getTask = Client.GetAsync(Strings.UriGetAll);
            getTask.Wait();
            HttpResponseMessage msg = getTask.Result;
            var parseTask = msg.Content.ReadAsStreamAsync();
            parseTask.Wait();
            Stream stream = parseTask.Result;
            XmlSerializer xmls = new XmlSerializer(typeof(List<Contact>));
            List<Contact> list = (List<Contact>)xmls.Deserialize(stream);
            return list;
        }
        public List<Contact> searchContacts(String searchString, String searchBy)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(searchString), Strings.KeySearchString);
            content.Add(new StringContent(searchBy), Strings.KeySearchType);
            var searchTask = Client.PostAsync(Strings.UriSearch, content);
            searchTask.Wait();
            HttpResponseMessage msg = searchTask.Result;
            var parseTask = msg.Content.ReadAsStreamAsync();
            parseTask.Wait();
            Stream stream = parseTask.Result;
            XmlSerializer xmls = new XmlSerializer(typeof(List<Contact>));
            List<Contact> list = (List<Contact>)xmls.Deserialize(stream);
            return list;
        }
        public void postContact(Contact contact)
        {
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(contact.name), Strings.KeyContactName);
                content.Add(new StringContent(contact.surname), Strings.KeyContactSnam);
                content.Add(new StringContent(contact.mail), Strings.KeyContactMail);
                content.Add(new StringContent(contact.number), Strings.KeyContactNumb);
                var postTask = Client.PostAsync(Strings.UriPostOne, content);
                postTask.Wait();
        }
    }
}
