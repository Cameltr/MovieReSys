using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    class UserMotionObserver : Observer
    {
        public void Update(string MessageType, int userId, int movieId, int submitScore)
        {
            MovieReSysAlgorithm movieReSysAlgorithm = (MovieReSysAlgorithm)MovieReSysFactory.getProduct("MovieReSysAlgorithm");
            movieReSysAlgorithm.UserUpdateScore(userId, movieId, submitScore);
            Console.WriteLine("修改成功!");
        }
    }
}
