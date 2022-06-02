using System;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using CompanyGetStoreLink;

namespace ClassLibrary1
{
    public class Class1
    {
        private static string apiKey = "417d052126msh1deb683cc269741p1edb74jsne4fb22630a5a";
        static void Main()
        {
            Company worten = new Company("worten");
            List<Store> mockStores = new List<Store>();

            Store mockStore1 = new Store("4770-751");
            mockStore1.FillStoreInformation("porto", "portugal", "state", "provincia");
            mockStore1.setLocation(new Location((float)42.45, (float)-8.5));
            Store mockStore2 = new Store("4770-751");
            mockStore2.FillStoreInformation("porto", "portugal", "state", "provincia");
            mockStore2.setLocation(new Location((float)41.45, (float)-6.5));
            Store mockStore3 = new Store("4770-751");
            mockStore3.FillStoreInformation("porto", "portugal", "state", "provincia");
            mockStore3.setLocation(new Location((float)40.45, (float)-7.5));
            Store mockStore4 = new Store("4770-751");
            mockStore4.FillStoreInformation("porto", "portugal", "state", "provincia");
            mockStore4.setLocation(new Location((float)43.45, (float)-9.5));

            mockStores.Add(mockStore1);
            mockStores.Add(mockStore2);
            mockStores.Add(mockStore3);
            mockStores.Add(mockStore4);

            HashSet<Station> stations = new HashSet<Station>();

            foreach(Store store in worten.getStores())
            {
                Station newStation = getNearbyStation(store);
                if (newStation != null) 
                { 
                    stations.Add(newStation); 
                }
            }

            foreach (Station station in stations)
            {
                Console.WriteLine(station.toString());
            }
        }

        private static Station getNearbyStation(Store store)
        {
            float latitude = store.getLatitude();
            float longitude = store.getLongitude();

            Station nearbyStation = apiRequestNearbyStation(latitude, longitude, apiKey).GetAwaiter().GetResult();

            return nearbyStation;
        }

        public static async Task<Station> apiRequestNearbyStation(float latitude, float longitude, string apiKey)
        {
            int id = -1;
            string name = "";
            
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://meteostat.p.rapidapi.com/stations/nearby?lat={latitude}&lon={longitude}"),
                Headers =
                {
                    { "X-RapidAPI-Host", "meteostat.p.rapidapi.com" },
                    { "X-RapidAPI-Key", apiKey },
                },
            };
            using (HttpResponseMessage response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string body = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(body);
                id = Int32.Parse(json["data"][0]["id"].ToString());
                name = json["data"][0]["name"].ToString();
            }

            if (id == -1 || name == "")
            {
                return null;
            }

            return new Station(id, name);
        }
    }

}
