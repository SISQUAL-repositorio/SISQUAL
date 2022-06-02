using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

using GoogleChromeDriver;

namespace CompanyGetStoreLink
{
    /// <summary>
    /// 
    /// </summary>
    public class CompanyGetStoreLinkClass
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="companyName"></param>
        /// <param name="keyWords"></param>
        /// <param name="companyWebLink"></param>
        /// <param name="companyStoresWebLink"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool GetCompanyWebLinks(IWebDriver driver, string companyName, Dictionary<int, string> keyWords, out string companyWebLink, out string companyStoresWebLink, out string errorMessage)
        {
            errorMessage = string.Empty;
            companyWebLink = string.Empty;
            companyStoresWebLink = string.Empty;

            if (string.IsNullOrEmpty(companyName) || driver == null)
            {
                return false;
            }

            if(!GetCompanyWebLink(driver, companyName, out companyWebLink, out errorMessage))
            {
                return false;
            }

            if (!GetCompanyStoresWeblink(driver, companyWebLink, keyWords, out companyStoresWebLink, out errorMessage))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="companyName"></param>
        /// <param name="companyWebLink"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static bool GetCompanyWebLink(IWebDriver driver, string companyName, out string companyWebLink, out string errorMessage)
        {
            companyWebLink = string.Empty;
            List<string> googleResultsList;

            if (!Driver.GetAllGoogleSearchLinks(driver, companyName, out googleResultsList, out errorMessage))
            {
                return false;
            }

            companyWebLink = googleResultsList.ElementAt(0); //TO DO: IMPROVE
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="companyWebLink"></param>
        /// <param name="keyWords"></param>
        /// <param name="companyStoresWebLink"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static bool GetCompanyStoresWeblink(IWebDriver driver, string companyWebLink, Dictionary<int, string> keyWords, out string companyStoresWebLink, out string errorMessage)
        {
            errorMessage = string.Empty;
            companyStoresWebLink= string.Empty;

            try
            {
                driver.Navigate().GoToUrl(companyWebLink);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            
            Dictionary<IWebElement, int> allPossibleCompanyStoresLinks = GetAllCompanyStoresLinks(driver, keyWords);
            List<string> bestCompanyStoresLinks = GetBestCompanyStoresLink(allPossibleCompanyStoresLinks, keyWords);

            if (bestCompanyStoresLinks.Count == 0)
            {
                Driver.CloseDriver(driver);
                errorMessage = "Unable to find company stores information link.";
                return false;
            }

            companyStoresWebLink = bestCompanyStoresLinks.ElementAt(0); //TO DO: IMPROVE
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="keyWords"></param>
        /// <returns></returns>
        private static Dictionary<IWebElement, int> GetAllCompanyStoresLinks(IWebDriver driver, Dictionary<int, string> keyWords)
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
                        linkTextWeight = CorrectKeyWord(keyWords, link.Text);
                        if (linkTextWeight > 0)
                        {
                            usefulLinks.Add(link, linkTextWeight);
                        }
                    }
                }
                catch (StaleElementReferenceException)
                {
                    continue;
                }
            }

            return usefulLinks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeLinks"></param>
        /// <param name="keyWords"></param>
        /// <returns></returns>
        private static List<string> GetBestCompanyStoresLink(Dictionary<IWebElement, int> storeLinks, Dictionary<int, string> keyWords)
        {
            if (storeLinks.Count == 0) {
                return null;
            }

            IOrderedEnumerable<KeyValuePair<IWebElement, int>> sortedStoreLinks = from entry in storeLinks orderby entry.Value ascending select entry;
            int lowestValue = sortedStoreLinks.ElementAt(0).Value; //the lowest value is equivalent to the first entry in the sorted dictionary
            List<string> bestsStoreLinks = new List<string>();
            
            foreach (KeyValuePair<IWebElement, int> storeLink in sortedStoreLinks)
            {
                if (storeLink.Value > lowestValue)
                {
                    break;
                }
                bestsStoreLinks.Add(storeLink.Key.GetAttribute("href"));
            }

            List<string> bestsStoreLinksBasedOnHref = new List<string>();
            
            foreach (string link in bestsStoreLinks)
            {
                if (CorrectKeyWord(keyWords, link) > 0)
                {
                    if (!bestsStoreLinksBasedOnHref.Contains(link))
                    {
                        bestsStoreLinksBasedOnHref.Add(link);
                    }
                }
            }

            return bestsStoreLinksBasedOnHref;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        private static int CorrectKeyWord(Dictionary<int, string> keyWords, String word)
        {
            if (string.IsNullOrEmpty(word))
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

        #endregion
    }
}
