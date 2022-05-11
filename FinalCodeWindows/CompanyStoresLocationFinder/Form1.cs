using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CompanyGetStoreLink;
using PostalCodeScraping;
using GetStoreInformation;


namespace CompanyStoresLocationFinder
{
    public partial class Form1 : Form
    {
        private List<string> companyNames = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }
        public List<string> getCompanyNames()
        {
            return companyNames;
        }

        public void debug(string text)
        {
            MessageBox.Show(text);
        }

        public void showOutput(string text) 
        {
            output.Text += text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            companyNames = companyNamesList.Text.Split(';').ToList();
            companyNamesList.Text = "";

            if (companyNames == null || companyNames.Count() == 0)
            {
                return;
            }
            this.debug("Creation of companies list");
            this.debug("COUNT: " + companyNames.Count());
            
            List<Company> companies = new List<Company>();
            foreach(string companyName in companyNames)
            {
                companies.Add(new Company(companyName));
            }

            this.debug("Getting stores postal code");
            List<Company> companiesFilledWithPostalCode = new List<Company>();
            foreach (Company company in companies)
            {
                Company companyFilledWithPostalCode = PostalCodeScrapingClass.getStoresPostalCodes(company);
                companiesFilledWithPostalCode.Add(companyFilledWithPostalCode);
            }

            this.debug("Getting stores location from the postal code, using the zicodeAPI");
            List<Company> companiesFilledWithAllInformation = new List<Company>();
            foreach (Company company in companiesFilledWithPostalCode)
            {
                Company companyFilledWithAllInformation = GetStoreInformationClass.getStoresLocation(company);
                companiesFilledWithAllInformation.Add(companyFilledWithAllInformation);
            }

            foreach (Company company in companiesFilledWithAllInformation)
            {
                this.showOutput(company.toString());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
