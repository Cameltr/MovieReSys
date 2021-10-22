using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReSys
{
    class UserMotionObserver : Observer
    {
        public void Update(string MessageType, int userId, int movieId, int submitScore)
        {
            string path = @"D:\xbc\Fighting\otherSubject\SOFTDe\data\ml-100k\";
            Recommend recommend = new Recommend(path);
            recommend.changeScore(userId, movieId, submitScore);
        }
    }
}
