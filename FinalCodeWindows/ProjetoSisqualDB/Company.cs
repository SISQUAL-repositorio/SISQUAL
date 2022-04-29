using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoSisqualDB
{
    internal class Company
    {
        private string companyName = "";
        private string companyWebPageHtml = "";
        private string storesInformationLink = "";
        private List<Store> stores = new List<Store>();

        public Company(string companyName)
        {
            this.companyName = companyName;
        }
        public void setCompanyWebPageHtml(string companyWebPageHtml)
        {
            this.companyWebPageHtml = companyWebPageHtml;
        }
        public void setStoresInformationLink(string storesInformationLink)
        {
            this.storesInformationLink = storesInformationLink;
        }
        public void setStores(List<Store> stores)
        {
            this.stores = stores;
        }
        public string getCompanyName()
        {
            return this.companyName;
        }
        public string getCompanyWebPageHtml()
        {
            return this.companyWebPageHtml;
        }
        public string getStoresInformationLink()
        {
            return this.storesInformationLink;
        }
        public List<Store> getStores()
        {
            return this.stores;
        }
        public void showInfo()
        { //just for debugging, delete when dll
            Console.WriteLine("Company Name: " + this.companyName);
            Console.WriteLine("Company Web Page: " + this.companyWebPageHtml);
            Console.WriteLine("Stores Information Link: " + this.storesInformationLink);
        }
    }
}
