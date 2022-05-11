using Newtonsoft.Json;

namespace MeteoStat
{
    public class Meteo
    {
        public static void Main(String[] args)
        {
            String apiKey = "417d052126msh1deb683cc269741p1edb74jsne4fb22630a5a";

            Console.Write("Use Default? -> ");
            String answer = Console.ReadLine();

            String[] answerPossiblities = { "yes", "y", "sim", "s" };

            string[]? requestParameters = new[] { "08545", "2020-01-01", "2020-01-31" };

            if(!answerPossiblities.Contains(answer.Trim().ToLower()))
            {
                requestParameters = getUserOptions();
            }

            string? returnJson = GetDataMeteo(requestParameters, apiKey).GetAwaiter().GetResult();

            Root organizedJson = JsonConvert.DeserializeObject<Root>(returnJson);

            organizedJson.logAllData();

        }
        public static String[] getUserOptions()
        {

            Console.Write("Do you wanna to input a station? -> ");
            String answer = Console.ReadLine();

            String[] answerPossiblities = { "yes", "y", "sim", "s" };
            String station = "08545";

            if (answerPossiblities.Contains(answer.Trim().ToLower()))
            {

                Console.Write("Input your station -> ");
                string? aux = Console.ReadLine();
                station = aux != null ? aux.Trim() : "08545";
            }

            Console.Write("Do you wanna to input a start date? -> ");
            String answerSd = Console.ReadLine();

            String startDate = "2020-01-01";

            if (answerPossiblities.Contains(answerSd.Trim().ToLower()))
            {

                Console.Write("Input your start date (YYYY-MM-DD) -> ");
                string? aux = Console.ReadLine();
                startDate = aux != null ? aux.Trim() : "2020-01-01";
            }

            Console.Write("Do you wanna to input a end date? -> ");
            String answerEd = Console.ReadLine();

            String endDate = "2020-01-31";

            if (answerPossiblities.Contains(answerEd.Trim().ToLower()))
            {

                Console.Write("Input your end date (YYYY-MM-DD) -> ");
                string? aux = Console.ReadLine();
                endDate = aux != null ? aux.Trim() : "2020-01-31";
            }

            return new[] { station, startDate, endDate };
        }

        public static async Task<String> GetDataMeteo(String [] requestParameters, String apiKey)
        {
            HttpClient? client = new HttpClient();
            HttpRequestMessage? request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://meteostat.p.rapidapi.com/stations/daily?station={requestParameters[0]}&start={requestParameters[1]}&end={requestParameters[2]}"),
                Headers =
                {
                    { "X-RapidAPI-Host", "meteostat.p.rapidapi.com" },
                    { "X-RapidAPI-Key", apiKey },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string? body = await response.Content.ReadAsStringAsync();
                return body;
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

        public class Data
        {
            public string? date { get; set; }
            public double? tavg { get; set; }
            public double? tmin { get; set; }
            public double? tmax { get; set; }
            public double? prcp { get; set; }
            public object? snow { get; set; }
            public double? wdir { get; set; }
            public double? wspd { get; set; }
            public double? wpgt { get; set; }
            public double? pres { get; set; }
            public object? tsun { get; set; }

            public void logAll()
            {
                Console.WriteLine($"\nDate :  {date} \n\nAverage Temperature : {tavg} Cº \nMinimum Temperature : {tmin} Cº \nMaximum Temperature : {tmax} Cº \nPrecipitation(milimeters) : {(prcp != null ? prcp : "No data")}  \nSnow(millimeters) : {(snow != null ? snow : "No data")} " +
                    $"\nWind Direction(Degrees) : {(wdir != null ? wdir : "No data")}  \nWind Speed : {wspd} Km/h \nWind Gust Peak : {wpgt} Km/h \nPressure : {pres} hPa \nDaily Sunshine(Minutes) : {(tsun != null ? tsun : "No data")}");
            }
            public void logDate()
            {
                Console.WriteLine($"date {date}");
            }
            public void logTemperature()
            {
                Console.WriteLine($"tavg {tavg} \ntmin {tmin} \ntmax {tmax}");
            }
            public void logPrcp()
            {
                Console.WriteLine($"prcp {prcp}");
            }
            public void logSnow()
            {
                Console.WriteLine($"snow {(snow != null ? snow : "no data")}");
            }
            public void logWind()
            {
                Console.WriteLine($"wdir {wdir} \nwspd {wspd} \nwpgt {wpgt}");
            }
            public void logPres()
            {
                Console.WriteLine($"pres {pres}");
            }
            public void logTsun()
            {
                Console.WriteLine($"tsun {(tsun != null ? tsun : "no data")}");
            }

        }

        public class Root
        {
            public Meta? meta { get; set; }
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
}