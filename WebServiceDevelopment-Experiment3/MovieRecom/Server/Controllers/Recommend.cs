using System;
using NumSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text.RegularExpressions;
using Server.Models;

namespace Server.Controllers
{
    class Recommend
    {
        private NDArray userScores = null;  // 打分的矩阵
        private Dictionary<int, NDArray> corMatrix = new Dictionary<int, NDArray>();
        private Dictionary<int, NDArray> corIndex = new Dictionary<int, NDArray>();
        private List<Movie> movieList = null;
        private BinaryFormatter formatter = new BinaryFormatter();
        private int[] nerbNums = null;
        bool isChanged = false;

        private int maxUser = 943;
        private int maxMovie = 1682;
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
            double[] readData = null;
            using (FileStream readstream = new FileStream(".\\Data\\tamp.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                readData = (double[])formatter.Deserialize(readstream);
            }
            this.userScores = np.array<double>(readData).reshape(new int[] { maxUser, maxMovie });

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

        public Recommend(string path = @".\Data\ml-100k\")
        {
            loadData(path);
            nerbNums = new int[maxUser];
            for (int i = 0; i < maxUser; i++) nerbNums[i] = -1;
        }

        public double calSimilarity(int userA, int userB)
        {
            var aList = userScores[userA - 1];
            var bList = userScores[userB - 1];

            var aHas = np.ones<int>(aList.shape); // hasScores[userA - 1];
            aHas[aList == -1] = 0;
            var bHas = np.ones<int>(bList.shape); // hasScores[userB - 1];
            bHas[bList == -1] = 0;

            var comMovie = aHas * bHas;
            int comNum = np.sum(comMovie);

            if (comNum < minComMoviesNum) return 0;

            double meanA = np.mean(aList[!(aList == -1)]); // double.Parse(np.sum(aList, typeof(double)).ToString()) / (double.Parse(np.sum(aHas).ToString()) + 1e-5);
            double meanB = np.mean(bList[!(bList == -1)]); // double.Parse(np.sum(bList, typeof(double)).ToString()) / (double.Parse(np.sum(bHas).ToString()) + 1e-5);


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

                for (int i = 1; i <= maxUser; i++)
                {
                    corMatrix[userId - 1][i - 1] = -calSimilarity(userId, i); // 存负相关度
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

            double meanA = np.mean(userScores[userId - 1][!(userScores[userId - 1] == -1)]);
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

                double meanB = np.mean(bList[!(userScores[userB] == -1)]);


                var bHas = np.ones<int>(bList.shape); // hasScores[userB - 1];
                bHas[bList == -1] = 0;
                preScores = preScores + simB / (totalSim + 1e-5) * (bList - meanB) * bHas;
            }
            preScores *= -1;
            // 电影按打分降序排列
            var movieIndexs = np.argsort<double>(preScores);
            var haveScored = userScores[userId - 1][movieIndexs];
            preScores = -1 * preScores[movieIndexs];

            movieIndexs = movieIndexs[haveScored == -1];
            preScores = preScores[haveScored == -1];

            // 推荐的电影序列
            List<ReMovie> reMovies = new List<ReMovie>();

            for (int i = 0; i < 50; i++)
            {
                int movieIndex = movieIndexs[i];
                double pre = preScores[i];
                pre = Math.Round(pre,3);
                Movie movie = movieList[movieIndex];
                ReMovie reMovie = new ReMovie(movie.Index, movie.Name, movie.Date, movie.Url, pre);
                reMovies.Add(reMovie);

            }

            return reMovies;
        }

        public void changeScore(int userId, int movieId, double score)
        {
            userScores[userId - 1, movieId - 1] = score;
            Console.WriteLine(userId + " 给 " + movieId + " 打的分变为 " + userScores[userId-1, movieId-1]);

            isChanged = true; // 标记需要重新计算

            using (FileStream stream = new FileStream(".\\data\\tamp.txt", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, userScores.ToArray<double>());

            }
        }

        public List<ReMovie> getScoredMovies(int userId)
        {
            List<ReMovie> scoredMovies = new List<ReMovie>();
            for(int movieId = 0; movieId < maxMovie; movieId++)
            {
                if((int)(double)userScores[userId - 1][movieId].GetValue() != -1)
                {
                    //Console.WriteLine(userId + " 给 " + (movieId+1) + " 打的分为 " + userScores[userId-1, movieId]);
                    Movie movie = movieList[movieId];
                    ReMovie reMovie = new ReMovie(movie.Index, movie.Name, movie.Date, movie.Url, userScores[userId - 1, movieId]);
                    scoredMovies.Add(reMovie);
                }
            }
            return scoredMovies;
        }
    }
}
