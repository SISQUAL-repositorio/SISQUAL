using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyGetStoreLink
{
    public class Store
    {
        private string postalCode = "";
        private Location location = new Location();
        private string city = "";
        private string country = "";
        private string state = "";
        private string province = "";
        private short metereologyStationID = -1;
        public Store(string postalCode)
        {
            this.postalCode = postalCode;
        }
        public string getPostalCode()
        {
            return postalCode;
        }
        public float getLatitude()
        {
            return location.getLatitude();
        }
        public float getLongitude()
        {
            return location.getLongitude();
        }
        public void setLocation(Location location)
        {
            this.location = location;
        }
        public void setMetereologyStationID(short metereologyStationID)
        {
            this.metereologyStationID = metereologyStationID;
        }
        public void fillStoreInformation(string city, string country, string state, string province)
        {
            this.city = city;
            this.country = country;
            this.state = state;
            this.province = province;
        }
        public void showInfo()
        { //just for debugging, delete when dll
            Console.WriteLine("Postal Code: " + this.postalCode);
            location.showInfo();
            Console.WriteLine("city: " + city);
            Console.WriteLine("Country: " + country);
            Console.WriteLine("State: " + state);
            Console.WriteLine("Province: " + province);
            Console.WriteLine("---------------------");
        }

        public string toString()
        { //just for debugging, delete when dll
            string output = "";
            output += "Postal Code: " + this.postalCode + "\n";
            output += location.toString() + "\n";
            output += "city: " + city + "\n";
            output += "Country: " + country + "\n";
            output += "State: " + state + "\n";
            output += "Province: " + province + "\n";
            output += "---------------------" + "\n";
            return output;
        }
    }
}
