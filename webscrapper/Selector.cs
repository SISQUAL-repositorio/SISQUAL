using Newtonsoft.Json;

namespace utils 
{
    public class Selector
    {
    public List<string> parentSelectors = new List<string> {"_root"};
    public string type = "SelectorText";
    public bool multiple = false;
    public string id = "title";
    public string selector = "h1";
    public string regex = "";
    public string delay = "";

    public Selector(List<string> parentSelectors, String type, Boolean multiple, String id, String selector, String regex, String delay)
    {
        this.parentSelectors = parentSelectors;
        this.type = type;
        this.multiple = multiple;
        this.id = id;
        this.selector = selector;
        this.regex = regex;
        this.delay = delay;
    }

    public string getJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
}