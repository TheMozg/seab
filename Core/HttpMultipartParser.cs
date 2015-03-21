using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Core
{
    public class HttpMultipartParser
    {

        public HttpMultipartParser(Stream stream)
        {
            this.Parse(stream);
        }

        private void Parse(Stream stream)
        {
            this.Success = false;

            byte[] data = Misc.ToByteArray(stream);

            string content = Encoding.UTF8.GetString(data);

            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = content.Substring(0, content.IndexOf("\r\n"));

                string[] sections = content.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in sections)
                {
                    if (s.Contains("Content-Disposition"))
                    {
                        string name = "";
                        Match nameMatch = new Regex(@"name=(.*)").Match(s);
                        if (nameMatch.Groups.Count == 2)
                            name = nameMatch.Groups[1].Value.Trim().ToLower();

                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            int startIndex = nameMatch.Index + nameMatch.Length + "\r\n".Length;
                            Parameters.Add(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());
                        }
                    }
                }

                if (Parameters.Count != 0)
                    this.Success = true;
            }
        }

        public IDictionary<string, string> Parameters = new Dictionary<string, string>();

        public bool Success
        {
            get;
            private set;
        }
    }
}
