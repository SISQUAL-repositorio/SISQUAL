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
            MessageBox.Show(companyNames.ToString());
            companyNamesList.Text = "";

            if (companyNames == null || companyNames.Count() == 0)
            {
                return;
            }
            this.debug("got here");
            this.debug("COUNT: " + companyNames.Count());
            this.debug("FIRST: " + companyNames.ElementAt(0));

            
            List<Company> companies = new List<Company>();
            foreach(string companyName in companyNames)
            {
                companies.Add(new Company(companyName));
            }

            this.debug("got here2");
            List<Company> companiesFilledWithPostalCode = new List<Company>();
            foreach (Company company in companies)
            {
                Company companyFilledWithPostalCode = PostalCodeScrapingClass.getStoresPostalCodes(company);
                companiesFilledWithPostalCode.Add(companyFilledWithPostalCode);
            }
            this.debug("got here3");

            List<Company> companiesFilledWithAllInformation = new List<Company>();
            this.debug("got here4");
            foreach (Company company in companiesFilledWithPostalCode)
            {
                Company companyFilledWithAllInformation = GetStoreInformationClass.getStoresLocation(company);
                companiesFilledWithAllInformation.Add(companyFilledWithAllInformation);
            }
            this.debug("got here5");

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
