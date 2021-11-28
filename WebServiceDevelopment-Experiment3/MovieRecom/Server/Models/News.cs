using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class News
    {
        public String Id { get; set; } = "";
        public String title { get; set; } = "";
        public String content { get; set; } = "";
        public String url { get; set; } = "";
        public String date { get; set; } = "";
        public double pagerank { get; set; } = 0;
        public double BM25 { get; set; } = 0;
        public String IsFavored { get; set; } = "0";

        // 每个关键词出现的次数，用于计算相关度
        public Dictionary<String, int> keywordNum { get; set; } = new Dictionary<String, int>();
        // 所有出现过的关键词
        public HashSet<String> keywords { get; set; } = new HashSet<string>();

        public News() { }

        public News(String Id, String title, String content, String url, String date, double pagerank, double BM25 = 0)
        {
            this.Id = Id;
            this.title = title;
            this.content = content;
            this.url = url;
            this.date = date;
            this.pagerank = pagerank;
            this.BM25 = BM25;
        }

        public News NewsClone()
        {
            return MemberwiseClone() as News;
        }
}
}
