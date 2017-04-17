using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JAM.WebScraper.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to The Pirate Bay Web Scraper POC");
            PrintHelper();
            var exit = false;
            while (!exit)
            {
                var cmd = Console.ReadLine();
                if (cmd == "exit" || cmd == "quit") exit = true;
                if (!exit)
                {
                    var results =  FindMovie(cmd);
                    if (results.Any())
                    {
                        Console.WriteLine("Mostrando {0} resultados de la busqueda {1}:", results.Count, cmd);
                        Console.WriteLine("");
                        foreach (var result in results)
                        {
                            Console.WriteLine(result.Name);
                            Console.WriteLine("{0} / {1}", result.Type, result.SubType);
                            Console.WriteLine("Seeds: {0}  /  Leeds: {1}", result.Seeds, result.Leeds);
                            foreach (var desc in result.Description)
                            {
                                Console.WriteLine(desc);
                            }
                            Console.WriteLine("");
                            Console.WriteLine("");
                        }
                    }
                    else
                        Console.WriteLine("No se encontraron resultados para {0}.", cmd);
                }

                PrintHelper();
            }
            Console.WriteLine("Bye!!");
            Thread.Sleep(1000);
        }

        static void PrintHelper()
        {
            Console.WriteLine("Enter movie name to find on The Pirate Bay or 'exit, quit' to Exit!!");
        }
        private const string tpbSearchUrl = @"https://thepiratebay.org/search/{0}/0/7/0";
        static List<TorrentResult> FindMovie(string movie)
        {
            var torrentResults = new List<TorrentResult>();
            var url = string.Format(tpbSearchUrl, movie);
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(url);
                if (data!=null && data.Length>0)
                {
                    String source = Encoding.GetEncoding("utf-8").GetString(data, 0, data.Length - 1);
                    source = WebUtility.HtmlDecode(source);
                    HtmlDocument resultat = new HtmlDocument();
                    resultat.LoadHtml(source);

                    //Parse results
                    var resultsTable = resultat.DocumentNode.Descendants().Where
                            (x => (x.Name == "table" && x.Attributes["id"] != null &&
                               x.Attributes["id"].Value.Equals("searchResult"))).ToList();
                    var resultTables = resultsTable.Count;
                    int resultsCount = 0;
                    foreach(var results in resultsTable)
                    {
                        var rows = results.Descendants("tr").ToList();
                        foreach(var row in rows)
                        {
                            var tempResult = new TorrentResult();
                            var details = row.Descendants().Where(x => x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Equals("detName"));
                            foreach(var detail in details)
                            {
                                tempResult.Name = detail.InnerText.Trim();
                                var descriptions = row.Descendants().Where(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Equals("detDesc"));
                                foreach (var desc in descriptions)
                                {
                                    tempResult.Description.Add(desc.InnerText);
                                }

                                var magnets = row.Descendants().Where(x => x.Name == "a" && x.Attributes["href"] != null && x.Attributes["href"].Value.StartsWith("magnet:"));
                                foreach (var magnet in magnets)
                                {
                                    tempResult.Magnet.Add(magnet.Attributes["href"].Value);
                                }

                                //Seeds and leeds
                                var columns = row.Descendants().Where(x => x.Name == "td").ToList();
                                var colCount = columns.Count();
                                if (colCount > 3)
                                {
                                    var types = columns[0].Descendants().Where(x => x.Name == "a").ToList();
                                    tempResult.Type = types[0].InnerText;
                                    tempResult.SubType = types[1].InnerText;
                                    tempResult.Leeds = int.Parse(columns[colCount-1].InnerText);
                                    tempResult.Seeds = int.Parse(columns[colCount - 2].InnerText);
                                }
                                resultsCount++;
                            }
                            if(!string.IsNullOrEmpty(tempResult.Name))
                                torrentResults.Add(tempResult);
                        }
                    }
                }
            }
            return torrentResults;
        }
    }

    public class TorrentResult
    {
        public string Name { set; get; }
        public string Type { set; get; }
        public string SubType { set; get; }
        public string Link { set; get; }
        public List<string> Description { set; get; }
        public List<string> Magnet { set; get; }

        public int Seeds { set; get; }
        public int Leeds { set; get; }

        public TorrentResult()
        {
            Description = new List<string>();
            Magnet = new List<string>();
        }
    }
}
