using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using System.Collections.Generic;

namespace ChromeDriverUtils
{
class ChromeDriverUtils {
    static public void Main(string[] args)
    {    
        IWebDriver driver = initializeDriver(".", false); // init the chrome driver
        
        string search_word = "stradivarius";
        if (!doGoogleSearch(driver, search_word)) 
        {
            closeDriver(driver);
            return;
        }

        string first_search_result = getAllGoogleSearchLinks(driver)[0]; //for now go directly to the first google search result

        driver.Navigate().GoToUrl(first_search_result);
        pause(driver, 2); // wait for all page to load to see if cookie pop-up opens

        closeDriver(driver);
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
        driver.Manage().Timeouts().ImplicitWait = System.TimeSpan.FromSeconds(seconds);
    }

    public static bool doGoogleSearch(IWebDriver driver, string searchStatement) 
    {
        driver.Navigate().GoToUrl("https://google.com");

        if (cookiePopUpDetected(driver))
        {
            if (!checkGoogleCookies(driver)) {
                return false;
            }
        }

        IWebElement searchBar = driver.FindElement(By.Name("q"));
        try 
        {
            if (searchBar.Displayed && searchBar.Enabled) 
            {
                searchBar.SendKeys(searchStatement + Keys.Enter);
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
        string fullPageHtml = driver.FindElement(By.TagName("body")).Text;
        
        if (string.IsNullOrEmpty(fullPageHtml)) 
        {
            return false;
        }

        return (fullPageHtml.Contains("cookie") || fullPageHtml.Contains("Cookie")); //if lower case "cookie" not detected, just to be sure check if uppercase "Cookie" is present
    }

    public static bool checkGoogleCookies(IWebDriver driver) //getting through google chrome cookie policy
    {
        IWebElement cookieButton = driver.FindElement(By.XPath("//div[contains(text(),'Aceit')]"));
        try {
            if (cookieButton.Displayed && cookieButton.Enabled) 
            {
                cookieButton.Click();
            }
            else return false;
        }
        catch (StaleElementReferenceException) {
            return false;
        }

        return true;
    }

    public static List<string> getAllGoogleSearchLinks(IWebDriver driver) 
    {
        List<IWebElement> results = driver.FindElements(By.XPath("//div[@class='yuRUbf']/a")).ToList();
        
        List<string> links = new List<string> {};
        results.ForEach(item => links.Add(item.GetAttribute("href")));

        return links;
    }
}
}