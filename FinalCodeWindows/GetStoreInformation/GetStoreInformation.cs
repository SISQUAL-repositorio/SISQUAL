using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using CompanyGetStoreLink;
using System.Globalization;
using System;
using System.Diagnostics;

namespace GetStoreInformation
{
    /// <summary>
    /// 
    /// </summary>
    public class GetStoreInformationClass
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stores"></param>
        /// <param name="zipCodeApiToken"></param>
        /// <param name="storesFilled"></param>
        /// <returns></returns>
        public static bool GetStoresLocation(List<Store> stores, string zipCodeApiToken, out List<Store> storesFilled)
        {
            storesFilled = new List<Store>();
            
            foreach (Store store in stores)
            {
                Store storeFilledToAdd;
                if (FillStoreInformation(store.PostalCode, zipCodeApiToken, out storeFilledToAdd))
                {
                    storesFilled.Add(storeFilledToAdd);
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postalCode"></param>
        /// <param name="zipCodeApiToken"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        private static bool FillStoreInformation(string postalCode, string zipCodeApiToken, out Store storeFilledToAdd)
        {
            storeFilledToAdd = new Store(postalCode);
            
            HttpClient client = new HttpClient();
            
            string content = client.GetStringAsync($"https://app.zipcodebase.com/api/v1/search?apikey={zipCodeApiToken}&codes={postalCode}").Result;
            
            JObject json = JObject.Parse(content);
            

            if (json == null || json["query"] == null || json["query"].Count() == 0)
            {
                return false;
            }

            try
            {
                if (json["results"][postalCode][0]["latitude"].Type != JTokenType.Null && json["results"][postalCode][0]["longitude"].Type != JTokenType.Null)
                {
                    string latitude = json["results"][postalCode][0]["latitude"].ToString();
                    string longitude = json["results"][postalCode][0]["longitude"].ToString();

                    storeFilledToAdd.Location = new Location(float.Parse(latitude, CultureInfo.InvariantCulture.NumberFormat), float.Parse(longitude, CultureInfo.InvariantCulture.NumberFormat));
                }

                string city = string.Empty;
                string country = string.Empty;
                string state = string.Empty;
                string province = string.Empty;

                if (json["results"][postalCode][0]["city"].Type != JTokenType.Null)
                {
                    city = json["results"][postalCode][0]["city"].ToString();
                }

                if (json["results"][postalCode][0]["country_code"].Type != JTokenType.Null)
                {
                    country = json["results"][postalCode][0]["country_code"].ToString();
                }

                if (json["results"][postalCode][0]["state"].Type != JTokenType.Null)
                {
                    state = json["results"][postalCode][0]["state"].ToString();
                }

                if (json["results"][postalCode][0]["province"].Type != JTokenType.Null)
                {
                    province = json["results"][postalCode][0]["province"].ToString();
                }

                storeFilledToAdd.FillStoreInformation(city, country, state, province);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
