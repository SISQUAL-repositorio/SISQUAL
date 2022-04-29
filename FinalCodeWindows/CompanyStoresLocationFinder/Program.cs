using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using CompanyGetStoreLink;
using PostalCodeScraping;
using GetStoreInformation;


namespace CompanyStoresLocationFinder
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            List<string> companieNames = new List<string>();
            List<Company> companies = CompanyGetStoreLinkClass.getCompaniesInformation(companieNames);

            List<Company> companiesFilledWithPostalCode = new List<Company>();
            foreach (Company company in companies)
            {
                Company companyFilledWithPostalCode = PostalCodeScrapingClass.getStoresPostalCodes(company);
                companiesFilledWithPostalCode.Add(companyFilledWithPostalCode);
            }

            List<Company> companiesFilledWithAllInformation = new List<Company>();
            foreach (Company company in companiesFilledWithPostalCode)
            {
                Company companyFilledWithAllInformation = GetStoreInformationClass.getStoresLocation(company);
                companiesFilledWithAllInformation.Add(companyFilledWithAllInformation);
            }
        }
    }
}
