# Web服务开发实验

[TOC]



## 一、电影推荐系统前端

### 1. 前端所用技术

本项目前端 `UI` 采用 `vue` 框架和 `element-ui` 组件库实现，数据传输使用 `Ajax` 实现。

* `vue` 是一套用于构建用户界面的渐进式 `Javascript`  框架 ，开发者只需要关注视图层， 它不仅易于上手，还便于与第三方库或既有项目的整合。是基于`MVVM`（`Model-View-ViewModel` 即：视图层-视图模型层-模型层）设计思想的前端框架。它提供`MVVM`数据双向绑定的库，专注于`UI`层面。而 `element` 是基于 `vue` 实现的一套不依赖业务的`UI`组件库，提供了丰富的PC端组件，减少用户对常用组件的封装，降低了开发的难易程度。本项目的前端页面`UI`主要使用了 `element-ui` 库的如下组件：

  > a) <el-container>**页面布局**：在头部放置了搜索框，在侧边栏放置了功能选择菜单，在底部放置了换页组件，然后将所有的电影显示在中间的主题部分，这样会使整个页面布局更清晰，也便于后期的维护。

  > b) <el-card>以块为单位**显示电影**：为了能够更加清晰地显示相关信息，使用了card组件，将每部电影放在一个card中，card内部使用<el-row>和<el-col>来布局，更规整，也更有序，能够适应各种不同长度的电影显示。

  > c) <el-menu>菜单栏实现**功能选择**，对于互斥的功能（查看电影推荐列表还是已打分的列表），使用菜单组件来实现，点击相应的菜单会改变activate的值，并且能够出发不同的函数，以发送相关请求。

  > d) 还使用了<el-rate>来实现**打分**，<el-pagination>来实现**分页**等。

  在 `element-ui` 组件以外，前端还利用了 `vue` 的一些重要机制，如使用 `v-model` 来动态**双向绑定**组件中的显示输入和内存中的变量值；使用`v-for`来基于一个数组**渲染**一系列的组件，使用 `template` 模板占位符包裹元素来将 `<span>` 标签内的内容也循环渲染而不必对每个 `span` 都使用 `v-for`。

* `ajax`：即异步 ` JavaScript`  和 `XML`。`ajax` 是一种用于创建快速动态网页的技术。通过在后台与服务器进行少量数据交换，`ajax` 可以使网页实现异步更新。这意味着可以在不重新加载整个网页的情况下，对网页的某部分进行更新。而传统的网页(不使用 `ajax` )如果需要更新内容，必需重载整个网页面。本项目使用 `ajax` 发送 `post/get` 请求给后端，如果是 `post` 请求还要携带相应参数。并设置好 `success` 回调函数，对后端相应的结果进行接受，绑定相应数据，并以弹窗等形式做出响应。

---

### 2. 前端UI

前端主要分为四个部分：头部的信息输入区域、侧边的功能选择区域、中间的电影显示区域，以及底部的页面跳转区域：
<div align=center>
<img src=".\image\前端页面" />
</div>
  
<div align=center>
<img src=".\image\分页区" />
</div>

---

### 3. 前端交互逻辑

前端主要实现了如下几个交互功能：

#### 3.1 推荐按钮

用户首先要在推荐搜索框中输入`ID`号，然后点击“电影推荐”按钮后，系统首先会获取侧边功能菜单栏激活状态，如果当前激活的是“我的推荐”，那么会调用“获取推荐”函数，如果当前激活的是“我的打分”，那么会调用“获取打分”函数。

此外，点击按钮左边的“🔍”图标，或者按下键盘上的回车键也能达到相同的效果，方便了用户的操作。

#### 3.2 侧边选择菜单栏

