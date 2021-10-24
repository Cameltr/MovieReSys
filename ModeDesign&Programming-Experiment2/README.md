# 设计模式实验
## 目录

* [设计模式实验](#设计模式实验)
  * [目录](#目录)
  * [总述](#总述)
  * [基于用户的电影推荐算法](#基于用户的电影推荐算法)
    * [代码文件](#代码文件)
    * [核心思想：](#核心思想)
  * [观察者模式](#观察者模式)
    * [代码文件](#代码文件-1)
    * [核心思想](#核心思想-1)
    * [工作流程](#工作流程)
    * [运行实例](#运行实例)
  * [简单工厂模式](#简单工厂模式)
    * [代码文件](#代码文件-2)
    * [核心思想](#核心思想-2)
    * [工作流程](#工作流程-1)
## 总述
在个性化电影推荐系统中，对于核心功能，我们使用了两种设计模式：**观察者模式**和**简单工厂模式**：
- **观察者模式**：观察者模式用于响应用户的行为。在本次实验中，我们使用该模式对用户打分这一行为进行设计，当用户打分后，观察者则会更新用户的打分矩阵。
- **简单工厂模式**：工厂模式用于将对象的创建和使用进行分离。在本次实验中，我们将推荐算法当作产品。
## 基于用户的电影推荐算法
### 代码文件
- Recommend.cs

### 核心思想：
**人以群分**：基于用户的最近邻推荐，某用户 ![](http://latex.codecogs.com/gif.latex?a) 可能只对电影库中的一小部分电影给过评分，但是可以通过历史评分来找出与 ![](http://latex.codecogs.com/gif.latex?a) 有相似兴趣的人，并根据他们对其他电影的评分**共同给出**一个 ![](http://latex.codecogs.com/gif.latex?a) 可能感兴趣的电影列表。算法步骤如下：

1. 对于请求推荐的用户 ![](http://latex.codecogs.com/gif.latex?a)，计算系统中的每个用户 ![](http://latex.codecogs.com/gif.latex?b) 和 ![](http://latex.codecogs.com/gif.latex?a) 之间的相似度 ![](http://latex.codecogs.com/gif.latex?s_{a,b}) ：
   <br/>
<div align=center>
<img src=".\pngs\f1.svg" />
</div>
<br/>

   其中，![](http://latex.codecogs.com/gif.latex?r_{a,i})、![](http://latex.codecogs.com/gif.latex?r_{b,i}) 表示用户 ![](http://latex.codecogs.com/gif.latex?a) 或用户 ![](http://latex.codecogs.com/gif.latex?b) 对电影 ![](http://latex.codecogs.com/gif.latex?i) 的评分，![](http://latex.codecogs.com/gif.latex?\\overline{r}_a)、 ![](http://latex.codecogs.com/gif.latex?\\overline{r}_b) 表示用户 ![](http://latex.codecogs.com/gif.latex?a) 或用户 ![](http://latex.codecogs.com/gif.latex?b) 所有历史评分的均值，![](http://latex.codecogs.com/gif.latex?I_{a,b}) 表示用户 ![](http://latex.codecogs.com/gif.latex?a) 和用户 ![](http://latex.codecogs.com/gif.latex?b) 打过分的电影的交集。

   

2. 得到了用户 ![](http://latex.codecogs.com/gif.latex?a) 与所有其他用户之间的相似度后，我们采用 ![](http://latex.codecogs.com/gif.latex?k-means) 算法对所有相似度进行二分类，然后取出相似度高的一簇作为用户 ![](http://latex.codecogs.com/gif.latex?a) 的近邻用户，认为他们与 ![](http://latex.codecogs.com/gif.latex?a) 的相似度较高，因此用来预测 ![](http://latex.codecogs.com/gif.latex?a) 用户对其他电影的评分。

   

3. 因为每个人的评分习惯不同：有些人喜欢给高评分，比如满意给5分，不满意给3分；有些人则比较鲜明，满意给5分，不满意给1分。所以，预测时考虑偏置项，用每个人减去均值后的**偏差**来衡量喜欢程度，则预测用户 ![](http://latex.codecogs.com/gif.latex?a) 对某电影 ![](http://latex.codecogs.com/gif.latex?i) 的评分 ![](http://latex.codecogs.com/gif.latex?\widehat{r}_{a,i}) ：
 <br/>
 <div align=center>
<img src=".\pngs\f2.svg" />
 </div>
 <br/>

   其中， ![](http://latex.codecogs.com/gif.latex?N_a) 表示由步骤 2 得到的用户 ![](http://latex.codecogs.com/gif.latex?a) 的近邻用户集合，![](http://latex.codecogs.com/gif.latex?\overline{r}_a)、 ![](http://latex.codecogs.com/gif.latex?\overline{r}_b) 、![](http://latex.codecogs.com/gif.latex?s_{a,b})、![](http://latex.codecogs.com/gif.latex?r_{b,i}) 定义同上。



4. 算出用户 $a$ 对每个剩余电影的预测评分后，按照降序排列，取出前若干个的电影序号，并根据序号到系统电影列表中取出相应电影的信息，即得到推荐电影列表。控制台运行实例如下：

<div align=center>
<img src=".\pngs\recommend_example.png" width="700"/>
<div>

## 观察者模式
### 代码文件
- Subject.cs
- Observer.cs
- UserMotionSubject.cs
- UserMotionObserver.cs

### 核心思想
该观察者模式的简易UML图为：

<div align=center>
<img src=".\pngs\observer_uml.png" width="700"/>
<div>

其中Subject和Observer为抽象类，分别是物品和观察者。UserMotionSubject继承Subject类，用于表示用户的行为，UserMotionObserver继承Observer类，通过观察用户的行为做出相应的举动。
### 工作流程
当UserMotionSubject类产生打分行为后，说明该用户进行的打分，于是该类会使用Notify()函数来告知UserMotionObserver，然后UserMotionObserver再通过工厂类获得推荐算法产品(MovieReSysAlgorithm)来修改用户打分矩阵。

### 运行实例
从推荐算法的演示中可以看到在没有进行任何打分时，基于用户3原始数据的推荐结果，现在我们让用户3给电影50(Star War)打上5分，然后再次进行推荐，运行结果如图所示

<div align=center>
<img src=".\pngs\after_recommend_example.png" width="700"/>
<div>

## 简单工厂模式
### 代码文件
- MovieReSysFactory.cs
### 核心思想
工厂模式将产品的生产和使用进行了区分。在该系统中，使用简单工厂模式将推荐算法类对象的创建和使用进行区分。我们只实现了一种推荐算法。该简单工厂模式的UML图如图所示

<div align=center>
<img src=".\pngs\factory_uml.png" width="700"/>
<div>

### 工作流程
在MovieReSysFactory类中，使用getProduct()函数可以获得推荐算法产品对象，为了避免重复生产，使用一个字典(productMap)来存储已经生产过的推荐算法产品，并且使用算法产品的对象名作为键来进行产品的获取。
同时，将getProduct()函数和productMap设置为静态函数和静态变量，从而可以直接通过类名来获取算法产品

```c#
MovieReSysFactory.getProduct("MovieReSysAlgorithm")
```