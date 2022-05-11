using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

using GoogleChromeDriver;

namespace CompanyGetStoreLink
{
    public class CompanyGetStoreLinkClass
    {
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
            {"en", keyWordsWeightEN},
        };
        public static List<Company> getCompaniesInformation(IWebDriver driver, List<string> companiesName)
        {
            List<Company> companiesInformations = new List<Company>();

            foreach (string companyName in companiesName)
            {
                Company companyToAdd = new Company(companyName);
                companyToAdd = getCompanyInformation(driver, companyToAdd);
                companiesInformations.Add(companyToAdd);
            }

            return companiesInformations;
        }
        public static Company getCompanyInformation(IWebDriver driver, Company company)
        {
            string companyName = company.getCompanyName();
            if (companyName == "")
            {
                return company;
            }

            Driver.doGoogleSearch(driver, companyName);

            string companyWebPageHtml = Driver.getAllGoogleSearchLinks(driver).ElementAt(0);
            company.setCompanyWebPageHtml(companyWebPageHtml);
            driver.Navigate().GoToUrl(companyWebPageHtml);

            Dictionary<int, string> keyWords = getKeyWords();

            Dictionary<IWebElement, int> storeLinks = getStoresLinks(driver, keyWords);

            string storeLink = getBestStoreLink(storeLinks);
            if (storeLink == null)
            {
                Driver.closeDriver(driver);
                return company;
            }

            company.setStoresInformationLink(storeLink);
            return company;
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
                        if (linkTextWeight > 0)
                        {
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
                if (link.Count() < lowestSizeLink)
                {
                    lowestSizeLink = link.Count();
                    bestLink = link;
                }
            }

            return (bestLink == "") ? null : bestLink;
        }
    }
}
