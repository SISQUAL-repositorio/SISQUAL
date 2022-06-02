using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Meteorology
    {

        #region Properties

        private float temperature;
        private float dewPoint;
        private float humidity;
        private float precipitation;
        private float snow;
        private float windDirection;
        private float windSpeed;
        private float windPeakGust;
        private float pressure;
        private float totalSunshineTime;

        public float Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }

        public float DewPoint
        {
            get { return dewPoint; }
            set { dewPoint = value; }
        }

        public float Humidity
        {
            get { return humidity; }
            set { humidity = value; }
        }

        public float Precipitation
        {
            get { return precipitation; }
            set { precipitation = value; }
        }

        public float Snow
        {
            get { return snow; }
            set { snow = value; }
        }

        public float WindDirection
        {
            get { return windDirection; }
            set { windDirection = value; }
        }

        public float WindSpeed
        {
            get { return windSpeed; }
            set { windSpeed = value; }
        }

        public float WindPeakGust
        {
            get { return windPeakGust; }
            set { windPeakGust = value; }
        }

        public float Pressure
        {
            get { return pressure; }
            set { pressure = value; }
        }

        public float TotalSunshineTime
        {
            get { return totalSunshineTime; }
            set { totalSunshineTime = value; }
        }

        #endregion

        #region Constructors

        public Meteorology()
        {
            temperature = -1;
            dewPoint = -1;
            humidity = -1;
            precipitation = -1;
            snow = -1;
            windDirection = -1;
            windSpeed = -1;
            windPeakGust = -1;
            pressure = -1;
            totalSunshineTime = -1;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            string result = string.Empty;
            string noData = Properties.Resources.NoData;

            string temperatureString = (temperature == -1) ? noData : temperature.ToString() + " Cº";
            string dewPointString = (dewPoint == -1) ? noData : dewPoint.ToString() + " Cº";
            string humidityString = (humidity == -1) ? noData : humidity.ToString() + " %";
            string precipitationString = (precipitation == -1) ? noData : precipitation.ToString() + " millimeters";
            string snowString = (snow == -1) ? noData : snow.ToString() + " millimeters";
            string windDirectionString = (windDirection == -1) ? noData : windDirection.ToString() + " Degrees";
            string windSpeedString = (windSpeed == -1) ? noData : windSpeed.ToString() + " Km/h";
            string windPeakGustString = (windPeakGust == -1) ? noData : windPeakGust.ToString() + " Km/h";
            string pressureString = (pressure == -1) ? noData : pressure.ToString() + " hPa";
            string totalSunshineTimeString = (totalSunshineTime == -1) ? noData : totalSunshineTime.ToString() + " Minutes";

            result += "Average Temperature: " + temperatureString + "\n";
            result += "Dew Point: " + dewPointString + "\n";
            result += "Humidity: " + humidityString + "\n";
            result += "Precipitation: " + precipitationString + "\n";
            result += "Snow: " + snowString + "\n";
            result += "Wind Direction: " + windDirectionString + "\n";
            result += "Wind Speed: " + windSpeedString + "\n";
            result += "Wind Gust Peak: " + windPeakGustString + "\n";
            result += "Pressure: " + pressureString + "\n";
            result += "Daily Sunshine: " + totalSunshineTimeString + "\n";

            return result;
        }

        #endregion
    }
}
