using System;
using NumSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MovieReSys
{
    class Recommend
    {
        private NDArray userScores = null;  // 打分的矩阵
        private NDArray hasScores = null;   // 是否打分的矩阵
        private Dictionary<int, NDArray> corMatrix = new Dictionary<int, NDArray>();
        private Dictionary<int, NDArray> corIndex = new Dictionary<int, NDArray>();
        private List<Movie> movieList = null;
        private int[] nerbNums = null;

        bool isChanged = false;

        private int maxUser = 0;
        private int maxMovie = 0;
        public int minComMoviesNum { get; set; }

        private int getNerbNum(int userId)
        {
            var corList = corMatrix[userId - 1].copy();
            //double corMin = np.min(corList), corMax = np.max(corList);
            //corList = (corList - corMin) / (corMax - corMin);

            double cen1 = np.max(corList), cen2 = np.min(corList);
            NDArray index = np.zeros<int>(maxUser); // 每个用户的相关度属于第几类
            while (true)
            {
                // 重分类
                for (int i = 0; i < corList.size; i++)
                {
                    double score = corList[i];

                    if (Math.Abs(score - cen1) > Math.Abs(score - cen2))
                    {
                        index[i] = 2;
                    }
                    else
                    {
                        index[i] = 1;
                    }
                }

                double newCen1 = np.mean(corList[index == 1]);
                double newCen2 = np.mean(corList[index == 2]);
                if (Math.Abs(newCen1 - cen1) + Math.Abs(newCen2 - cen2) < 1e-3)
                {
                    break;
                }

                cen1 = newCen1;
                cen2 = newCen2;
            }

            int newbNum = 0;

            if (cen1 > cen2)
            {
                newbNum = corList[index == 2].size;
                //Console.WriteLine(np.max(corList[index == 2]).ToString());
            }
            else
            {
                newbNum = corList[index == 1].size;
                //Console.WriteLine(np.max(corList[index == 1]).ToString());
            }

            return newbNum;
        }

        public void loadData(string path)
        {
            // 创建一个 StreamReader 的实例来读取文件 
            // using 语句也能关闭 StreamReader
            using (StreamReader sr = new StreamReader(path + "u.data"))
            {
                string line;

                List<List<int>> arrs = new List<List<int>>();
                List<double> scores = new List<double>();
                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    var list = Regex.Split(line, @"\s+");
                    int user = int.Parse(list[0]), movie = int.Parse(list[1]);
                    double score = double.Parse(list[2]);
                    maxUser = Math.Max(maxUser, user);
                    maxMovie = Math.Max(maxMovie, movie);
                    arrs.Add(new List<int>(new int[] { user, movie }));
                    scores.Add(score);
                }

                this.userScores = np.array<double>(new double[maxUser, maxMovie]);
                this.hasScores = np.zeros<int>(userScores.shape);

                for (int i = 0; i < arrs.Count; i++)
                {
                    userScores[arrs[i][0] - 1, arrs[i][1] - 1] = scores[i];
                    if (scores[i] > 0) hasScores[arrs[i][0] - 1, arrs[i][1] - 1] = 1;
                }

            }

            movieList = new List<Movie>(maxMovie);
            // 创建一个 StreamReader 的实例来读取文件 
            // using 语句也能关闭 StreamReader
            using (StreamReader sr = new StreamReader(path + "u.item"))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    var list = Regex.Split(line, @"\|");
                    movieList.Add(new Movie(int.Parse(list[0]), list[1], list[2], list[4]));
                }
            }
        }


        public Recommend(string path = @"E:\ml-100k\")
        {
            loadData(path);
            nerbNums = new int[maxUser];
            for (int i = 0; i < maxUser; i++) nerbNums[i] = -1;
        }

        public double calSimilarity(int userA, int userB)
        {
            var aList = userScores[userA - 1];
            var bList = userScores[userB - 1];

            var aHas = hasScores[userA - 1];
            var bHas = hasScores[userB - 1];

            var comMovie = aHas * bHas;
            int comNum = np.sum(comMovie);

            if (comNum < minComMoviesNum) return 0;

            double meanA = np.mean(aList[!aHas == 0]); // double.Parse(np.sum(aList, typeof(double)).ToString()) / (double.Parse(np.sum(aHas).ToString()) + 1e-5);
            double meanB = np.mean(bList[!bHas == 0]); // double.Parse(np.sum(bList, typeof(double)).ToString()) / (double.Parse(np.sum(bHas).ToString()) + 1e-5);


            var aBiasList = (aList - meanA) * comMovie;
            var bBiasList = (bList - meanB) * comMovie;

            double sim = np.sum(aBiasList * bBiasList, typeof(double)) /
                (np.sqrt(np.sum(aBiasList * aBiasList, typeof(double))) * np.sqrt(np.sum(bBiasList * bBiasList, typeof(double))) + 1e-5);

            return sim;
        }

        public List<ReMovie> userRecom(int userId)
        {
            // 需要重新做
            if (isChanged || nerbNums[userId - 1] == -1)
            {
                corMatrix[userId - 1] = np.array<double>(new double[maxUser]);

                for (int i = 0; i < maxUser; i++)
                {
                    corMatrix[userId - 1][i] = -calSimilarity(userId, i); // 存负相关度
                }

                // 按照相关度降序排列
                corIndex[userId - 1] = np.argsort<double>(corMatrix[userId - 1]);
                corMatrix[userId - 1] = corMatrix[userId - 1][corIndex[userId - 1]];

                nerbNums[userId - 1] = getNerbNum(userId);
                //Console.WriteLine("======");
                //Console.WriteLine(corMatrix[userId - 1][$"0:{nerbNums[userId - 1]}"].ToString());
            }

            int nerNum = nerbNums[userId - 1]; // 选择的近邻个数
            //Console.WriteLine(nerNum);

            double meanA = np.mean(userScores[userId - 1][!hasScores[userId - 1] == 0]);
            //Console.WriteLine("meanA: ");
            //Console.WriteLine(meanA);

            var preScores = np.ones<double>(maxMovie) * meanA;

            double totalSim = np.sum(corMatrix[userId - 1][$"0:{nerNum}"], typeof(double));
            //Console.WriteLine(totalSim);

            for (int i = 0; i < nerNum; i++)
            {
                int userB = corIndex[userId - 1][i]; // B序号
                double simB = corMatrix[userId - 1][i];  // B相关度
                var bList = userScores[userB]; // B给分矩阵

                double meanB = np.mean(bList[!hasScores[userB] == 0]);
                preScores = preScores + simB / (totalSim + 1e-5) * (bList - meanB) * hasScores[userB];
            }
            preScores *= -1;
            // 电影按打分降序排列
            var movieIndexs = np.argsort<double>(preScores);
            var haveScored = hasScores[userId-1][movieIndexs];
            preScores = -1 * preScores[movieIndexs];

            movieIndexs = movieIndexs[haveScored == 0];
            preScores = preScores[haveScored == 0];

            // 推荐的电影序列
            List<ReMovie> reMovies = new List<ReMovie>();

            for (int i = 0; i < 10; i++)
            {
                int movieIndex = movieIndexs[i];
                double pre = preScores[i];
                Movie movie = movieList[movieIndex];
                ReMovie reMovie = new ReMovie(movie.Index, movie.Name, movie.Date, movie.Url, pre);
                reMovies.Add(reMovie);
                // Console.WriteLine($"{movie.Index}、 {movie.Name}, preScore：{pre.ToString("f3")}");
                // Console.WriteLine($"date: {movie.Date}");
                // Console.WriteLine($"url: {movie.Url}");
                // Console.WriteLine("------------------");
            }

            return reMovies;
        }

        public void changeScore(int userId, int movieId, int score)
        {
            userScores[userId - 1, movieId - 1] = score;
            hasScores[userId - 1, movieId - 1] = 1;

            isChanged = true; // 标记需要重新计算
        }
    }
}
