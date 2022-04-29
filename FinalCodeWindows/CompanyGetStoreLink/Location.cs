using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyGetStoreLink
{
    public class Location
    {
        private float latitude = -1;
        private float longitude = -1;
        public Location(float latitude, float longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public Location() { }
        public float getLatitude()
        {
            return latitude;
        }

        public float getLongitude()
        {
            return longitude;
        }
        public void showInfo()
        { //just for debugging, delete when dll
            Console.WriteLine("Latitude: " + latitude + "\n" + "Longitude: " + longitude);
        }
    }
}
