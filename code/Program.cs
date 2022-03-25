using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;

using System.Collections.ObjectModel;

namespace app
{
class Program {
    static public void Main(string[] args)
    {    
        log("Initializing the chrome driver...");
        IWebDriver driver = initializeDriver(".", false); // init the chrome driver
        
        String search_word = "stradivarius";
        log("Doing a google search for: " + search_word);
        if (!doGoogleSearch(driver, search_word)) 
        {
            log("Something went wrong. Closing the driver...");
            closeDriver(driver);
            return;
        }

        String first_search_result = getAllGoogleSearchLinks(driver).ElementAt(0); //for now go directly to the first google search result
        log("The first search result was " + first_search_result);

        log("Navigating to " + first_search_result);
        driver.Navigate().GoToUrl(first_search_result);
        pause(driver, 2); // wait for all page to load to see if cookie pop-up opens

        if (cookiePopUpDetected(driver)) 
        {
            checkCookies(driver);
        }
        
        log("Trying to find stores link in the webpage...");
        List<String> keyWords = getKeyWords("shopKeyWords.json");
        String shops_page = getStoresLink(driver, keyWords);
        log("Found the stores link under the name :" + shops_page);

        log("Navigating to " + shops_page);
        driver.Navigate().GoToUrl(shops_page);

        log("Beginning to extract stores information");
        //string filename = "locations.json";
        //getAllStoresInformation(driver, filename);
    }

    private static IWebDriver initializeDriver(string chromeDriverPath, bool headless) 
    {
        ChromeOptions chromeOptions = new ChromeOptions();
        if (headless) chromeOptions.AddArgument("headless"); //if headless true means we want the driver to work without opening the browser
        //chromeOptions.AddArgument("user-data-dir=chrome_profile"); //using a chrome user profile dont need to accept again cookies
        chromeOptions.AddArguments("--lang=es");
    
        IWebDriver chromeDriver;
        chromeDriver = new ChromeDriver(chromeDriverPath, chromeOptions);
        chromeDriver.Manage().Window.Maximize(); //check if needed
        return chromeDriver;
    }

    private static void closeDriver(IWebDriver driver) 
    {
        driver.Close();
    }

    private static void pause(IWebDriver driver, int seconds) 
    {
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);
    }

    public static bool doGoogleSearch(IWebDriver driver, string searchStatement) 
    {
        driver.Navigate().GoToUrl("https://google.com");
        
        string currentHandle = driver.CurrentWindowHandle;
        
        ReadOnlyCollection<String> originalHandles = driver.WindowHandles;

        if (cookiePopUpDetected(driver)) 
        {
            if (!checkGoogleCookies(driver)) {
                return false;
            }
        }

        IWebElement search_bar = driver.FindElement(By.Name("q"));
        try 
        {
            if (search_bar.Displayed && search_bar.Enabled) 
            {
                search_bar.SendKeys(searchStatement + Keys.Enter);
            }
            else return false;
        }
        catch (StaleElementReferenceException) 
        {
            return false;
        }

        return true;
    }

    public static bool cookiePopUpDetected(IWebDriver driver) 
    {
        String full_page_html = driver.FindElement(By.TagName("body")).Text;
        
        if (String.IsNullOrEmpty(full_page_html)) 
        {
            return false;
        }

        return (full_page_html.Contains("cookie") || full_page_html.Contains("Cookie")); //if lower case "cookie" not detected, just to be sure check if uppercase "Cookie" is present
    }

    public static bool checkGoogleCookies(IWebDriver driver) //getting through google chrome cookie policy
    {
        IWebElement cookie_button = driver.FindElement(By.XPath("//div[contains(text(),'Aceit')]"));
        try {
            if (cookie_button.Displayed && cookie_button.Enabled) 
            {
                cookie_button.Click();
            }
            else return false;
        }
        catch (StaleElementReferenceException) {
            return false;
        }

        return true;
    }

    public static List<String> getAllGoogleSearchLinks(IWebDriver driver) 
    {
        List<IWebElement> results = driver.FindElements(By.XPath("//div[@class='yuRUbf']/a")).ToList();
        
        List<String> links = new List<String> {};
        results.ForEach(item => links.Add(item.GetAttribute("href")));

        return links;
    }

    public static void checkCookies(IWebDriver driver) 
    {
        List<IWebElement> possible_buttons = driver.FindElements(By.XPath("//button[contains(text(),'Aceit')]")).ToList();
        List<IWebElement> possible_buttons_english = driver.FindElements(By.XPath("//button[contains(text(),'Agree')]")).ToList();
        List<IWebElement> possible_buttons_espanhol = driver.FindElements(By.XPath("//button[contains(text(),'Aceptar')]")).ToList();
        
        possible_buttons_english.ForEach(item => possible_buttons.Add(item));
        possible_buttons_espanhol.ForEach(item => possible_buttons.Add(item));

        foreach (IWebElement button in possible_buttons)
        {
            try
            {
                if (button.Displayed && button.Enabled) {
                    button.Click();
                    break;
                }
            }
            catch (StaleElementReferenceException)
            {
                continue; //see if this is needed
            }
        }
    }

    public static String getStoresLink(IWebDriver driver, List<String> keyWords) 
    {
        List<IWebElement> all_links = driver.FindElements(By.TagName("a")).ToList();
        Console.WriteLine(all_links.Count);
                
        List<IWebElement> useful_links = new List<IWebElement> {};

        foreach (IWebElement link in all_links)
        {
            try
            {
                if (String.IsNullOrEmpty(link.Text)) 
                {
                    continue;
                }

                Console.WriteLine(link.Text);
                
                if (link.Displayed && link.Enabled && correctKeyWord(keyWords, link.Text)) 
                {
                    useful_links.Add(link);
                }
            }
            catch(OpenQA.Selenium.StaleElementReferenceException)
            {
                continue;
            }
        }

        return useful_links.ElementAt(0).GetAttribute("href"); //For now just ignore if more than one is found
    }

    public static List<String> getKeyWords(String filename) 
    {
        return JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(filename));
    }    

    public static void getAllStoresInformation(IWebDriver driver, String filename) 
    {
        List<IWebElement> stores_info = driver.FindElements(By.XPath("//body[contains(concat(' ', @class, ' '), ' store ')]")).ToList();
        //FindElements(By.Xpath("//div[contains(text(),'leaver')]")).ToList();

        Console.WriteLine(stores_info.Count);
        foreach (IWebElement store_info in stores_info) 
        {
            writeToFile(filename, store_info);
        }
    }

    public static void writeToFile(String filename, IWebElement info) 
    {
        File.WriteAllText(filename, info.Text);
        Console.WriteLine(info.Text);
    }

    public static bool correctKeyWord(List<String> keyWords, String word) 
    {
        if (String.IsNullOrEmpty(word)) 
        {
            return false;
        }

        word = word.ToLower(); // to be compared in lower case

        foreach (String keyWord in keyWords) 
        {
            if (word.Contains(keyWord))
            {
                return true;
            }
        }

        return false;
    }

    public static void log(String message) 
    {
        Console.WriteLine(message);
    }
}
}