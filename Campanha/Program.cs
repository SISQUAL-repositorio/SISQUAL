using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using System.Net;


namespace Campanha
{
    class Program
    {
        static public void Main(string[] args)
        {

            System.Console.WriteLine("Search your store");
            string? user_input = System.Console.ReadLine();

            ChromeOptions options = new ChromeOptions();
            // options.AddArgument("headless");

            IWebDriver driver;
            driver = new ChromeDriver(".", options);
            driver.Manage().Window.Maximize();

            driver.Navigate().GoToUrl("https://google.com");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            // getting through google chrome cookie policy
            driver.FindElement(By.XPath("//div[text()='Aceito']")).Click();

            // key websites
            List<string> key_sites = new List<string> { "https://blog200", "https://www.ocacapromocoes" };

            // searching in google search bar
            driver.FindElement(By.Name("q")).SendKeys($"{user_input}" + Keys.Enter);
            user_input?.Replace(" ", "-");

            string google_source = driver.PageSource;
            var HtmlDocument_G = new HtmlDocument();
            HtmlDocument_G.LoadHtml(google_source);

            var get_all_page = HtmlDocument_G.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("id", "").Contains("center_col")).ToList();

            foreach (var item in get_all_page)
            {
                var get_links = HtmlDocument_G.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("href", "").Contains($"black-friday"));
                var get_links_no_duplicates = get_links.Distinct().ToList();

                var best_websites = HtmlDocument_G.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("href", "").Contains($"{key_sites[0]}")).ToList();
                var best_websites1 = HtmlDocument_G.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("href", "").Contains($"{key_sites[1]}")).ToList();
                foreach (var xyz in best_websites1)
                {
                    best_websites.Add(xyz);
                }
                var best_websites_no_duplicates = best_websites.Distinct().ToList();

                foreach (var item1 in get_links_no_duplicates)
                {

                    System.Console.WriteLine(item1.GetAttributeValue("href", ""));
                }

                if (best_websites_no_duplicates.Count != 0)
                {
                    System.Console.WriteLine("\nBest Websites");
                    foreach (var item2 in best_websites_no_duplicates)
                    {
                        System.Console.WriteLine(item2.GetAttributeValue("href", ""));
                    }
                }
            }
            driver.Close();
            driver.Quit();
        }
    }
}


/*List<IWebElement> get_all_links= driver.FindElements(By.TagName("a")).ToList();
foreach (var item in get_all_links)
{
    System.Console.WriteLine(item.GetAttribute("href"));
}


           // best websites for information from best to worst
           List<IWebElement> possible_search = driver.FindElements(By.CssSelector($"a[href*='{key_sites[0]}']")).ToList();
           // List<IWebElement> possible_search1 = driver.FindElements(By.CssSelector($"a[href*='{key_sites[1]}']")).ToList();

           // first results
           foreach (IWebElement search in possible_search)
           {
               try
               {

                   search.Click();

                   //html download
                   string page_source = driver.PageSource;
                   var HtmlDocument = new HtmlDocument();
                   HtmlDocument.LoadHtml(page_source);

                   //string user_keywords[] = user_input?.Split(" ");
                   var results_list = HtmlDocument.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("href", "").Contains($"{user_input?.Replace(" ", "-")}")).ToList();
                   var date = HtmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("date")).ToList();

                   if (results_list.Count != 0)
                   {                       
                       foreach (var data in date)
                       {
                           results_list.Add(data);
                       }
                       foreach (var item in results_list)
                       {
                           var prints = HtmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("date")).ToList();
                           foreach (var item1 in prints)
                           {
                               System.Console.WriteLine(item.InnerText + item1.InnerText);
                           }
                           //System.Console.WriteLine(item.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("date")).ToList());
                           break;

                       }
                   }


                   break;
               }

               catch (Exception)
               {
                   continue;
               }
           }

           // second results

           driver.Navigate().GoToUrl("https://google.com");
           driver.FindElement(By.Name("q")).SendKeys($"{user_input}" + $" ocacapromocoes" + Keys.Enter);
           List<IWebElement> possible_search1 = driver.FindElements(By.CssSelector($"a[href*='{key_sites[1]}']")).ToList();

           foreach (IWebElement search in possible_search1)
           {
               try
               {

                   search.Click();

                   //html download
                   string page_source = driver.PageSource;
                   var HtmlDocument = new HtmlDocument();
                   HtmlDocument.LoadHtml(page_source);

                   //string user_keywords[] = user_input?.Split(" ");
                   var results_list = HtmlDocument.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("href", "").Contains($"{user_input?.Replace(" ", "-")}")).ToList();
                   //var date = HtmlDocument.DocumentNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("date")).ToList();

                   foreach (var item in results_list)
                   {
                       System.Console.WriteLine(item.InnerText.Replace("Antevisão", "").Replace("Folheto", ""));                      
                   }

                   break;
               }

               catch (Exception)
               {
                   continue;
               }
           }
*/


