using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace app
{
    class StoreLink
    {
        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////  DLL STORE LINK  //////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<int, string> keyWordsWeightPT = new Dictionary<int, string>() {
            {1, "loj"},
            {2, "contact"},
        };
        private static Dictionary<int, string> keyWordsWeightEN = new Dictionary<int, string>() {
            {1, "shop"},
            {2, "contact"},
        };
        private static Dictionary<string, Dictionary<int, string>> keyWordsWeight = new Dictionary<string, Dictionary<int, string>>() {
            {"pt", keyWordsWeightPT},
            {"es", keyWordsWeightEN},
        };

        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////  DLL POSTAL CODES  //////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, Regex> postalCodesTemplates = new Dictionary<string, Regex>() {
            {"pt", new Regex("[1-9][0-9]{3}-[0-9]{3}")},
            {"es", new Regex("[0-9]{4}-[0-9]{3}")},
        };
        static public void Main(string[] args)
        {
            Company company = new Company("radio popular");
            getStoresPostalCodes(ref company);
            getStoresLocation(ref company);


            foreach(Store store in company.getStores()) {
                store.showInfo();
            }
        }

        public static void getStoresLocation(ref Company company)
        {
            List<Store> stores = company.getStores();
            List<Store> storesFilled = new List<Store>();

            foreach (Store store in stores)
            {
                Store storeFilledToAdd = fillStoreInformation(store);
                storesFilled.Add(storeFilledToAdd);
            }

            company.setStores(storesFilled);
        }
        public static Store fillStoreInformation(Store store)
        {
            var client = new HttpClient();
            string key = "de744d20-bb62-11ec-bc34-ab9b8c173a58";

            string storePostalCode = store.getPostalCode();

            string content = client.GetStringAsync($"https://app.zipcodebase.com/api/v1/search?apikey={key}&codes={storePostalCode}").Result;

            JObject json = JObject.Parse(content);

            if (json["query"] == null || json["query"].Count() == 0)
            {
                return store;
            }
            try
            {
                string latitude = json["results"][storePostalCode][0]["latitude"].ToString();
                string longitude = json["results"][storePostalCode][0]["longitude"].ToString();

                store.setLocation(new Location(float.Parse(latitude), float.Parse(longitude)));

                string city = json["results"][storePostalCode][0]["city"].ToString();
                string country = json["results"][storePostalCode][0]["country_code"].ToString();
                string state = json["results"][storePostalCode][0]["state"].ToString();
                string province = json["results"][storePostalCode][0]["province"].ToString();

                store.fillStoreInformation(city, country, state, province);
            }
            catch (System.ArgumentException e)
            {
                return store;
            }

            return store;
        }


        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////  DLL POSTAL CODES  //////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////

        public static void getStoresPostalCodes(ref Company company)
        {
            List<Store> stores = new List<Store>();

            IWebDriver driver = initializeDriver(".", false);

            getCompanyInformation(driver, ref company);

            company.showInfo();

            string storeInformationLink = company.getStoresInformationLink();
            if (storeInformationLink == "")
            {
                return;
            }
            driver.Navigate().GoToUrl(storeInformationLink);

            List<string> zipCodes = getPostalCodes(driver);

            foreach (string zipCode in zipCodes)
            {
                Console.WriteLine("postalCodeFound");
                Store storeToAdd = new Store(zipCode);
                stores.Add(storeToAdd);
                Console.WriteLine(zipCode);
            }

            closeDriver(driver);

            company.setStores(stores);
            return;
        }

        public static List<string> getPostalCodes(IWebDriver driver)
        {
            //string fullPageHtml = driver.FindElement(By.TagName("body")).Text;
            string fullPageHtml = driver.PageSource;
            List<string> zipcodes = new List<string> { };

            Regex postalCodeRegex = getPostalCodesTemplate("pt");
            if (postalCodeRegex == null)
            {
                return zipcodes; //return the empty list
            }
            
            //add zipcodes found using regex excluding repeated values
            foreach (Match match in postalCodeRegex.Matches(fullPageHtml))
            {
                if (!zipcodes.Contains(match.Value))
                {
                    zipcodes.Add(match.Value);
                }
            }

            return zipcodes;
        }

        public static Regex getPostalCodesTemplate(string country)
        {
            if (!postalCodesTemplates.ContainsKey(country))
            {
                return null;
            }

            return postalCodesTemplates[country];
        }



        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////  DLL STORE LINK  //////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////


        private static List<Company> getCompaniesInformation(IWebDriver driver, List<string> companiesName)
        {
            List<Company> companiesInformations = new List<Company>();

            foreach (string companyName in companiesName)
            {
                Company companyToAdd = new Company(companyName);
                getCompanyInformation(driver, ref companyToAdd);
                companiesInformations.Add(companyToAdd);
            }

            return companiesInformations;
        }
        private static void getCompanyInformation(IWebDriver driver, ref Company company)
        {            
            string companyName = company.getCompanyName();
            if (companyName == "")
            {
                return;
            }

            doGoogleSearch(driver, companyName);

            string companyWebPageHtml = getAllGoogleSearchLinks(driver).ElementAt(0);
            company.setCompanyWebPageHtml(companyWebPageHtml);
            driver.Navigate().GoToUrl(companyWebPageHtml);

            Dictionary<int, string> keyWords = getKeyWords();

            Dictionary<IWebElement, int> storeLinks = getStoresLinks(driver, keyWords);

            string storeLink = getBestStoreLink(storeLinks);
            if (storeLink == null)
            {
                closeDriver(driver);
                return;
            }

            company.setStoresInformationLink(storeLink);

            return;
        }

        public static Dictionary<IWebElement, int> getStoresLinks(IWebDriver driver, Dictionary<int, string> keyWords)
        {
            List<IWebElement> allLinks = driver.FindElements(By.TagName("a")).ToList();

            Dictionary<IWebElement, int> usefulLinks = new Dictionary<IWebElement, int> { };
            int linkTextWeight = -1;

            foreach (IWebElement link in allLinks)
            {
                try
                {
                    if (String.IsNullOrEmpty(link.Text))
                    {
                        continue;
                    }

                    if (link.Displayed && link.Enabled)
                    {
                        linkTextWeight = correctKeyWord(keyWords, link.Text);
                        if (linkTextWeight > 0) {
                            usefulLinks.Add(link, linkTextWeight);
                        }
                    }
                }
                catch (OpenQA.Selenium.StaleElementReferenceException)
                {
                    continue;
                }
            }

            return usefulLinks;
        }

        public static Dictionary<int, string> getKeyWords()
        {
            return keyWordsWeightPT; //TO DO: find a way of knowing the website language to use keyWordsWeight in order to get the right country
        }

        public static int correctKeyWord(Dictionary<int, string> keyWords, String word)
        {
            if (String.IsNullOrEmpty(word))
            {
                return -1;
            }

            word = word.ToLower(); // to be compared in lower case

            foreach (KeyValuePair<int, string> keyWord in keyWords)
            {
                if (word.Contains(keyWord.Value))
                {
                    return keyWord.Key;
                }
            }

            return -1;
        }

        public static string getBestStoreLink(Dictionary<IWebElement, int> storeLinks)
        {
            if (storeLinks.Count == 0) return null;

            IOrderedEnumerable<KeyValuePair<IWebElement, int>> sortedStoreLinks = from entry in storeLinks orderby entry.Value ascending select entry;
            List<IWebElement> bestsStoreLinksBasedOnText = new List<IWebElement>();
            int lowestValue = sortedStoreLinks.ElementAt(0).Value; //the lowest value is equivalent to the first entry in the sorted dictionary

            foreach (KeyValuePair<IWebElement, int> storeLink in sortedStoreLinks)
            {
                if (storeLink.Value > lowestValue)
                {
                    break;
                }
                bestsStoreLinksBasedOnText.Add(storeLink.Key);

            }

            if (bestsStoreLinksBasedOnText.Count() == 1) return bestsStoreLinksBasedOnText.ElementAt(0).GetAttribute("href");

            List<string> bestsStoreLinksBasedOnHref = new List<string>();
            foreach (IWebElement link in bestsStoreLinksBasedOnText)
            {
                Dictionary<int, string> keyWords = getKeyWords();

                if (correctKeyWord(keyWords, link.GetAttribute("href")) > 0)
                {
                    if (!bestsStoreLinksBasedOnHref.Contains(link.GetAttribute("href")))
                    {
                        bestsStoreLinksBasedOnHref.Add(link.GetAttribute("href"));
                    }
                }
            }

            if (bestsStoreLinksBasedOnHref.Count() == 1) return bestsStoreLinksBasedOnHref.ElementAt(0);

            int lowestSizeLink = 9999;
            string bestLink = "";
            foreach (string link in bestsStoreLinksBasedOnHref)
            {
                if(link.Count() < lowestSizeLink) {
                    lowestSizeLink = link.Count();
                    bestLink = link;
                }
            }

            return (bestLink == "") ? null : bestLink;
        }



        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        ///////////////////////  DLL CHROME DRIVER  //////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////
        private static IWebDriver initializeDriver(string chromeDriverPath, bool headless)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            if (headless) chromeOptions.AddArgument("headless"); //if headless true means we want the driver to work without opening the browser
                                                                 //chromeOptions.AddArgument("user-data-dir=chrome_profile"); //using a chrome user profile dont need to accept again cookies
            chromeOptions.AddArguments("--lang=es");
            chromeOptions.AddArgument("incognito");

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
                if (!checkGoogleCookies(driver))
                {
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
            try
            {
                if (cookieButton.Displayed && cookieButton.Enabled)
                {
                    cookieButton.Click();
                }
                else return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }

            return true;
        }

        public static List<string> getAllGoogleSearchLinks(IWebDriver driver)
        {
            List<IWebElement> results = driver.FindElements(By.XPath("//div[@class='yuRUbf']/a")).ToList();

            List<string> links = new List<string> { };
            results.ForEach(item => links.Add(item.GetAttribute("href")));

            return links;
        }
    }
}