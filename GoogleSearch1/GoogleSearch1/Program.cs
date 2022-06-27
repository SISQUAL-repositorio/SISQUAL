using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;


namespace GoogleSearch1
{

    public class Result
    {
        public string Link { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
           
            Console.Write("O que procura -> ");
            string search = Console.ReadLine();

            string cx = "bf763dcfb6c466ac9";
            string apikey = "AIzaSyBc-tyJbLrCoPXSgx4N4wb6dG32Tleh3-0";

            var request = System.Net.WebRequest.Create("https://cse.google.com/cse?cx=bf763dcfb6c466ac9" + apikey + "&cx=" + cx + "&q=" + search);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Process.Start("https://www.google.com/search?q=" + search);

            //Dar retrieve à data
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);


            string responseString = reader.ReadToEnd();

            dynamic jsonData = JsonConvert.DeserializeObject(responseString);


            //Resultados
            var results = new List<Result>();
            foreach(var item in jsonData.items)
            {
                results.Add(new Result { Link = item.link });
            }

            //TODO: Apresentar resultados ao utilizador permitindo-o escolher a opção que quiser

            // cx :  bf763dcfb6c466ac9
            // API - AIzaSyBc-tyJbLrCoPXSgx4N4wb6dG32Tleh3-0
        }
    }
    
}
