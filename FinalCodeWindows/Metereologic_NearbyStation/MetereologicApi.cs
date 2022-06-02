using System;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Newtonsoft.Json.Linq;

using CompanyGetStoreLink;

namespace Metereologic
{
    /// <summary>
    /// 
    /// </summary>
    public class MeteostatApi
    {
        public static bool GetNearbyStations(List<Store> stores, string apiKey, int apiSingleStationRequestLimit, int apiSleepTimeSecondsBetweenFailure, out List<Store> storesFilledWithStationId, out HashSet<Station> stations, out string errorMessage)
        {
            stations = new HashSet<Station>();
            storesFilledWithStationId = new List<Store>();
            errorMessage = string.Empty;

            foreach (Store store in stores)
            {
                string latitudeString = store.Location.Latitude.ToString().Replace(",", ".");
                string longitudeString = store.Location.Longitude.ToString().Replace(",", ".");

                Station newStation;
                Store newStore = store;
                if (GetNearbyStation(latitudeString, longitudeString, apiKey, apiSingleStationRequestLimit, apiSleepTimeSecondsBetweenFailure, out newStation, out errorMessage))
                {
                    stations.Add(newStation);
                    newStore.MetereologyStationID = newStation.Id;
                }

                storesFilledWithStationId.Add(store);
            }

            if (stations.Count == 0)
            {
                errorMessage = "No station was found"; // TODO: Resource
                return false;
            }

            return true;
        }

