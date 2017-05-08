using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAM.WebScraper.dto
{
    public class TorrentResult
    {
        public int Position { set; get; }
        public string Name { set; get; }
        public string Type { set; get; }
        public string SubType { set; get; }
        public string Link { set; get; }
        public bool Vip { set; get; }
        public List<string> Description { set; get; }
        public List<Tuple<string,string>> Links { set; get; }

        public int Seeds { set; get; }
        public int Leeds { set; get; }

        public TorrentResult()
        {
            Description = new List<string>();
            Links = new List<Tuple<string, string>>();
        }
        
        
    }
}
