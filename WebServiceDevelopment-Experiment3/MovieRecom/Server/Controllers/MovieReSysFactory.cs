using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Controllers
{
    // 抽象产品(算法)
    abstract class AlgorithmProduct
    {
        public abstract List<ReMovie> UserRecommend(int userId);

        public abstract List<ReMovie> GetScoredMovies(int userId);

    }

    // 具体产品(推荐算法)
    class MovieReSysAlgorithm : AlgorithmProduct
    {
        Recommend recommend;

        public MovieReSysAlgorithm()
        {
            recommend = new Recommend();
        }

        public override List<ReMovie> UserRecommend(int userId)
        {
            return recommend.userRecom(userId);
        }

        public override List<ReMovie> GetScoredMovies(int userId)
        {
            return recommend.getScoredMovies(userId);
        }

        public void UserUpdateScore(int userId, int movieId, double submitScore)
        {
            recommend.changeScore(userId, movieId, submitScore);
        }

        
    }

    // 算法工厂
    class MovieReSysFactory
    {
        private static Dictionary<string, AlgorithmProduct> productMap = new Dictionary<string, AlgorithmProduct>();

        public MovieReSysFactory()
        {
        }

        public static AlgorithmProduct getProduct(string productName)
        {
            if (productMap.Keys.Contains(productName))
            {
                return productMap[productName];
            }

            AlgorithmProduct product = null;

            if (productName == "MovieReSysAlgorithm")
            {
                product = new MovieReSysAlgorithm();
                productMap[productName] = product;
            }

            return product;
        }


    }
}
