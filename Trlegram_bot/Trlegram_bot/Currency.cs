using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trlegram_bot
{
    public class Currency
    {
        public class ApiResult
        {
            public Dictionary<string, Rate> Valute { get; set; }
        }

        public class Rate
        {
            public int Nominal { get; set; }
            public string Name { get; set; }
            public float Value { get; set; }

        }

        private RestClient RC = new RestClient();

        private const string URL = "https://www.cbr-xml-daily.ru/daily_json.js";

        private ApiResult rates;
        //USD, EUR, CHF, GBP
        //string currency

        public void download() 
        {
            var request = new RestRequest(URL);
            var Response = RC.Get(request);
            var json = Response.Content;
            rates = JsonConvert.DeserializeObject<ApiResult>(json);
        }

        public string getRate(string currency)
        {
            if (rates.Valute.ContainsKey(currency))
            {
                var rate = rates.Valute[currency];
                return $"Rate: {rate.Nominal} {rate.Name} costs {rate.Value} rubles";
            }
            else
            {
                return $"Sorry, i don't know {currency} currency :(";
            }
        }

        public Currency()
        {

        }
    }
}
