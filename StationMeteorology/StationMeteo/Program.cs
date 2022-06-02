using Newtonsoft.Json;

namespace StationMeteo
{
    public class Station
    {
        public static void Main(String[] args)
        {
            String apiKey = "417d052126msh1deb683cc269741p1edb74jsne4fb22630a5a";

            Console.Write("Use Default? -> ");
            String answer = Console.ReadLine();

            String[] answerPossiblities = { "yes", "y", "sim", "s" };

            string[]? requestParameters = new[] { "38.7167" , "-9.1333" };

            if (!answerPossiblities.Contains(answer.Trim().ToLower()))
            {
                requestParameters = getUserOptions();
            }

            string? returnJson = GetDataMeteo(requestParameters, apiKey).GetAwaiter().GetResult();

            Root organizedJson = JsonConvert.DeserializeObject<Root>(returnJson);

            organizedJson.logAllData();

        }
        public static String[] getUserOptions()
        {

            Console.Write("Do you want to input Latitude? -> ");
            String answer = Console.ReadLine();

            String[] answerPossiblities = { "yes", "y", "sim", "s" };
            string? latitude = "38.7167";
            
            if (answerPossiblities.Contains(answer.Trim().ToLower()))
            {

                Console.Write("Input latitude -> ");
                string? aux = Console.ReadLine();
                latitude = aux != null ? aux.Trim() : "38.7167";

            }

            Console.Write("Do you wanna to input Longitude? -> ");
            String answerLon = Console.ReadLine();

            string? longitude = "-9.1333";

            if (answerPossiblities.Contains(answerLon.Trim().ToLower()))
            {

                Console.Write("Input longitude -> ");
                string? aux1 = Console.ReadLine();
                longitude = aux1 != null ? aux1.Trim() : "-9.1333";
            }

            return new[] { latitude, longitude };   
        }


        public static async Task<String> GetDataMeteo(String[] requestParameters, String apiKey)
        {
            HttpClient? client = new HttpClient();
            HttpRequestMessage? request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri($"https://meteostat.p.rapidapi.com/stations/nearby?lat={requestParameters[0]}&lon={requestParameters[1]}"),
				Headers =
	            {
		            { "X-RapidAPI-Host", "meteostat.p.rapidapi.com" },
		            { "X-RapidAPI-Key", apiKey },
	            },
			};
			using (HttpResponseMessage? response = await client.SendAsync(request))
			{
				response.EnsureSuccessStatusCode();
                string? body = await response.Content.ReadAsStringAsync();
                return body;
            }
		}
    }
    public class Meta
    {
        public string? generated { get; set; }

        public void logGenerated()
        {
            Console.WriteLine(generated);
        }
    }

    public class Name
    {
        public string? en { get; set; }

        public void logEn()
        {
            Console.WriteLine(en);
        }
        public override string ToString()
        {
            return en.ToString();
        }
    }

    public class Data
    {
        public string? id { get; set; }
        public Name? name { get; set; }
        public double? distance { get; set; }

        public void logAll()
        {
            Console.WriteLine($"\nState Id : {id} \nName : {name.ToString()} \nDistance : {distance} m");
        }
        public void logID()
        {
            Console.WriteLine($"State Id:  {id}");
        }
        public void logName()
        {
            Console.WriteLine($"Name: {name}");
        }
        public void logDistance()
        {
            Console.WriteLine($"Distance {distance}");
        }
    }

    public class Root
    {
        public Meta meta { get; set; }
        public List<Data> data { get; set; }


        public void logAllData()
        {
            foreach (Data? item in data)
            {
                item.logAll();
            }
        }
    }
}
