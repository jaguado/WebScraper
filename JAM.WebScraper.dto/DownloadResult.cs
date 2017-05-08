using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAM.WebScraper.dto
{
    public class DownloadResult
    {
        public string Name { set; get; }
        public string Url { set; get; }
        public string BaseUrl { set; get; }
        public bool Selected { set; get; }
        public int DownloadProgress { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDirectory
        {
            get
            {
                return Url != null && Url.EndsWith("/");
            }
        }

        public string RelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(BaseUrl))
                    return "";
                return Url.Replace(BaseUrl, "").Replace("//","/").Replace(Name,"");
            }
        }
    }
}
