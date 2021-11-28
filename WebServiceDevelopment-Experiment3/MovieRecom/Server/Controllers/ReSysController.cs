using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using JiebaNet.Segmenter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReSysController : Controller
    {
        private static List<ReMovie> recommendMovies = new List<ReMovie>();
        private static List<ReMovie> scoredMovies = new List<ReMovie>();
        private static bool scored = false;
        private static int userId;

        public ReSysController(ILogger<ReSysController> logger)
        {

        }

        [HttpPost("scoreMovie")]
        public ContentResult ScoreMovie([FromBody] ScorePara para)
        {
            try
            {
                int userId = para.userId;
                int movieId = para.movieId;
                double score = para.score;

                scored = true;
                Console.WriteLine("是否需要重推荐：" + scored);

                ((MovieReSysAlgorithm)MovieReSysFactory.getProduct("MovieReSysAlgorithm")).UserUpdateScore(userId, movieId, score);

                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = $"{{ \"status\": \"202\" }}"
                };
            }
            catch
            {
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = $"{{ \"status\": \"404\" }}"
                };
            }

        }

        [HttpGet("recommendMovies/{userId}")]
        public ContentResult RecommendMovies(int userId)
        {
            try
            {
                recommendMovies = MovieReSysFactory.getProduct("MovieReSysAlgorithm").UserRecommend(userId);
                Console.WriteLine("推荐电影数量：" + recommendMovies.Count);
                scored = false;
                Console.WriteLine("是否需要重推荐：" + scored);
                ReSysController.userId = userId;

                List<ReMovie> subRecommendMovies = new List<ReMovie>();
                for(int i = 0; i < 10; i++)
                {
                    subRecommendMovies.Add(recommendMovies[i]);
                }

                var json = JsonConvert.SerializeObject(subRecommendMovies);

                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = $"{{ \"status\": \"202\", \"movieList\": {json}}}"
                };

            } catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = $"{{ \"status\": \"404\" }}"
                };
            }

        }

        [HttpGet("pageChanged/{currentPage}/{listType}")]
        public ContentResult PageChanged(int currentPage, int listType)
        {
            Console.WriteLine("当前页：" + currentPage + " 种类：" + listType);
            Console.WriteLine("是否需要重推荐：" + scored);
            try
            {
                if (listType == 1)
                {
                    if (scored == false)
                    {
                        List<ReMovie> subRecommendMovies = new List<ReMovie>();
                        Console.WriteLine("换页时推荐电影数量：" + recommendMovies.Count);
                        for (int i = (currentPage - 1) * 10; i < currentPage * 10; i++)
                        {
                            Console.WriteLine(i);
                            subRecommendMovies.Add(recommendMovies[i]);
                        }

                        var json = JsonConvert.SerializeObject(subRecommendMovies);

                        return new ContentResult
                        {
                            ContentType = "application/json",
                            Content = $"{{ \"status\": \"202\", \"movieList\": {json}}}"
                        };
                    }
                    else
                    {
                        recommendMovies = MovieReSysFactory.getProduct("MovieReSysAlgorithm").UserRecommend(userId);
                        scored = false;
                        List<ReMovie> subRecommendMovies = new List<ReMovie>();
                        for (int i = (currentPage - 1) * 10; i < currentPage * 10; i++)
                        {
                            subRecommendMovies.Add(recommendMovies[i]);
                        }

                        var json = JsonConvert.SerializeObject(subRecommendMovies);

                        return new ContentResult
                        {
                            ContentType = "application/json",
                            Content = $"{{ \"status\": \"202\", \"movieList\": {json}}}"
                        };
                    }
                }
                else
                {
                    List<ReMovie> subScoredMovies = new List<ReMovie>();
                    for (int i = (currentPage - 1) * 10; i < Math.Min(currentPage * 10, scoredMovies.Count()); i++)
                    {
                        subScoredMovies.Add(scoredMovies[i]);
                    }

                    var json = JsonConvert.SerializeObject(subScoredMovies);

                    return new ContentResult
                    {
                        ContentType = "application/json",
                        Content = $"{{ \"status\": \"202\", \"movieList\": {json}}}"
                    };
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = $"{{ \"status\": \"404\" }}"
                };
            }

        }

        [HttpGet("scoredMovies/{userId}")]
        public ContentResult ScoredMovies(int userId)
        {
            try
            {
                scoredMovies = MovieReSysFactory.getProduct("MovieReSysAlgorithm").GetScoredMovies(userId);
                List<ReMovie>  subScoredMovies = new List<ReMovie>();
                for (int i = 0; i < Math.Min(10, scoredMovies.Count()); i++)
                {
                    subScoredMovies.Add(scoredMovies[i]);
                }
                var json = JsonConvert.SerializeObject(subScoredMovies);
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = $"{{ \"status\": \"202\", \"movieList\": {json}, \"totalNum\": {scoredMovies.Count()}}}"
                };
            }
            catch
            {
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = $"{{ \"status\": \"404\" }}"
                };
            }
        }
    }
}
