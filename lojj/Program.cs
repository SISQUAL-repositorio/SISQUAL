using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;

using System.Collections.ObjectModel;

namespace app
{
class Program {
    static public void Main(string[] args)
    {    
        IWebDriver driver = initializeDriver(".", false); // init the chrome driver
        
        String search_word = "worten";
        driver.Navigate().GoToUrl("https://www.lojj.pt/marcas/");
        
        IWebElement search_bar = driver.FindElement(By.Name("search"));
        search_bar.SendKeys(search_word + Keys.Enter);
        
        IWebElement div_num_paginas = driver.FindElement(By.ClassName("paginacao"));        
        List<IWebElement> paginas = div_num_paginas.FindElements(By.XPath("a")).ToList();
        List<String> links = new List<String> {};
        paginas.ForEach(item => links.Add(item.GetAttribute("href")));
        Console.WriteLine(paginas.Count);
        
        foreach(String link in links) {
            driver.Navigate().GoToUrl(link);
            List<IWebElement> lojas = driver.FindElements(By.CssSelector("div[class='listBox_pesquisa ']")).ToList();
            List<IWebElement> lojas_white = driver.FindElements(By.CssSelector("div[class='listBox_pesquisa white ']")).ToList();
            Console.WriteLine(lojas.Count + lojas_white.Count);
            
        }

        closeDriver(driver);

    }

    private static IWebDriver initializeDriver(string chromeDriverPath, bool headless) 
    {
        ChromeOptions chromeOptions = new ChromeOptions();
        if (headless) chromeOptions.AddArgument("headless"); //if headless true means we want the driver to work without opening the browser
        chromeOptions.AddArgument("user-data-dir=chrome_profile"); //using a chrome user profile dont need to accept again cookies
    
        IWebDriver chromeDriver;
        chromeDriver = new ChromeDriver(chromeDriverPath, chromeOptions);
        chromeDriver.Manage().Window.Maximize(); //check if needed
        return chromeDriver;
    }

    private static void closeDriver(IWebDriver driver) 
    {
        driver.Close();
    }
}
}