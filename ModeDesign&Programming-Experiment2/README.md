## 基于用户的电影推荐算法
![](http://latex.codecogs.com/gif.latex?\\sigma=\sqrt{\frac{1}{n}{\sum_{k=1}^n(x_i-\bar{x})^2}})
* 核心思想：**人以群分** -> 基于用户的最近邻推荐，某用户 $a$ 可能只对电影库中的一小部分电影给过评分，但是可以通过历史评分来找出与 $a$ 有相似兴趣的人，并根据他们对其他电影的评分**共同给出**一个 $a$ 可能感兴趣的电影列表。算法步骤如下：

1. 对于请求推荐的用户 $a$，计算系统中的每个用户 $b$ 和 $a$ 之间的相似度 $s_{a,b}$ ：
   ![](http://latex.codecogs.com/gif.latex?\\s_{a,b}=\frac{\sum_{ i \in I_{a,b} } \left ( r_{a,i} - \overline{r}_a \right ) \left ( r_{b,i} - \overline{r}_b \right )}{\sqrt{\sum_{ i \in I_{a,b} } \left ( r_{a,i}-\overline{r}_a \right )^2 }\sqrt{\sum_{ i \in I_{a,b} } \left ( r_{b,i}-\overline{r}_b \right )^2 }})
   其中，$r_{a,i}$、$r_{b,i}$ 表示用户 $a$ 或用户 $b$ 对电影 $i$ 的评分，$\overline{r}_a$、 $\overline{r}_b$ 表示用户 $a$ 或用户 $b$ 所有历史评分的均值，$I_{a,b}$ 表示用户 $a$ 和用户 $b$ 打过分的电影的交集。

   

2. 得到了用户 $a$ 与所有其他用户之间的相似度后，我们采用 $k-means$ 算法对所有相似度进行二分类，然后取出相似度高的一簇作为用户 $a$ 的近邻用户，认为他们与 $a$ 的相似度较高，因此用来预测 $a$ 用户对其他电影的评分。

   

3. 因为每个人的评分习惯不同：有些人喜欢给高评分，比如满意给5分，不满意给3分；有些人则比较鲜明，满意给5分，不满意给1分。所以，预测时考虑偏置项，用每个人减去均值后的**偏差**来衡量喜欢程度，则预测用户 $a$ 对某电影 $i$ 的评分 $\widehat{r}_{a,i}$ ：
   $$
   \widehat{r}_{a,i}=\overline{r}_a + \frac{1}{\sum_{b\in N_a}^{}s_{a,b}} \sum_{b\in N_a}^{}s_{a,b}\left (r_{b,i} -\overline{r}_b \right)
   $$
   其中， $N_a$ 表示由步骤 2 得到的用户 $a$ 的近邻用户集合，$\overline{r}_a$、 $\overline{r}_b$ 、$s_{a,b}$、$r_{b,i}$ 定义同上。



4. 算出用户 $a$ 对每个剩余电影的预测评分后，按照降序排列，取出前若干个的电影序号，并根据序号到系统电影列表中取出相应电影的信息，即得到推荐电影列表。控制台运行实例如下：

<div align=center>
<img src=".\recommend_example.png" width="700"/>
<div>
