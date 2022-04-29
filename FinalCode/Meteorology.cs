namespace app {
public class Meteorology {
    private Store store = new Store("");
    private DateTime date;
    private float temperature = -1;
    private float dewPoint = -1;
    private float humidity = -1;
    private float windSpeed = -1;
    private float pressure = -1;
    private float precipitation = -1;

    private static List<string> validDateFormats = new List<string>() 
    {
        "dd-mm-yyyy", 
        "dd/mm/yyyy", 
        "mm-dd-yyyy", 
        "mm/dd/yyyy", 
        "yyyy-mm-dd", 
        "yyyy/mm/dd", 
        "yyyy-dd-mm", 
        "yyyy/dd/mm"
    };
    private static IDictionary<int, string> monthNames = new Dictionary<int, string>() { 
        {1, "January"},
        {2, "February"},
        {3, "March"},
        {4, "April"},
        {5, "May"},
        {6, "June"},
        {7, "July"},
        {8, "August"},
        {9, "September"},
        {10, "October"},
        {11, "November"},
        {12, "December"}
    };
    public Meteorology(Store store) {
        this.store = store;
    }

    public string getDate(string format="dd/MM/yyyy") {
        if (!validDateFormats.Contains(format)) return "Invalid date format!";
        return date.ToString(format);
    }

    public string getDay() {
        return this.date.Day.ToString();
    }

    public string getMonth() {
        return monthNames[this.date.Month];
    }

    public string getYear() {
        return this.date.Year.ToString();
    }

    public float getTemperature() {
        return this.temperature;
    }

    public float getDewPoint() {
        return this.dewPoint;
    }

    public float getHumidity() {
        return this.humidity;
    }

    public float getWindSpeed() {
        return this.windSpeed;
    }

    public float getPressure() {
        return this.pressure;
    }

    public float getPrecipitation() {
        return this.precipitation;
    }

    public bool setDate(string dateString) {
        try {
            date = DateTime.ParseExact(dateString + " 00:00 am", "dd-MM-yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
        }
        catch (FormatException) {
            return false;
        }
        return true;
    }

    public void setTemperature(float temperature) {
        this.temperature = temperature;
    }

    public void setdewPoint(float dewPoint) {
        this.dewPoint = dewPoint;
    }

    public void setHumidity(float humidity) {
        this.humidity = humidity;
    }

    public void setWindApeed(float windSpeed) {
        this.windSpeed = windSpeed;
    }

    public void setPressure(float pressure) {
        this.pressure = pressure;
    }

    public void setPrecipitation(float precipitation) {
        this.precipitation = precipitation;
    }

    public void showInfo() {
        Console.WriteLine("Day: " + date.ToString("dd/MM/yyyy"));
        store.showInfo();
        Console.WriteLine("Temperature: " + temperature);
        Console.WriteLine("Dew Point: " + dewPoint);
        Console.WriteLine("Humidity: " + humidity);
        Console.WriteLine("Wind Speed: " + windSpeed);
        Console.WriteLine("Pressure: " + pressure);
        Console.WriteLine("Precipitation: " + precipitation);
    }

    public string getValues() {
        string text = "";
        text += "Temperature: " + temperature + " / ";
        text += "Dew Point: " + dewPoint + " / ";
        text += "Humidity: " + humidity + " / ";
        text += "Wind Speed: " + windSpeed + " / ";
        text += "Pressure: " + pressure + " / ";
        text += "Precipitation: " + precipitation;
        return text;
    }
}
}