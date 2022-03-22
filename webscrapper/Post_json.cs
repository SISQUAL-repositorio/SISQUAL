using Newtonsoft.Json;

namespace utils 
{
    public class Post_json
    {
        public  string _id = "webscraper-io-landing";
        public string startUrl = "http://webscraper.io/";
        public List<Selector> selectors = new List<Selector> {};

    public Post_json(String _id, String startUrl, List<Selector> selectors)
    {
        this._id = _id;
        this.startUrl = startUrl;
        this.selectors = selectors;
    }

    public string getJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
}