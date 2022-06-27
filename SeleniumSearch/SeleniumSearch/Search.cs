using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;


namespace SeleniumSearch
{
    class Search
    {
    
        static void Main(string[] agrs)
        {

            //INICIAR CHROME MODO HEADLESS
            //var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("headless");

            ChromeOptions option = new ChromeOptions();
            option.AddArgument("headless");


            //User input
            Console.Write("O que pretende encontrar? -> ");
            string inputPesquisa = Console.ReadLine();

            inputPesquisa.Trim().Replace(" ", "+");
            //Iniciar driver modo headless
            IWebDriver driver = new ChromeDriver(@"C:\Users\Pedro.Costa\Desktop\ChromeDriver",option);
            driver.Navigate().GoToUrl($"https://google.com/search?q={inputPesquisa}");


            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();


            //Pesquisa
            IWebElement termsAgree = driver.FindElement(By.XPath("//*[@id='L2AGLb']"));
            termsAgree.Click();


            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
           // IWebElement search = driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div[1]/div/div[2]/input"));

           // search.SendKeys(inputPesquisa);

          //  driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
          //  IWebElement searhBtn = driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[3]/center/input[1]"));
          //  searhBtn.Click();

            //VERIFICAR QUAIS SÃO ANÚNCIOS
            //CLICAR NO PRIMEIRO RESULTADO
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            IWebElement results = driver.FindElement(By.ClassName("iUh30"));
            
            results.Click();


            //Retrieve link

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(4);
            string link = driver.Url;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(4);
            //Fechar driver headless
            driver.Close();

            //Abrir nova driver
            
            IWebDriver driver2 = new ChromeDriver(@"C:\Users\Pedro.Costa\Desktop\ChromeDriver");
            driver2.Navigate().GoToUrl(link);

            Console.WriteLine("LINK É" + link);

            //RETIRAR MODO HEADLESS

            //driver.Close();
        }

       

    }
}
