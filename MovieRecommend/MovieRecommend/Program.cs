using System;
using NumSharp;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Text.RegularExpressions;

namespace MovieRecommend
{
    class Program
    {
        static void Main(string[] args)
        {
            //var y1 = np.ones<double>(2);
            //var y2 = np.array<double>(new double[,] {{ 1.1, 2.2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }});
            //var y3 = np.array<double>(new double[5] { 1.1, 2.2, 3.4, 3.5, 1.2 });
            //var y4 = np.array<int>(new int[5] { 0, 0, 1, 0, 1 });
            //int[] t = new int[2] { 1, 0 };

            //y3[y4 == 0] = 100;
            //Console.WriteLine(y3.ToString());
            string path = @"D:\xbc\Fighting\otherSubject\SOFTDe\data\ml-100k\";

            Recommend recomSystem = new Recommend(path);

            Console.WriteLine("请输入要推荐的用户编号：");
            int userIndex = int.Parse(Console.ReadLine());
            Console.WriteLine("===========================");
            recomSystem.userRecom(userIndex);
            //recomSystem.userRecom(7);
            //recomSystem.userRecom(8);


        }
    }
}
