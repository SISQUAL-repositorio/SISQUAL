using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

using GoogleChromeDriver;
using CompanyGetStoreLink;

namespace PostalCodeScraping
{
    /// <summary>
    /// 
    /// </summary>
    public class PostalCodeScrapingClass
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="chromeDriver"></param>
        /// <returns></returns>
        public static bool GetStoresPostalCodes(string companyName, string chromeDriverPath, Dictionary<int, string> storeLinkKeyWords, Regex postalCodeTemplate, out Company company, out string errorMessage)
        {
            company = new Company(companyName);

            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(chromeDriverPath) || postalCodeTemplate==null || storeLinkKeyWords==null || storeLinkKeyWords.Count==0)
            {
                errorMessage = "Invalid parameters in function GetStoresPostalCodes call";
                return false;
            }
            

            IWebDriver driver = Driver.InitializeDriver(chromeDriverPath, false);

            try
            {
                string companyWebLink = string.Empty;
                string companyStoresWebLink = string.Empty;

                if(!CompanyGetStoreLinkClass.GetCompanyWebLinks(driver, companyName, storeLinkKeyWords, out companyWebLink, out companyStoresWebLink, out errorMessage))
                {
                    return false;
                }

                company.CompanyWebLink = companyWebLink;
                company.CompanyStoresWebLink = companyStoresWebLink;


                List<Store> stores;
                if (!GetStores(driver, companyStoresWebLink, postalCodeTemplate, out stores, out errorMessage))
                {
                    Driver.CloseDriver(driver);
                    return false;
                }

                company.Stores = stores;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Driver.CloseDriver(driver);
                return false;
            }

            Driver.CloseDriver(driver);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="companyStoresWebLink"></param>
        /// <param name="postalCodeRegex"></param>
        /// <param name="stores"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static bool GetStores(IWebDriver driver, string companyStoresWebLink, Regex postalCodeRegex, out List<Store> stores, out string errorMessage)
        {
            stores = new List<Store>();

            List<string> zipCodes;
            if (!GetPostalCodes(driver, companyStoresWebLink, postalCodeRegex, out zipCodes, out errorMessage))
            {
                return false;
            }

            foreach(string zipCode in zipCodes)
            {
                stores.Add(new Store(zipCode));
            }

            if(stores.Count == 0)
            {
                errorMessage = "No store was found.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="companyStoresWebLink"></param>
        /// <param name="postalCodeRegex"></param>
        /// <param name="zipCodes"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static bool GetPostalCodes(IWebDriver driver, string companyStoresWebLink, Regex postalCodeRegex, out List<string> zipCodes, out string errorMessage)
        {
            errorMessage = string.Empty;
            zipCodes = new List<string>();

            try
            {
                driver.Navigate().GoToUrl(companyStoresWebLink);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            string fullCompanyStoreLinkSource = driver.PageSource;

            //add zipcodes found using regex excluding repeated values
            foreach (Match match in postalCodeRegex.Matches(fullCompanyStoreLinkSource))
            {
                if (!zipCodes.Contains(match.Value))
                {
                    zipCodes.Add(match.Value);
                }
            }

            return true;
        }

        #endregion
    }
}
