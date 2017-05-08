using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAM.WebScraper.dto
{
    public class VideosResult
    {
        public Info info { get; set; }
        public string url { get; set; }
    }
    public class Format
{
    public string ext { get; set; }
    public string format { get; set; }
    public string format_id { get; set; }
    public string format_note { get; set; }

    public double? preference { get; set; }
    public string protocol { get; set; }
    public string resolution { get; set; }
    public string url { get; set; }
    public string acodec { get; set; }
    public double? asr { get; set; }
    public object filesize { get; set; }
    public double? fps { get; set; }
    public double? height { get; set; }
    public object language { get; set; }
    public string manifest_url { get; set; }
    public double? tbr { get; set; }
    public string vcodec { get; set; }
    public double? width { get; set; }
}
    public class Thumbnail
    {
        public string id { get; set; }
        public string url { get; set; }
    }
    public class Info
{
    public object comment_count { get; set; }
    public string description { get; set; }
    public string display_id { get; set; }
    public int duration { get; set; }
    public string ext { get; set; }
    public string extractor { get; set; }
    public string extractor_key { get; set; }
    public string format { get; set; }
    public string format_id { get; set; }
    public List<Format> formats { get; set; }
    public double fps { get; set; }
    public double height { get; set; }

    public string id { get; set; }
    public object license { get; set; }
    public object like_count { get; set; }
    public object playlist { get; set; }
    public object playlist_index { get; set; }
    public string protocol { get; set; }
    public object requested_subtitles { get; set; }

    public object tbr { get; set; }
    public string thumbnail { get; set; }
    public List<Thumbnail> thumbnails { get; set; }
    public int timestamp { get; set; }
    public string title { get; set; }
    public string upload_date { get; set; }
    public string uploader { get; set; }
    public string uploader_id { get; set; }
    public string uploader_url { get; set; }
    public string url { get; set; }
    public object view_count { get; set; }
    public string webpage_url { get; set; }
    public string webpage_url_basename { get; set; }
    public int width { get; set; }
}
}
