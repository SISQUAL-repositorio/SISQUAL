namespace app {
public class Meteorology {
    private Location location = new Location("", "", "");
    private DateTime date;
    private float temperature = -1;
    private float dew_point = -1;
    private float humidity = -1;
    private float wind_speed = -1;
    private float pressure = -1;
    private float precipitation = -1;

    private static List<string> valid_date_formats = new List<string>() 
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
    private static IDictionary<int, string> month_names = new Dictionary<int, string>() { 
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
    public Meteorology() {}

    public string get_location() {
        return location.get_location();
    }

    public string get_date(string format="dd/MM/yyyy") {
        if (!valid_date_formats.Contains(format)) return "Invalid date format!";
        return date.ToString(format);
    }

    public string get_day() {
        return this.date.Day.ToString();
    }

    public string get_month() {
        return month_names[this.date.Month];
    }

    public string get_year() {
        return this.date.Year.ToString();
    }

    public float get_temperature() {
        return this.temperature;
    }

    public float get_dew_point() {
        return this.dew_point;
    }

    public float get_humidity() {
        return this.humidity;
    }

    public float get_wind_speed() {
        return this.wind_speed;
    }

    public float get_pressure() {
        return this.pressure;
    }

    public float get_precipitation() {
        return this.precipitation;
    }

    public void set_location(Location location) {
        this.location = location;
    }

    public bool set_date(string date_str) {
        try {
            date = DateTime.ParseExact(date_str + " 00:00 am", "dd-MM-yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
        }
        catch (FormatException) {
            return false;
        }
        return true;
    }

    public void set_temperature(float temperature) {
        this.temperature = temperature;
    }

    public void set_dew_point(float dew_point) {
        this.dew_point = dew_point;
    }

    public void set_humidity(float humidity) {
        this.humidity = humidity;
    }

    public void set_wind_speed(float wind_speed) {
        this.wind_speed = wind_speed;
    }

    public void set_pressure(float pressure) {
        this.pressure = pressure;
    }

    public void set_precipitation(float precipitation) {
        this.precipitation = precipitation;
    }

    public void show_info() {
        Console.WriteLine("Day: " + date.ToString("dd/MM/yyyy"));
        location.show_info();
        Console.WriteLine("Temperature: " + temperature);
        Console.WriteLine("Dew Point: " + dew_point);
        Console.WriteLine("Humidity: " + humidity);
        Console.WriteLine("Wind Speed: " + wind_speed);
        Console.WriteLine("Pressure: " + pressure);
        Console.WriteLine("Precipitation: " + precipitation);
    }

    public string get_values() {
        string text = "";
        text += "Temperature: " + temperature + " / ";
        text += "Dew Point: " + dew_point + " / ";
        text += "Humidity: " + humidity + " / ";
        text += "Wind Speed: " + wind_speed + " / ";
        text += "Pressure: " + pressure + " / ";
        text += "Precipitation: " + precipitation;
        return text;
    }
}
}