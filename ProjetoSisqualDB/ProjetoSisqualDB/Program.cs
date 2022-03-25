using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using System.Net;
using System.Threading;

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
            Console.Write("O que pretende encontrar? -> ");
            string inputPesquisa = Console.ReadLine();
            inputPesquisa.Trim().Replace(" ", "+");

            //Create option for headless mode
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("headless");

            //saddsad
            //Initialize Driver (on Headlessmode add "option" before created on ChromeDriver)
            IWebDriver driver = new ChromeDriver(@"C:\Users\Pedro.Costa\Desktop\ChromeDriver",option);
            driver.Navigate().GoToUrl($"https://google.com/search?q={inputPesquisa}");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();

            //Google terms Accept
            IWebElement termsAgree = driver.FindElement(By.XPath("//*[@id='L2AGLb']"));
            termsAgree.Click();

            //VERIFY ADDS
            //CLICK ON 1ST RESULT THATS NOT AN ADD
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            IWebElement results = driver.FindElement(By.ClassName("iUh30"));

            results.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            //Next method
            cookieAccept(driver);
           
        }

        private static void cookieAccept(IWebDriver driver)
        {
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

            //Source HTML of the page
            string source = driver.PageSource;
            getLocationData(source);

        }

        private static void getLocationData(string source)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(source);

            List<String> possible_classes_Keywords = new List<string>(new[] { "shops", "store" });

            //List<HtmlNode> LocationsHtml = new List<HtmlNode>();

            //Find store divs
        
                 var LocationsHtml = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => possible_classes_Keywords.Contains(node.GetAttributeValue("class", "")))
                .ToList();
            
            
            
            //Find where store info is located
            var LocationsItems = LocationsHtml[0].Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("shop")).ToList();


            //List<String> LocationData = new List<string>() ;
            Console.Clear();

            // Get all the stores found
            foreach (var store in LocationsItems)
            {       
                //Get all data from location
                 var storeData = store.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("")).ToList();

                //Separate data for each store
                foreach (var storeInfo in storeData)
                {
                    Console.WriteLine(storeInfo.GetAttributeValue("class","") + ":" + storeInfo.InnerText);
                }
                Console.WriteLine();
            }

            //Console.Clear();
            Console.WriteLine();
        }



    }
}
