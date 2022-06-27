using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
//using static SeleniumMaps.ResultsList;


namespace SeleniumMaps
{
    internal class Program
    {
        static void Main(string[] agrs)
        {

            var resultadosPesquisa = new List<String> { 
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[3]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[5]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[7]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[9]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[11]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[13]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[15]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[17]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[19]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[21]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[23]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[25]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[27]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[29]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[31]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[33]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[35]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[37]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[39]/div",
                "//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[41]/div"
            };

            //INICIAR CHROME MODO HEADLESS
            //var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("headless");

            ChromeOptions option = new ChromeOptions();
            option.AddArgument("headless");


            //User input
            Console.Write("O que pretende encontrar? -> ");
            string inputPesquisa = Console.ReadLine();

            //Iniciar driver modo headless
            IWebDriver driver = new ChromeDriver(@"C:\Users\Pedro.Costa\Desktop\ChromeDriver");
            driver.Navigate().GoToUrl("https://www.google.pt/maps/");


            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
           // driver.Manage().Window.Maximize();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            


            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            IWebElement search = driver.FindElement(By.XPath("//*[@id='searchboxinput']"));
            search.SendKeys(inputPesquisa);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            IWebElement searhBtn = driver.FindElement(By.XPath("//*[@id='searchbox']/div[1]"));
            searhBtn.Click();

            

            //Ver quantos resultados aparecem
            // bool resultsExist = driver.FindElement(By.XPath("//*[@id='pane']/div/div[1]/div/div/div[2]/div[2]")).Displayed();

            //bool results = driver.PageSource.Contains("//*[@id='pane']/div/div[1]/div/div/div[2]/div[2]");


            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            List<IWebElement> e = new List<IWebElement>();
            e.AddRange(driver.FindElements(By.XPath("//*[@id='pane']/div/div[1]/div/div/div[1]/div[1]/button/img")));
            Console.Clear();
            //checking element count in list
            if (e.Count == 0)
            {

               
                int nrResults = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("A exibir os " + nrResults + " resultados ("+ inputPesquisa+") mais perto de si:");


                var c = 1;
                var c2 = 0;
                foreach (string s in resultadosPesquisa)
                {
                    if (c2 < nrResults){
                        //IWebElement element2 = driver.FindElement(By.XPath("//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]"));
                        //Actions actions = new Actions(driver);
                        //actions.MoveToElement(element2);
                        //actions.Perform();
                        IWebElement clickPesquisa = driver.FindElement(By.XPath(s));
                        clickPesquisa.Click();
                        List<IWebElement> e2 = new List<IWebElement>();
                        e2.AddRange(driver.FindElements(By.XPath("//*[@id='pane']/div/div[1]/div/div/div[9]/div[1]/button/div[1]/div[2]/div[1]")));
                        if (e2.Count != 0)
                        {
                            IWebElement textLocalidade = driver.FindElement(By.XPath("//*[@id='pane']/div/div[1]/div/div/div[9]/div[1]/button/div[1]/div[2]/div[1]"));
                            //*[@id="pane"]/div/div[1]/div/div/div[2]/div[1]/div[3]/div
                            string textLocalidade2 = textLocalidade.Text;
                            IWebElement textLocalidadeEsp = driver.FindElement(By.XPath("//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[1]/div[1]/h1/span[1]"));
                            string textLocalidadeEsp2 = textLocalidadeEsp.Text;
                            Console.WriteLine(textLocalidadeEsp2 + " situa-se em -> " + textLocalidade2);
                            Console.WriteLine("---------------------------------------------------------------------------------------------------------------");

                        }
                        else
                        {
                            IWebElement textLocalidade = driver.FindElement(By.XPath("//*[@id='pane']/div/div[1]/div/div/div[7]/div[1]/button/div[1]/div[2]/div[1]"));                          
                            //*[@id="pane"]/div/div[1]/div/div/div[2]/div[1]/div[3]/div
                            string textLocalidade2 = textLocalidade.Text;
                            //IWebElement textLocalidadeEsp = driver.FindElement(By.XPath("//*[@id='pane']/div/div[1]/div/div/div[2]/div[1]/div[1]/div[1]/h1/span[1]"));
                            //string textLocalidadeEsp2 = textLocalidadeEsp.Text;
                            Console.WriteLine(inputPesquisa + " situa-se em -> " + textLocalidade2);
                            Console.WriteLine("---------------------------------------------------------------------------------------------------------------");

                        }
                       
                        c++;
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                        //driver.Navigate().Back();
                        IWebElement backBtn = driver.FindElement(By.XPath("//*[@id='omnibox-singlebox']/div[1]/div[1]/button"));
                        backBtn.Click();
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                        c2++;

                    }
  
                }
  
            }
            else
            {
                Console.WriteLine("Resultado para "+inputPesquisa+" :");
                IWebElement textLocalidadeSolo = driver.FindElement(By.XPath("//*[@id='pane']/div/div[1]/div/div/div[7]/div[1]/button/div[1]/div[2]/div[1]"));
                string textLocalidadeSolo2 = textLocalidadeSolo.Text;
                Console.WriteLine(inputPesquisa + " situa-se em -> " + textLocalidadeSolo2);
            }

            driver.Close();

        }



    }
}

