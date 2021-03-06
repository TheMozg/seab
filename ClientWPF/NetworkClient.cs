﻿using System;
using System.Collections.Generic;
using Core;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace ClientWPF
{
    class NetworkClient
    {
        HttpClientHandler clientHandler;
        HttpClient Client;
        public bool ValidServerUri = true;
        public string BaseAdress
        {
            get
            {
                return Client.BaseAddress.OriginalString;
            }
            set
            {
                try
                {
                    Client.BaseAddress = new Uri(value);
                }
                catch
                {
                    ValidServerUri = false;
                }
            }
        }
        public NetworkClient()
        {
            clientHandler = new HttpClientHandler();
            Client = new HttpClient(clientHandler);
        }
        public List<Contact> getAllContacts()
        {
            var getTask = Client.GetAsync(Strings.UriGetAll);
            getTask.Wait();
            HttpResponseMessage msg = getTask.Result;
            var parseTask = msg.Content.ReadAsStreamAsync();
            parseTask.Wait();
            List<Contact> list = Misc.Deserialize(parseTask.Result);

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
            List<Contact> list = Misc.Deserialize(parseTask.Result);

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
