using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

using GoogleChromeDriver;
using CompanyGetStoreLink;

namespace PostalCodeScraping
{
    public class PostalCodeScrapingClass
    {
        private static Dictionary<string, Regex> postalCodesTemplates = new Dictionary<string, Regex>() {
            {"pt", new Regex("[1-9][0-9]{3}-[0-9]{3}")},
            {"en", new Regex("[0-9]{4}-[0-9]{3}")},
        };
        public static Company getStoresPostalCodes(Company company)
        {
            List<Store> stores = new List<Store>();

            IWebDriver driver = Driver.initializeDriver("C:\\Users\\Pedro\\MEOCloud", false);

            CompanyGetStoreLinkClass.getCompanyInformation(ref company);

            company.showInfo();

            string storeInformationLink = company.getStoresInformationLink();
            if (storeInformationLink == "")
            {
                return company;
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

            Driver.closeDriver(driver);

            company.setStores(stores);
            return company;
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
    }
}
