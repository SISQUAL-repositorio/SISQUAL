using System.Net;

namespace app

{
class Program {
    private static HttpClient client = null;

    public static void Main(string[] args)
    {    
        //using var client = new HttpClient();
        //var content = client.GetStringAsync("http://api.positionstack.com/v1/forward?access_key=26d930d54271f6ca02183adb2b5a2133&query=Av.%20José%20Bonifácio%20de%20Andrade%20e%20Silva%20Coimbra&country=Portugal&fields=results.latitude,results.longitude").Result;
        
        //Console.WriteLine(content);
    }

    public string Get(string uri)
    {
        if(client == null) {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };        
            
            client = new HttpClient(handler);
        }

        client.BaseAddress = new Uri("https://api.stackexchange.com/2.2/");

        HttpResponseMessage response = client.GetAsync("answers?order=desc&sort=activity&site=stackoverflow").Result;
        response.EnsureSuccessStatusCode();
        string result = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine("Result: " + result);           
    }
}
}