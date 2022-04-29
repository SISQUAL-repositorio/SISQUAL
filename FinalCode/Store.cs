namespace app
{
    public class Store
    {
        private string postalCode = new string("");
        private Location location = new Location();
        private string city = new string("");
        private string country = new string("");
        private string state = new string("");
        private string province = new string("");
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
    }
}