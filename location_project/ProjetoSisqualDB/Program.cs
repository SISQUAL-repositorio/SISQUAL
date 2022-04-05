using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

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
            string inputPesquisa = Console.ReadLine();
            inputPesquisa.Trim().Replace(" ", "+");

            //Create option for headless mode
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("headless");


            //Initialize Driver (on Headlessmode add "option" before created on ChromeDriver)
            IWebDriver driver = new ChromeDriver(@"C:\Users\Pedro.Costa\Desktop\ChromeDriver");
            driver.Navigate().GoToUrl($"https://google.com/search?q={inputPesquisa}");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();
            Console.WriteLine("Searching for "+ inputPesquisa + "...");

            //Google terms Accept
            IWebElement termsAgree = driver.FindElement(By.XPath("//*[@id='L2AGLb']"));
            termsAgree.Click();
            Console.WriteLine("Accepting google cookies...");

            //VERIFY ADDS
            //CLICK ON 1ST RESULT THATS NOT AN ADD
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            IWebElement results = driver.FindElement(By.ClassName("iUh30"));
            Console.WriteLine("Verify results (adds excluded)...");
            results.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            //Next method
            cookieAccept(driver);

        }

        private static void cookieAccept(IWebDriver driver)
        {

            Console.WriteLine("Searching for cookies policy...");
            //This method consists in looking for Cookies Policy and accept them
            List<IWebElement> possible_buttons = driver.FindElements(By.XPath("//button[contains(text(),'Aceitar')]")).ToList();
            List<IWebElement> possible_buttons_english = driver.FindElements(By.XPath("//button[contains(text(),'Agree')]")).ToList();
            List<IWebElement> possible_buttons_espanhol = driver.FindElements(By.XPath("//button[contains(text(),'Aceptar')]")).ToList();

            possible_buttons_english.ForEach(item => possible_buttons.Add(item));
            possible_buttons_espanhol.ForEach(item => possible_buttons.Add(item));

            foreach (IWebElement button in possible_buttons)
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
            List<IWebElement> possible_BtnMap = driver.FindElements(By.CssSelector("a[href*='/lojas']")).ToList();

            foreach (IWebElement button in possible_BtnMap)
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
           

            //Source HTML of the page and page URL
            string pageURL = driver.Url;
            string source = driver.PageSource;
            getLocationData(source,pageURL);

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
                getGeoLocation(store.InnerText, ref longitude, ref latitude , ref adminArea);
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
            var paramenters = $"?access_key={key}&query={store}&country=PT&fields&limit=1";

            var content = client.GetStringAsync($"http://api.positionstack.com/v1/forward{paramenters}").Result;
           

            var json = JObject.Parse(content);

            if (json["data"].Count() == 0)
            {
                return;
            }
           
            latitude = json["data"][0]["latitude"].ToString();
            longitude = json["data"][0]["longitude"].ToString();
            adminArea= json["data"][0]["administrative_area"].ToString(); 

        }




    }
}
