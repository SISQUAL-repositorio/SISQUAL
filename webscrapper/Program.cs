using Newtonsoft.Json;
using System.Text;

using utils;

namespace app
{
class Program {
    static public static await Main(string[] args)
    {    
        using var client = new HttpClient();
        var content = client.GetStringAsync("http://api.positionstack.com/v1/forward?access_key=26d930d54271f6ca02183adb2b5a2133&query=Av.%20José%20Bonifácio%20de%20Andrade%20e%20Silva%20Coimbra&country=Portugal&fields=results.latitude,results.longitude").Result;
        
        Console.WriteLine(content);
    }
}
}