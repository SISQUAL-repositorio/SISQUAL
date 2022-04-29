using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace ProjetoSisqualDB
{
    internal class Program
    {
        static void Main(string[] agrs)
        {
            webLinkRetrieve();
            Console.ReadLine();
            
        }
        
        private static void webLinkRetrieve()
        {

            //User input best method (directly search from the url section)
            Console.Write("What are you looking for? -> ");
            string inputSearch = Console.ReadLine();
            inputSearch.Trim().Replace(" ", "+");


            

            //Create option for headless mode
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("incognito");
            

            //Initialize Driver (on Headlessmode add "option" before created on ChromeDriver)
            IWebDriver driver = new ChromeDriver(@"C:\Users\Pedro.Costa\Desktop\ChromeDriver",option);
            driver.Navigate().GoToUrl($"https://google.com/search?q={inputSearch}");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();
            Console.WriteLine("Searching for "+ inputSearch + "...");

            //Google terms Accept
            IWebElement termsAgree = driver.FindElement(By.XPath("//*[@id='L2AGLb']"));
            termsAgree.Click();
            Console.WriteLine("Accepting google cookies...");

            //VERIFY ADDS
            //CLICK ON 1ST RESULT THATS NOT AN ADD
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            IWebElement results = driver.FindElement(By.ClassName("iUh30"));
            Console.WriteLine("Verifying results (adds excluded)...");
            results.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            //Next method
            cookieAccept(driver);

        }

        private static void cookieAccept(IWebDriver driver)
        {

            Console.WriteLine("Searching for cookies policy...");
            //This method consists in looking for Cookies Policy and accept them
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(8);
            List<IWebElement> possibleButtons = driver.FindElements(By.XPath("//button[contains(text(),'Aceitar')]")).ToList();
            List<IWebElement> possibleButtonsEnglish = driver.FindElements(By.XPath("//button[contains(text(),'Agree')]")).ToList();
            List<IWebElement> possibleButtonsSpanish = driver.FindElements(By.XPath("//button[contains(text(),'Aceptar')]")).ToList();

            possibleButtonsEnglish.ForEach(item => possibleButtons.Add(item));
            possibleButtonsSpanish.ForEach(item => possibleButtons.Add(item));

            foreach (IWebElement button in possibleButtons)
            {
                try
                {
                    button.Click();
                    break;
                }
                catch (ElementNotInteractableException)
                {
                    continue;
                }
            }

            //Retrieve link
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(4);
            string link = driver.Url;

            //Next method
            getData(link, driver);

        }

        private static void getData(string link, IWebDriver driver)
        {
            Console.WriteLine("Retrieving source of the current website...");
            List<IWebElement> possibleBtnMap = driver.FindElements(By.CssSelector("a[href*='loja']")).ToList();

            // page URL
            string pageURL = driver.Url;

            foreach (IWebElement button in possibleBtnMap)
            {
                try
                {
                    button.Click();
                    break;
                }
                catch (ElementNotInteractableException)
                {
                    continue;
                }
            }

            // page URL after clicking on stores
            string pageURLafter = driver.Url;

            if (pageURL == pageURLafter)
            {


            }


            //Source HTML of the page 
            string source = driver.PageSource;


            //Next function
            getLocationDataVersion2(source,pageURLafter);
            //getLocationData(source, pageURL);

            //Driver close
            Console.WriteLine("Closing driver...");
           // driver.Close();
        }
        private static void getLocationDataVersion2(string source, string pageURL)
        {
            Console.WriteLine("Searching for locations in the current website...");
            
            var srcStr = source.ToString();

            //Regex -> sequence of 4 numbers (0-9) followed by "-" and another 3 numbers (0-9)
            var regex = new Regex("[0-9]{4}-[0-9]{3}");  //REGEX PT VER PARA OUTROS PAISES

            //Results count
            var resultsCount = 0;

            List<String> zipcodes = new List<String> { };

            //add zipcodes found using regex excluding repeated values
            foreach (Match match in regex.Matches(srcStr))
            {
                if (!zipcodes.Contains(match.Value))
                {
                    zipcodes.Add(match.Value);
                    resultsCount += 1;
                }
            }
     
            //Uso assim para cada zipcode na lista o qe fica mais lento. P voltar ao normal fazer getStoreData(zipcodes) e alterar a funçao 
            //abaixo (substituir o code por codes)

            
            foreach (var zipcode in zipcodes)
            {
                getStoreData(zipcode);  
            }

            Console.WriteLine(resultsCount + " zipcodes were found. (Some might be repeated or invalid)");
            Console.WriteLine("Valid and unrepeated results were shown above!");
            Console.WriteLine("Page link: " + pageURL);
   
        }


        private static void getStoreData(string zipcode)
        {
            
            var client = new HttpClient();
            string key = "de744d20-bb62-11ec-bc34-ab9b8c173a58";

            //var codes = String.Join(",", zipcodes.ToArray());
            var code = zipcode;

                var content = client.GetStringAsync($"https://app.zipcodebase.com/api/v1/search?apikey={key}&codes={code}").Result;

                var json = JObject.Parse(content);

                if (json["query"].Count() == 0)
                {
                    return;
                }
                else
                {
                //var output = json["results"].ToString();
                //Console.WriteLine(output);
                    try
                    {
                        var city = json["results"][code][0]["city"].ToString();
                        var latitude = json["results"][code][0]["latitude"].ToString();
                        var longitude = json["results"][code][0]["longitude"].ToString();
                        var country = json["results"][code][0]["country_code"].ToString();
                        var postalCode = json["results"][code][0]["postal_code"].ToString();
                        var state = json["results"][code][0]["state"].ToString();
                        var province = json["results"][code][0]["province"].ToString();
                        Console.WriteLine("Store " + city + ":");
                        Console.WriteLine("Country: " + country);
                        Console.WriteLine("Latitude: " + latitude);
                        Console.WriteLine("Longitude: " + longitude);
                        Console.WriteLine("Postal Code: " + postalCode);
                        Console.WriteLine("State: " + state);
                        Console.WriteLine("Province: " + province);
                        Console.WriteLine("---------------------");
                    }
                    catch (System.ArgumentException ae)
                    {
                    
                    }
    
            }
                
        }

        private static void getLocationData(string source, string pageURL)
        {
            Console.WriteLine("Searching for locations in the current website...");
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(source);

            List<String> possible_classes_Keywords = new List<String> { "shop", "store" };
            List<String> possible_descendants = new List<String> { "div", "section" };

            //Find store divs
            List<HtmlNode> LocationsHtml = new List<HtmlNode>();

            
            foreach (var keyword in possible_classes_Keywords)
            {
                LocationsHtml = htmlDocument.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "")
               .Contains(keyword)).ToList();

                var validData = analyzeData(LocationsHtml);
                if (LocationsHtml.Count > 0 && validData== true)
                {
                    break;
                }
            }
            
            /*IEnumerable<HtmlNode> htmlNodes = from htmlNode in htmlDocument.DocumentNode.Descendants()
                                              where possible_descendants.Contains(htmlNode.Name) &&
                                              possible_classes_Keywords.Contains(htmlNode.GetAttributeValue("class", ""))
                                              select htmlNode;

            foreach (HtmlNode htmlNode in htmlNodes)
            {
                Console.WriteLine($"{htmlNode.Name} - {htmlNode.GetAttributeValue("class", "")}");
            }*/


            //Find where store info is located
            List<HtmlNode> LocationsItems = new List<HtmlNode>();
            foreach (var keyword in possible_classes_Keywords)
            {
                LocationsItems = LocationsHtml[0].Descendants("div")
               .Where(node => node.GetAttributeValue("class", "")
               .Contains(keyword)).ToList();
                if (LocationsItems.Count > 0)
                {
                    break;
                }
            }
            Console.Clear();
            Console.WriteLine("Displaying results:");

            // Get all the stores found
            foreach (var store in LocationsItems)
            {
                //Get all data from each location
                var storeData = store.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "")
               .Contains("")).ToList();

                //Separate data for each store
                foreach (var storeInfo in storeData)
                {
                    Console.WriteLine(storeInfo.GetAttributeValue("class", "") + ": " + storeInfo.InnerText);
                    
                }
                string longitude = "";
                string latitude = "";
                string adminArea = "";
                getGeoLocation(storeData[1].InnerText, ref longitude, ref latitude , ref adminArea);
                Console.WriteLine("Longitude: " + longitude + "\nLatitude: " + latitude + "\nAdministrative area: " + adminArea);
                Console.WriteLine();
              
            }
            Console.WriteLine("Page link: "+pageURL);
            
            //Console.WriteLine("Latitude_" + teste);
            

        }

        private static bool analyzeData(List<HtmlNode> List)
        {

            List<HtmlNode> usefullData = new List<HtmlNode>();
            foreach (var location in List)
            {
                if (location.InnerHtml.Contains("address"))
                {
                    return true;
                }
                return false;
            }
            return true;


        }
       
        private static void getGeoLocation(string store, ref string latitude, ref string longitude, ref string adminArea)
        {
            /*string address = "123 something st, somewhere";
            string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(address), "AIzaSyCquCVln1np30yTr9HObpNmZC5fVBMgN9c");

            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();
            XDocument xdoc = XDocument.Load(response.GetResponseStream());

            XElement result = xdoc.Element("GeocodeResponse").Element("result");
            XElement locationElement = result.Element("geometry").Element("location");
            XElement lat = locationElement.Element("lat");
            XElement lng = locationElement.Element("lng");

            Console.WriteLine(lat);*/

            var client = new HttpClient();
            string key = "26d930d54271f6ca02183adb2b5a2133";

         
            store.Trim().Replace(" ", "%20");

            if (store != "")
            {
                var paramenters = $"?access_key={key}&query={store}&country=PT&fields&limit=1";
                var content = client.GetStringAsync($"http://api.positionstack.com/v1/forward{paramenters}").Result;

                var json = JObject.Parse(content);

                if (json["data"].Count() == 0)
                {
                    return;
                }

                latitude = json["data"][0]["latitude"].ToString();
                longitude = json["data"][0]["longitude"].ToString();
                adminArea = json["data"][0]["administrative_area"].ToString();
            }
          
        }

    }
}
