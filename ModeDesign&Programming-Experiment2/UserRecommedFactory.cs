using MovieReSys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieResys
{
    abstract class Product// 抽象产品类（电影列表）
    {
        public abstract void UserRecommend();
    }

    class UserRecommendedMovie : Product //返回推荐的电影，具体产品类
    {
        //实现业务方法
        private int userId;//推荐算法需要userId
        protected int[] product;

        public UserRecommendedMovie(int _userId)
        {
            this.userId = _userId;
        }
        public override void UserRecommend()//覆盖原抽象类的推荐函数
        {
            string path = @"D:\xbc\Fighting\otherSubject\SOFTDe\data\ml-100k\";
            //路径xbgg修改一下，这里是照搬你代码的
            MovieReSys.Recommend recommend = new Recommend(path);
            //新建推荐对象
            product = recommend.userRecom(userId);
            //调用recommed类里面的推荐函数
        }
    }

    class Factory
    {
        //静态工厂方法
        public static Product GetProduct(int _userId)
        {
            Product product = null;
            Console.WriteLine("start to recommend movie");
            product = new UserRecommendedMovie(_userId);
            //初始化设置product
            return product;
        }
    }
}