* 点击“我的推荐”功能，前端会使用 `ajax` 发送 `get` 请求，获得推荐电影列表。对应 `api` 的 `url` 为 `'https://localhost:5001/api/ReSys/recommendMovies/' + this.userId`。指定 `contentType` 为 `application/json` 。设置回调函数：

  * 如果后端返回的 `status` 为 404，说明推荐失败，弹出相应的警告信息
  
    <div align=center>
    <img src=".\image\警告" />
    </div>
  
  * 如果后端返回的 `status` 为 202，说明推荐成功。弹出成功提示框，页面滚动到最顶部，将新闻数量赋值给 `movieNum` 以确定分的页数，跳转到第一页，并将电影列表绑定到相关变量 `showList` 上

    <div align=center>
    <img src=".\image\成功" />
    </div>

    可以看到，当前向3号用户共推荐50部电影，每部电影都使用一个`card`进行展示，展示内容包括：电影的名字，序号、上映时间，推荐指数，以及与推荐指数对应的⭐数量。

* 点击“我的打分”功能，前端也会使用 `ajax` 发送 `get` 请求，获得已经打分的电影列表。这次调用的 `api` 的 `url` 是 `'https://localhost:5001/api/ReSys/scoredMovies/' + that.userId`。同样，如果后端返回的 `status` 为 404，说明推荐失败，弹出相应的警告信息；而如果后端返回的 `status` 为 202，说明获取成功。弹出成功提示框，页面滚动到最顶部，将已打分新闻数量赋值给 `movieNum` 以确定分的页数，跳转到第一页，并将电影列表绑定到相关变量 `showList` 上。

    <div align=center>
    <img src=".\image\已打" />
    </div>

#### 3.3 打分组件

打分组件的五颗⭐可以进行点击，点击相应的星或半星后，会使用 `ajax` 发送 `post` 请求，更新后端数据库。调用的 `api` 的 `url` 是 `'https://localhost:5001/api/ReSys/scoreMovie'`。向后端传递的参数包括：用户ID、电影ID以及分数，使用 `JSON.stringify()` 函数将参数列表化为 `json` 格式。后端执行相应的更新后，返回一个状态：如果返回状态为 202，提示打分成功；如果返回状态为 404，警告操作失败。

 <div align=center>
 <img src=".\image\打分" />
 </div>

如图所示，电影 `Silence of the Lambs, The(1991)` 原本的推荐指数是 `3.233`，现在点击打分控件，打分为 `4.5` 分，打分成功。现在重新获取推荐电影可以发现，刚刚打分的电影，已经不在推荐列表上了：
  
 <div align=center>
 <img src=".\image\成功" />
 </div>

重新获取已打分列表可以发现，打分的电影数从 `58` 变为了 `59`， `Silence of the Lambs, The(1991)` 也已在列：
 
 <div align=center>
 <img src=".\image\已打_1" />
 </div>

#### 3.4 界面底部页码

页面滑动到最下端时，会出现页面跳转控件
 
 <div align=center>
 <img src=".\image\页码" />
 </div>

点击相应的页数，前端会使用 `ajax` 发送 `get` 请求，调用的 `api` 为 `'https://localhost:5001/api/ReSys/pageChanged/' + that.currentPage + '/' + that.showType`。其中，`currentPage` 表示要跳转的页码数，`showType` 代表当前要展示的电影种类（推荐电影或已打分电影）。接收到返回值后，首先将页面滚动到顶部，然后将电影列表绑定到相关变量 `showList` 上，如下图显示了我的打分里第二页的电影：
  
 <div align=center>
 <img src=".\image\第二页" />
 </div>

#### 3.5 返回顶部按钮

为了便于用户操作，在前端添加了“返回顶部”按钮，起初不会出现，当页面向下滚动超过 `500px` 时，就会出现在左下角，点击后页面自动在 `0.5s` 内滚动到最顶端：
  
 <div align=center>
 <img src=".\image\返回顶端" />
 </div>

## 二、REST API的设计与实现

### 1.REST API概述

REST（Representational State Transfer）表征状态转移，是由Roy Fielding提出的一种URL设计风格。Web本质上是由各种各样的资源组成，并且资源由URI进行唯一标识。RESTful Web服务（也成为RESTful Web API）是一个使用HTTP并遵循REST原则的Web服务。其从三个方面对资源进行的定义：

- URI, 例如 http://example.com/resources
- Web服务接受与返回的互联网媒体类型，比如：JSON，XML ，YAML 等
- Web服务在该资源上所支持的一系列请求方法（比如：POST，GET，PUT或DELETE）
  基于上述三点即可设计出遵循REST原则的Web服务。

