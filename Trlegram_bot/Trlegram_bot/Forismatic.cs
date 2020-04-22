using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trlegram_bot
{
    public class Forismatic
    {
        public class Quote
        {
            public string quoteText { get; set; }
            public string quoteAuthor { get; set; }
        }

        private RestClient RC = new RestClient();

        private const string URL = "https://api.forismatic.com/api/1.0/?method=getQuote&format=json&lang=en";

        public string getRandom()
        {
            var request = new RestRequest(URL);
            var Response = RC.Get(request);
            var json = Response.Content;

            var quote = JsonConvert.DeserializeObject<Quote>(json);

            if (quote.quoteAuthor.Length > 0)
            {
                return $"Wise {quote.quoteAuthor} once said: \"{quote.quoteText.Trim()}\"";
            }
            else
            {
                return $"Someone oce said: \"{quote.quoteText.Trim()}\"";
            }
        }

        public Forismatic()
        {

        }
    }
}
