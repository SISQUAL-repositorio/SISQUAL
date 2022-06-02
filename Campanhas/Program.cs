using Newtonsoft.Json.Linq;

namespace Campanhas
{
    public class Program
    {
        static public void Main(string[] args)
        {
            //user input - store
            System.Console.WriteLine("What store are you looking for?");
            string? userInput = System.Console.ReadLine();

            //user input - event
            System.Console.WriteLine("\nWhat event are you looking for?");
            string? userInput2 = System.Console.ReadLine();

            //join inputs
            string joinInput = userInput + " " + userInput2;

            //get requests
            HttpClient client = new HttpClient();
            int sucess_rate = 100;
            string apiKey = "ab1cf9273a4fbe3f070538b3837a244489be334839789dffe5ce0555cf7d26be";
            string numberSearch = "50";
            string countryCode = "pt";


            //use api
            string content = client.GetStringAsync
            ($"https://serpapi.com/search.json?engine=google&q={joinInput}&api_key={apiKey}&num={numberSearch}&gl={countryCode}").Result;

            //parse information
            JObject json = JObject.Parse(content);
            int getCount = json["organic_results"].Count();

            //create lists to save data
            List<string> listOfLinks = new List<string>();
            List<string> listOfSnippets = new List<string>();
            List<string> listOfSnippetsWithSucessRate = new List<string>();
            List<string> wordsToFind = new List<string>() { $"{userInput}", $"{userInput2}" };

            //get links from json
            for (int i = 0; i < getCount; i++)
            {
                string? get_link = json["organic_results"]?[i]?["link"]?.ToString();
                listOfLinks.Add("\nLink - > " + get_link);

            }

            //get snippets from json
            for (int i = 0; i < getCount; i++)
            {
                string? get_snippet = json["organic_results"]?[i]?["snippet"]?.ToString();
                if (get_snippet != null)
                {
                    listOfSnippets.Add(get_snippet);

                }

            }



            //calculate sucess rate
            foreach (var item in listOfSnippets)
            {
                var itemLowerCase = item.ToLower();

                int count = 0;
                foreach (string word in wordsToFind)
                {
                    if (itemLowerCase.Contains(word))
                    {
                        count++;
                    }

                }

                if (count >= wordsToFind.Count)
                {
                    sucess_rate = 70;
                    listOfSnippetsWithSucessRate.Add(item + " -> " + sucess_rate + "% probabilidade de sucesso");
                }

                if (count == (wordsToFind.Count / wordsToFind.Count))
                {
                    sucess_rate = 40;
                    listOfSnippetsWithSucessRate.Add(item + " -> " + sucess_rate + "% probabilidade de sucesso");
                }


                else if (count == 0)
                {
                    listOfSnippetsWithSucessRate.Add(item);
                }
            }

            //print lists
            for (int i = 0; i < listOfSnippets.Count; i++)
            {
                System.Console.WriteLine(listOfLinks[i]);
                System.Console.WriteLine(listOfSnippetsWithSucessRate[i]);
            }
        }

    }

}



