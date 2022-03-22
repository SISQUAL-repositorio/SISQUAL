using Newtonsoft.Json;
using System.Text;

using utils;

namespace app
{
class Program {
    private static readonly HttpClient client = new HttpClient();

    static public void Main(string[] args)
    {    
        doPostRequest("", "");
    }

    private async static void doPostRequest(string ul, string cnt) 
    {
        List<string> parentSelectors = new List<string> {"_root"};
        
        string type = "SelectorText";
        bool multiple = true;
        string id = "store_info";
        string selector = "li div.middle"; // this will be the one than changes
        string regex = "";
        string delay = "";
        Selector Selector = new Selector(parentSelectors, type, multiple, id, selector, regex, delay);
        
        string _id = "boutique_dos_relogios_2"; //name of the company
        string startUrl = "https://boutiquedosrelogios.pt/sobre-nos/4/encontre-uma-loja"; //output of the finding program in code folder
        List<Selector> selectors = new List<Selector> {Selector};

        Post_json post_Json = new Post_json(_id, startUrl, selectors);

        var json = post_Json.getJson();
        //Console.WriteLine(json);
        
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var url = "https://api.webscraper.io/api/v1/sitemap?api_token=APeynP2lCFaYutcT9OnWszQ2pYuG2MVtPwBvDlbiFfWIX2HQt5VdhPGmUHd6";
        
        using var client = new HttpClient();
        var response = client.PostAsync(url, data).Result;
        var result = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(result);

        Console.WriteLine("content");
        var content = client.GetStringAsync("https://api.webscraper.io/api/v1/scraping-job/7575627/json?api_token=APeynP2lCFaYutcT9OnWszQ2pYuG2MVtPwBvDlbiFfWIX2HQt5VdhPGmUHd6").Result;
        Console.WriteLine("content");
        Console.WriteLine(content);
    }


    public static void createScrapingJob() {

    }
}
}