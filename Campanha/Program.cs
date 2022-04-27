using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Campanha
{
    class Program
    {
        static public void Main(string[] args)
        {
            System.Console.WriteLine("What are you looking for?");
            string? user_input = System.Console.ReadLine();

            HttpClient client = new HttpClient();
            int sucess_rate = 100;
            string api_key = "ab1cf9273a4fbe3f070538b3837a244489be334839789dffe5ce0555cf7d26be";
            string number_search = "50";
            string country_code = "pt";



            string content = client.GetStringAsync
            ($"https://serpapi.com/search.json?engine=google&q={user_input}&api_key={api_key}&num={number_search}&gl={country_code}").Result;

            var json = JObject.Parse(content);
            int get_count = json["organic_results"].Count();


            List<string> list_links = new List<string>();
            List<string> list_snippet = new List<string>();
            List<string> list_snippet_w_sucess_rate = new List<string>();


            for (int i = 0; i < get_count; i++)
            {
                string? get_link = json["organic_results"]?[i]?["link"]?.ToString();
                list_links.Add("\nLink - > " + get_link);

            }

            for (int i = 0; i < get_count; i++)
            {
                string? get_snippet = json["organic_results"]?[i]?["snippet"]?.ToString();
                if (get_snippet != null)
                {
                    list_snippet.Add(get_snippet);

                }

            }

            foreach (var item in list_snippet)
            {
                var item_lowerCase = item.ToLower();
                

                if (item_lowerCase.Contains("black") && item_lowerCase.Contains("friday") == false)
                {
                    sucess_rate = 20;
                    list_snippet_w_sucess_rate.Add(item + " -> " + sucess_rate + "% probabilidade de sucesso");
                }

                else if (item_lowerCase.Contains("black") && item_lowerCase.Contains("friday"))
                {
                    sucess_rate = 40;
                    list_snippet_w_sucess_rate.Add(item + " -> " + sucess_rate + "% probabilidade de sucesso");
                }
                else
                {
                    list_snippet_w_sucess_rate.Add(item);
                }
            }

            for (int i = 0; i < list_snippet.Count; i++)
            {
                System.Console.WriteLine(list_links[i]);
                System.Console.WriteLine(list_snippet_w_sucess_rate[i]);
            }
        }

    }

}