        private static bool GetNearbyStation(string locationLatitude, string locationLongitude, string apiKey, int apiSingleStationRequestLimit, int apiSleepTimeSecondsBetweenFailure, out Station station, out string errorMessage)
        {
            errorMessage = string.Empty;
            station = null;
            int triesCounter = 0;

            while (triesCounter < apiSingleStationRequestLimit)
            {
                triesCounter++;
                try
                {
                    station = ApiRequestNearbyStation(locationLatitude, locationLongitude, apiKey);
                    break;
                }
                catch (HttpRequestException ex)
                {
                    
                    Thread.Sleep(apiSleepTimeSecondsBetweenFailure * 1000);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }

            if (station == null)
            {
                errorMessage = "Unable to get information about the nearest station";
                return false;
            }

            return true;
        }

        private static Station ApiRequestNearbyStation(string latitude, string longitude, string apiKey)
        {
            if (string.IsNullOrEmpty(latitude))
            {
                throw new ArgumentException($"'{nameof(latitude)}' cannot be null or empty.", nameof(latitude));
            }

            if (string.IsNullOrEmpty(longitude))
            {
                throw new ArgumentException($"'{nameof(longitude)}' cannot be null or empty.", nameof(longitude));
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException($"'{nameof(apiKey)}' cannot be null or empty.", nameof(apiKey));
            }

            string id = string.Empty;
            string name = string.Empty;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-RapidAPI-Key", apiKey); //Setting na dll Class Settings
            string content = client.GetStringAsync(String.Format(MateriallogicSetting.Url, latitude, longitude)).Result;
            JObject json = JObject.Parse(content);

            if (json == null)
            {
                return null;
            }

            if (json["data"][0]["id"].Type != JTokenType.Null)
            {
                id = json["data"][0]["id"].ToString();
            }

            if (json["data"][0]["name"]["en"].Type != JTokenType.Null)
            {
                name = json["data"][0]["name"]["en"].ToString();
            }           

            if (name == string.Empty || id == string.Empty)
            {
                return null;
            }

            return new Station(id, name);
        }

        public static bool GetMeteorologicDataForStations(HashSet<Station> stations, DateTime start, DateTime end, string apiKey, int apiSingleStationRequestLimit, int apiSleepTimeSecondsBetweenFailure, out List<Station> stationsWithData, out string errorMessage)
        {
            stationsWithData = new List<Station>();
            errorMessage = string.Empty;

            int numDays = DaysCounter(start, end);
            string fixedFormatDateFromMeteostatApi = "yyyy-MM-dd";
            string startDateString = start.ToString(fixedFormatDateFromMeteostatApi);
            string endDateString = end.ToString(fixedFormatDateFromMeteostatApi);

            foreach (Station station in stations)
            {
                Dictionary<DateTime, Meteorology> meteorologicData;

                if (GetMeteorologicDataForStation(station.Id, startDateString, endDateString, numDays, apiKey, apiSingleStationRequestLimit, apiSleepTimeSecondsBetweenFailure, out meteorologicData))
                {
                    station.MeteorologyData = meteorologicData;
                }
                stationsWithData.Add(station);
            }

            return true;
        }

        private static bool GetMeteorologicDataForStation(string stationId, string startDateString, string endDateString, int numDays, string apiKey, int apiSingleStationRequestLimit, int apiSleepTimeSecondsBetweenFailure, out Dictionary<DateTime, Meteorology> meteorologicData)
        {
            meteorologicData = new Dictionary<DateTime, Meteorology>();

            int triesCounter = 0;

            while (triesCounter < apiSingleStationRequestLimit)
            {
                try
                {
                    meteorologicData = GetDataMeteo(stationId, startDateString, endDateString, numDays, apiKey);
                    break;
                }
                catch (HttpRequestException ex)
                {
                    triesCounter++;
                    Thread.Sleep(apiSleepTimeSecondsBetweenFailure * 1000);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            if (meteorologicData.Count == 0)
            {
                return false;
            }

            return true;
        }

        private static Dictionary<DateTime, Meteorology> GetDataMeteo(string stationId, string startDateString, string endDateString, int numDays, string apiKey)
        {
            Dictionary<DateTime, Meteorology> meteorologicData = new Dictionary<DateTime, Meteorology>();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-RapidAPI-Key", apiKey);
            string content = client.GetStringAsync($"https://meteostat.p.rapidapi.com/stations/hourly?station={stationId}&start={startDateString}&end={endDateString}").Result;
            JObject json = JObject.Parse(content);
            
            DateTime startDate = DateTime.ParseExact(startDateString, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            for (int dayIndex = 0; dayIndex < numDays; dayIndex++)
            {
                for (int hourIndex = 0; hourIndex < 24; hourIndex++)
                {
                    Meteorology meteorology;
                    if (ParseApiResponseMeteorologicData(json, dayIndex, hourIndex, out meteorology))
                    {
                        meteorologicData[startDate.AddDays(dayIndex).AddHours(hourIndex)] = meteorology;
                    }
                }
            }

            return meteorologicData;
        }

        private static bool ParseApiResponseMeteorologicData(JObject json, int dateIndex, int hourIndex, out Meteorology meteorologicData)
        {
            meteorologicData = new Meteorology();
            int jsonDataIndex = dateIndex * 24 + hourIndex;

            if (json["data"][jsonDataIndex]["time"].Type == JTokenType.Null)
            {
                return false;
            }

            if (json["data"][jsonDataIndex]["temp"].Type != JTokenType.Null)
            {
                meteorologicData.Temperature = float.Parse(json["data"][jsonDataIndex]["temp"].ToString());
            }

            if (json["data"][jsonDataIndex]["dwpt"].Type != JTokenType.Null)
            {
                meteorologicData.DewPoint = float.Parse(json["data"][jsonDataIndex]["dwpt"].ToString());
            }

            if (json["data"][jsonDataIndex]["rhum"].Type != JTokenType.Null)
            {
                meteorologicData.Humidity = float.Parse(json["data"][jsonDataIndex]["rhum"].ToString());
            }

            if (json["data"][jsonDataIndex]["prcp"].Type != JTokenType.Null)
            {
                meteorologicData.Precipitation = float.Parse(json["data"][jsonDataIndex]["prcp"].ToString());
            }

            if (json["data"][jsonDataIndex]["snow"].Type != JTokenType.Null)
            {
                meteorologicData.Snow = float.Parse(json["data"][jsonDataIndex]["snow"].ToString());
            }

            if (json["data"][jsonDataIndex]["wdir"].Type != JTokenType.Null)
            {
                meteorologicData.WindDirection = float.Parse(json["data"][jsonDataIndex]["wdir"].ToString());
            }

            if (json["data"][jsonDataIndex]["wspd"].Type != JTokenType.Null)
            {
                meteorologicData.WindSpeed = float.Parse(json["data"][jsonDataIndex]["wspd"].ToString());
            }

            if (json["data"][jsonDataIndex]["wpgt"].Type != JTokenType.Null)
            {
                meteorologicData.WindPeakGust = float.Parse(json["data"][jsonDataIndex]["wpgt"].ToString());
            }

            if (json["data"][jsonDataIndex]["pres"].Type != JTokenType.Null)
            {
                meteorologicData.Pressure = float.Parse(json["data"][jsonDataIndex]["pres"].ToString());
            }

            if (json["data"][jsonDataIndex]["tsun"].Type != JTokenType.Null)
            {
                meteorologicData.TotalSunshineTime = float.Parse(json["data"][jsonDataIndex]["tsun"].ToString());
            }

            return true;
        }

        private static int DaysCounter(DateTime start, DateTime end)
        {
            return Convert.ToInt32((end - start).TotalDays) + 1;
        }
    }
}
