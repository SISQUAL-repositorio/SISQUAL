using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using System.Collections.ObjectModel;

namespace app
{
public class Program {
    static public void Main(string[] args)
    {    
        //For testing only
        string date = "03-04-2019";
        List<Location> locations = new List<Location> ();
        Location location1 = new Location("Porto", "Porto", "Portugal");
        Location location2 = new Location("Póvoa de Varzim", "Porto", "Portugal");
        locations.Add(location1);
        locations.Add(location2);
        //
        
        get_data(locations, date);
    }

    public static List<Meteorology> get_data(List<Location> locations, string date) {
        log("LOG::Initializing the chrome driver...");
        IWebDriver driver = initializeDriver(".", false); // init the chrome driver
        
        log("LOG::Navigating to https://www.wunderground.com/");
        driver.Navigate().GoToUrl("https://www.wunderground.com/");

        List<Meteorology> metereologies = new List<Meteorology> ();
        metereologies = get_meteorology_data(driver, locations, date);

        //closeDriver(driver);

        return metereologies;
    }

    private static List<Meteorology> get_meteorology_data(IWebDriver driver, List<Location> locations, string date) {
        List<Meteorology> metereologies = new List<Meteorology> ();

        log("LOG::Date: " + date);
        log("LOG::Total locations: " + locations.Count);
        
        ushort index = 1;
        foreach (Location location in locations) {
            Meteorology metereology = get_meteorology_for_day(driver, location, date);
            metereologies.Add(metereology);
            
            log("LOG::" + index++ + " - " + location.get_location());
            log("LOG::Results\n" + "LOG::" + metereology.get_values());
        }

        return metereologies;
    }

    private static Meteorology get_meteorology_for_day(IWebDriver driver, Location location, string date) {
        Meteorology meteorology = new Meteorology();
        
        if (meteorology.set_date(date) == false) { //means an error has occurred
            log("Error: Unable to set the date given.");
        }

        meteorology.set_location(location);

        if (search_location(driver, location.get_location()) == false) {
            log("Error: Entering the given location" + location + " in search bar.");
        }

        //driver.Navigate().GoToUrl("https://www.wunderground.com/weather/pt/porto"); // TO DO

        if (click_in_history_button(driver) == false) {
            log("Error: Unable to click in history button.");
        }

        if (click_in_monthly_button(driver) == false) {
            log("Error: Unable to click in monthly button.");
        }
        
        if (submit_input_date(driver, meteorology.get_month(), meteorology.get_year()) == false) {
            log("Error: Unable to submit the given date.");
        }
        
        short row_index = -1;
        List<IWebElement> table_columns = new List<IWebElement> ();
        if (get_table_information(driver, ref table_columns, ref row_index, meteorology.get_day()) == false) {
            log("Error: Unable to find the table with the information.");
        }
        
        float temperature = -9999, dew_point = -9999, humidity = -9999, wind_speed = -9999, pressure = -9999, precipitation = -9999;
        temperature = convert_fahrenheit_to_celsius(get_values(driver, table_columns, row_index, 1));
        dew_point = get_values(driver, table_columns, row_index, 2);
        humidity = get_values(driver, table_columns, row_index, 3);
        wind_speed = get_values(driver, table_columns, row_index, 4);
        pressure = get_values(driver, table_columns, row_index, 5);
        precipitation = get_values(driver, table_columns, row_index, 6);

        if (temperature != -9999) meteorology.set_temperature(temperature);
        if (dew_point != -9999) meteorology.set_dew_point(dew_point);
        if (humidity != -9999) meteorology.set_humidity(humidity);
        if (wind_speed != -9999) meteorology.set_wind_speed(wind_speed);
        if (pressure != -9999) meteorology.set_pressure(pressure);
        if (precipitation != -9999) meteorology.set_precipitation(precipitation);

        //closeDriver(driver);

        return meteorology;
    }

    private static bool click_in_history_button(IWebDriver driver) {
        IWebElement history_button;
        ushort num_tries = 0;

        while (num_tries < 3) {
            try {
                history_button = driver.FindElement(By.XPath("//span[contains(text(),'History')]"));
                if (history_button.Displayed && history_button.Enabled) {
                    history_button.Click();
                    return true;
                }
                else Thread.Sleep(1000);
            } catch (StaleElementReferenceException) {
                Thread.Sleep(1000);
            }
            catch (NoSuchElementException) {
                Thread.Sleep(1000);
            }
            num_tries++;
        }

        return false;
    }

    private static bool click_in_monthly_button(IWebDriver driver) {
        IWebElement monthly_button;
        ushort num_tries = 0;

        while (num_tries < 3) {
            try {
                monthly_button = driver.FindElement(By.XPath("//a[contains(text(),'Monthly')]"));
                if (monthly_button.Displayed && monthly_button.Enabled) {
                    monthly_button.Click();
                    return true;
                }
                else Thread.Sleep(1000);
            } catch (StaleElementReferenceException) {
                Thread.Sleep(1000);
            } catch (NoSuchElementException) {
                Thread.Sleep(1000);
            }
            num_tries++;
        }

        return false;
    }

