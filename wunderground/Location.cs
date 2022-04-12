namespace app {
public class Location {
    private string city = "";
    private string district = "";
    private string country = "";
    public Location(string city, string district, string country) {
        this.city = city;
        this.district = district;
        this.country = country;
    }
    public string get_location() {
        return city + ", " + district + ", " + country;
    }
    public void show_info() {
        Console.WriteLine("Location: " + city + ", " + district + ", " + country);
    }
}
}