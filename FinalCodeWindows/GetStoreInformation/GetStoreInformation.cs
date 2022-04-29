using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using CompanyGetStoreLink;
using System.Globalization;

namespace GetStoreInformation
{
    public class GetStoreInformationClass
    {
        public static Company getStoresLocation(Company company)
        {
            List<Store> stores = company.getStores();
            List<Store> storesFilled = new List<Store>();

            foreach (Store store in stores)
            {
                Store storeFilledToAdd = fillStoreInformation(store);
                storesFilled.Add(storeFilledToAdd);
            }

            company.setStores(storesFilled);

            return company;
        }
        public static Store fillStoreInformation(Store store)
        {
            HttpClient client = new HttpClient();
            string key = "de744d20-bb62-11ec-bc34-ab9b8c173a58";

            string storePostalCode = store.getPostalCode();

            string content = client.GetStringAsync($"https://app.zipcodebase.com/api/v1/search?apikey={key}&codes={storePostalCode}").Result;

            JObject json = JObject.Parse(content);

            if (json["query"] == null || json["query"].Count() == 0)
            {
                return store;
            }
            try
            {
                string latitude = json["results"][storePostalCode][0]["latitude"].ToString();
                string longitude = json["results"][storePostalCode][0]["longitude"].ToString();

                store.setLocation(new Location(float.Parse(latitude, CultureInfo.InvariantCulture.NumberFormat), float.Parse(longitude, CultureInfo.InvariantCulture.NumberFormat)));

                string city = json["results"][storePostalCode][0]["city"].ToString();
                string country = json["results"][storePostalCode][0]["country_code"].ToString();
                string state = json["results"][storePostalCode][0]["state"].ToString();
                string province = json["results"][storePostalCode][0]["province"].ToString();

                store.fillStoreInformation(city, country, state, province);
            }
            catch (System.ArgumentException e)
            {
                return store;
            }

            return store;
        }
    }
}
