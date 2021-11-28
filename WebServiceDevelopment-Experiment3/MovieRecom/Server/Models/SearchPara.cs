using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ScorePara
    {
        public int userId { set; get; }
        public int movieId { set; get; }
        public double score { set; get; }
    }
}
