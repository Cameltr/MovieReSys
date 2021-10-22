using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommend
{
    class Movie
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Url { get; set; }

        public Movie(int index, string name, string date, string url)
        {
            Index = index;
            Name = name;
            Date = date;
            Url = url;
        }
    }
}