### 2.REST API设计

在个性化电影推荐系统中，对于用户而言可以细分为以下几个功能：

- 用户获取推荐的电影列表
- 用户对电影进行打分
- 用户获取打过分的电影列表

而对于前端页面展示电影列表而言，一次不能展示过多的电影，因此需要**分页展示**，所以要求后端可以将电影列表进行缓存，每次依据页数返回一部分的电影列表，因此后端还需要实现一个换页服务功能：

- 列表换页获取新的电影列表缓存

综上所述，在该系统中一共要设计4个API，下面分别进行分析和介绍。

#### 2.1 用户获取推荐的电影列表

从服务端获取推荐的电影列表仅需要一个参数即用户的唯一标识符`userId`，基于此可以将URI设计为

```c#
https://localhost:5001/api/ReSys/recommendMovies/{userId}
```

并且由于该操作不会对服务端所存储的数据产生影响，因此可以将请求方法设计为**`GET`**

对于互联网媒体类型，选择JSON作为载体：

- 从前端传递到后端的数据（即`userId`）在URI中已经含有，以`userId`为1为例，则URI为：

```C#
https://localhost:5001/api/ReSys/recommendMovies/1
```

- 从后端传递到前端的数据，以一个`status`标识状态（202标识正常），`movieList`标识电影列表（`index`标识顺次，`name`标识电影名称，`date`标识电影日期，`url`标识电影链接，`score`标识电影推荐分数），该URI返回10条电影，且后端会缓存50条电影：

```json
{"status": 202, "movieList": [{"index": myindex, "name": myname, "date": mydate, "url": myurl, "score": myscore}]}
```

#### 2.2 用户对电影进行打分

用户对电影打分，需要向服务端传递多个信息，包括用户的标识符`userId`，电影标识符`movieId`以及分值`score`，所以需要使用JSON进行传递数据，数据不直接在URI中标识，因此将URI设计为

```c#
https://localhost:5001/api/ReSys/scoreMovie
```

并且由于该操作会对服务端所存储的数据产生影响，因此可以将请求方法设计为**`POST`**
对于互联网媒体类型，选择JSON作为载体：

- 从前端传递到后端的数据，其JSON格式为：

```C#
{"userId": myuserId, "movieId": movieId, "score": myscore}
```

- 从后端传递到前端的数据，以一个标识符status标识后端执行状态（202标识成功，404标识失败）：

```json
{"status": 202}
```

#### 2.3 用户获取打过分的电影列表

从服务端获取打过分的电影列表，其与URI（**用户获取推荐的电影列表**）类似，仅需要一个参数即用户的唯一标识符`userId`，基于此可以将URI设计为

```c#
https://localhost:5001/api/ReSys/scoredMovies/{userId}
```

并且由于该操作不会对服务端所存储的数据产生影响，因此可以将请求方法设计为**`GET`**

对于互联网媒体类型，选择JSON作为载体：

- 从前端传递到后端的数据（即`userId`）在URI中已经含有，以`userId`为1为例，则URI为：

```C#
https://localhost:5001/api/ReSys/scoredMovies/1
```

- 从后端传递到前端的数据，以一个`status`标识状态（202标识正常），`movieList`标识电影列表（`index`标识顺次，`name`标识电影名称，`date`标识电影日期，`url`标识电影链接，`score`标识电影推荐分数），以`totalNum`标识打过分的电影个数，便于前端进行页数判断，该URI返回的电影数量至多为10条，并且在后端缓存所有打过分的电影，：

```json
{"status": 202, "movieList": [{"index": myindex, "name": myname, "date": mydate, "url": myurl, "score": myscore}], "totalNum": mytotalNum}
```

#### 2.4 列表换页获取新的电影列表缓存

从服务端获取缓存的电影列表，前端需要传递两个参数：`currentPage`标识跳转到的页数，`listType`标识电影列表类别，其为1表示列表为推荐的电影列表，其为2表示列表为打过分的电影列表，因此URI可以设计为

```c#
https://localhost:5001/api/ReSys/pageChanged/{currentPage}/{listType}
```

