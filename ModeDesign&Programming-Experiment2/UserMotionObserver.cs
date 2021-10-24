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
            MovieReSysAlgorithm movieReSysAlgorithm = (MovieReSysAlgorithm)MovieReSysFactory.getProduct("MovieReSysAlgorithm");
            movieReSysAlgorithm.UserUpdateScore(userId, movieId, submitScore);
            Console.WriteLine(String.Format("打分成功！： 用户 {0} 给电影 {1} 打分 {2}", userId, movieId, submitScore));
        }
    }
}