    private static bool submit_input_date(IWebDriver driver, string month, string year) {
        IWebElement select_month, select_year, submit_date;
        ushort num_tries = 0;

        while (num_tries < 3) {
            try {
                select_month = driver.FindElement(By.XPath("//*[@id='monthSelection']")).FindElement(By.XPath("//option[contains(text(), " + month + ")]"));
                select_year = driver.FindElement(By.XPath("//*[@id='yearSelection']")).FindElement(By.XPath("//option[contains(text(), " + year + ")]"));
                submit_date = driver.FindElement(By.XPath("//*[@id='dateSubmit']"));
                select_month.Click();
                select_year.Click();
                submit_date.Click();
                return true;
            } catch (StaleElementReferenceException) {
                Thread.Sleep(1000);
            }
            catch (ElementNotVisibleException) {
                Thread.Sleep(1000);
            }
            catch (NoSuchElementException) {
                Thread.Sleep(1000);
            }
            num_tries++;
        }

        return false;
    }

    private static bool get_table_information(IWebDriver driver, ref List<IWebElement> table_columns, ref short row_index, string day) {
        List<IWebElement> days_column = new List<IWebElement> ();
        ushort num_tries = 0;

        while (num_tries < 3) {
            try {
                IWebElement table = driver.FindElement(By.XPath("//table[@aria-labelledby='History days']"));
                table_columns = table.FindElements(By.XPath(".//tbody/tr/td")).ToList();
                days_column = table_columns.ElementAt(0).FindElements(By.XPath(".//table/tr")).ToList();
                break;
            } catch (NoSuchElementException) {
                Thread.Sleep(1000);
            }
            num_tries++;
        }

        if (num_tries == 3) return false; //means the table was not found after the 3 tries

        for (short i = 0; i < days_column.Count; i++) {
            if (days_column.ElementAt(i).Text == day) {
                row_index = i;
                return true;
            }
        }

        return false;
    }

    private static float get_values(IWebDriver driver, List<IWebElement> table_columns, short row_index, ushort field_index) {
        List<IWebElement> field_column = new List<IWebElement> ();
        List<IWebElement> data_values = new List<IWebElement> ();

        try {
            field_column = table_columns.ElementAt(field_index).FindElements(By.XPath(".//table/tr")).ToList();
            data_values = field_column.ElementAt(row_index).FindElements(By.XPath(".//td")).ToList();
        } catch (NoSuchElementException) {
            return -9999;
        }

        if (data_values.Count == 1) // if it only has one value        
            return float.Parse(data_values.ElementAt(0).Text, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        
        return float.Parse(data_values.ElementAt(1).Text, System.Globalization.CultureInfo.InvariantCulture.NumberFormat); //means it has 3 values, Max Avg Min
    }

    private static float convert_fahrenheit_to_celsius(float fahrenheit_temperature) {
        return (fahrenheit_temperature - 32) * 5 / 9;
    }

    private static bool search_location(IWebDriver driver, string location_to_search) 
    {
        IWebElement search_bar = driver.FindElement(By.Id("wuSearch"));
        
        try {
            if (search_bar.Displayed && search_bar.Enabled) {
                search_bar.Click();
                search_bar.SendKeys(location_to_search);
                search_bar.Click();
                //ui-autocomplete ui-front ui-menu ui-widget ui-widget-content ui-corner-all hide
                ////*[@id="wuForm"]/search-autocomplete/ul/li[2]
                Thread.Sleep(2000);
                driver.FindElement(By.XPath("//*[@id='wuForm']/search-autocomplete/ul/li[2]")).Click();
                //List<IWebElement> suggestions = driver.FindElement(By.XPath("//*[@id='wuForm']/search-autocomplete/ul")).FindElements(By.XPath(".//li")).ToList();
                /*bool accept_next_suggestion = false;
                foreach(IWebElement suggestion in suggestions) {
                    Console.WriteLine(suggestion.Text);
                    if (suggestion.Text == "city") {
                        accept_next_suggestion = true;
                    }
                    if(accept_next_suggestion) suggestion.Click();
                }*/
            }
            else {
                return false;
            }  
        }
        catch (StaleElementReferenceException) {
            return false;
        }
        
        return true;
    }
    private static IWebDriver initializeDriver(string chromeDriverPath, bool headless) {
        ChromeOptions chromeOptions = new ChromeOptions();
        if (headless) chromeOptions.AddArgument("headless"); //if headless true means we want the driver to work without opening the browser
    
        IWebDriver chromeDriver;
        chromeDriver = new ChromeDriver(chromeDriverPath, chromeOptions);
        chromeDriver.Manage().Window.Maximize();
        return chromeDriver;
    }

    private static void closeDriver(IWebDriver driver) {
        driver.Close();
    }
    private static void log(String message) {
        Console.WriteLine(message);
    }
}
}