并且由于该操作不会对服务端所存储的数据产生影响，因此可以将请求方法设计为**`GET`**

对于互联网媒体类型，选择JSON作为载体：

- 从前端传递到后端的数据在URI中已经含有，以`currentPage`为1，`listType`为1为例，则URI为：

```C#
https://localhost:5001/api/ReSys/pageChanged/1/1
```

- 从后端传递到前端的数据，以一个`status`标识状态（202标识正常），`movieList`标识电影列表（`index`标识顺次，`name`标识电影名称，`date`标识电影日期，`url`标识电影链接，`score`标识电影推荐分数）：

```json
{"status": 202, "movieList": [{"index": myindex, "name": myname, "date": mydate, "url": myurl, "score": myscore}]}
```

### 3.REST API实现

#### 3.1 ASP.NET 基本概念

在个性化电影推荐系统中，REST API是基于ASP.NET实现的。ASP.NET具备开发网站应用程序的一切解决方案，包括验证、缓存、状态管理、调试和部署等全部功能。在代码撰写方面特色是将页面逻辑和业务逻辑分开，它分离程序代码与显示的内容，让丰富多彩的网页更容易撰写。同时使程序代码看起来更洁净、更简单。

在ASP.NET MVC程序中除了一些静态的资源如html，js，图片，css等等，其它每一个URL请求都会被相应的**Controller**处理并做出相应的响应。**Controller**是MVC中连接**Model**和**View**的中间桥梁，**Controller**中文意思是控制器，也就是起到一个获取请求信息，控制返回结果，控制跳转页面等的使用。当**Controller**中的一个具体**Action**接收到URL请求，会调用我们的业务代码，操作领域对象，最后根据得到的结果选择相应的视图或者JSON字符串给前端客户端。

#### 3.2 基于Controller实现REST API

在ASP.NET MVC程序中，**Controller**的实现决定了API的具体形式和功能。因此需要创建一个专门服务于电影推荐的类`ReSysController`：

```C#
public class ReSysController : Controller
```

在该类中，其含有四个属性：

```c#
private List<ReMovie> recommendMovies;
private List<ReMovie> scoredMovies;
private bool scored;
private int userId;
```

其中前两个属性用于缓存基于算法搜索到的电影列表，后两个数据用于缓存从前端传递而来的数据。

##### 3.2.1 用户获取推荐的电影列表API实现

创建函数

```c#
[HttpGet("recommendMovies/{userId}")]
public ContentResult RecommendMovies(int userId){}
```

在该函数中实现了返回电影推荐列表的有关逻辑，具体而言即依据前端传递而来的`userId`，通过工厂产生的算法对象来运算获取推荐电影列表，并转换成JSON串的形式返回。

##### 3.2.2 用户对电影进行打分API实现

创建函数

```c#
[HttpPost("scoreMovie")]
public ContentResult ScoreMovie([FromBody] ScorePara para)
```

在该函数中，其参数`para`是ASP.NET解析前端传递而来的JSON对象后所获得的参数对象，其中包括了前端传递而来的数据，即`userId`， `movieId`，`score`，然后函数通过工厂产生的算法对象来依据这些参数执行打分功能，并将执行结果状态进行返回。

##### 3.2.3 用户获取打过分的电影列表API实现

创建函数

```c#
[HttpGet("scoredMovies/{user}")]
public ContentResult ScoredMovies(int userId)
```

在该函数中实现了返回打过分的电影列表的有关逻辑，具体而言即依据前端传递而来的`userId`，通过工厂产生的算法对象来运算获取打过分的电影列表，并转换成JSON串的形式返回。

##### 3.2.4 列表换页获取新的电影列表缓存API实现

创建函数

```c#
[HttpGet("pageChanged/{currentPage}/{listType}")]
public ContentResult PageChanged(int currentPage, int listType)
```

在该函数中实现了依据页数返回电影列表缓存的功能，具体而言即依据前端传递而来的两个参数`currentPage`和`listType`，从相应的电影缓存列表中切片出电影属于该页面数的电影列表，并且转换成JSON串的形式返回。
