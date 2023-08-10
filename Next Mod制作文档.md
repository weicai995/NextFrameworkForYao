# **Next Mod制作文档**

1. #### 基于Next框架的Mod是什么？

  Next本身需要BepinEx进行加载。使用Next框架可以加载基于规则的更简易的mod。

2. #### 新建 Mod

  ```
  plugins
  └── Next
   └── mod测试                 	    PatchMod文件夹
       ├── Assets                  Mod美术资源文件夹 （用于存放spine等美术资源）
       │	 └── TalentPics          心法大图片
       │	 └── TalentIcons         心法小图标
       │	 └── UI(Class)           游戏内与职业相关的一些UI图标（主要为卡面背景，费用图标等）
       │	 └── UI                  一些其他的UI(主要为选人界面的UI)
       │	 │	 └── StandPic	     选人界面的大立绘，可以是组成spine的atlas，json，png文件 或直接放	   │   │   │				   png图片
       │	 │	 └── NamePic		 选人界面的名字图片
       │	 │	 └── BtnPic          选人界面的按钮图片，需要两张分别为选中时与未选中的状态。
       │	 └── CardArts			 卡图
       │	 └── Skills              角色技能的图标
       │	 └── Items               道具的图标
       │	 └── EventPictures       事件相关的图片
       │	 └── UnitModel           新角色模型的文件夹，可以使用spine或者png图片，具体可以查看5.2
       │	 │	 └── Spine           存放spine美术资源
       │	 │	 └── Pic             存放角色png图片
       ├── Config                  Mod配置文件夹
       │   └── modConfig.json      Mod描述文件
       ├── Data                    游戏数据文件夹 （用于存放导表后得到的json文件）
       └── Script                  脚本文件夹 （用于存放游戏脚本）
  ```

  Next Patch的文件夹以 mod 开头，如 mod测试
  只有以"mod"开头的文件夹会被加载。

3. #### Mod描述文件

   Mod描述文件是用于描述mod名称、作者、版本与介绍。
   该文件本身不影响mod加载，但是完善的mod描述文件有利于留下mod作者的信息，以及进行版本管理等。

   ##### 3.1 创建描述文件

   新建json文件，保存至 mod测试/Config/modConfig.json 的路径中

文件结构：
modConfig.json

{
    "Name" : "测试Mod",
    "Author" : "佚名",
    "Version" : "1.0.0",
    "Description" : "测试用的Mod。",
    "Settings" : [ ... ]
}

4. #### 数据文件

  ##### 4.1 数据类型

  Next使用由导表工具生成的json文件

  ##### 4.2 表格与导表工具

  在创意工坊中订阅了mod前置插件BepInEx后，可以于下面路径中找到7张表格文件和导表工具.bat

  ```
  路径 ：   安装盘:\SteamLibrary\steamapps\workshop\content\1915510\2981800108\BepInEx\Excel
  ```

  

  ###### 4.2.1 数据含义参考

  在参考用表文件夹中可以找到游戏当前版本所有官方内容的表项数据作为参考。
  具体含义请参考官方的Excel开源文件

  ###### 4.2.2 导表工具

  运行导表工具会把上述路径中7张xlsx的数据作为json格式导入 .\Json 文件夹内。 之后便可以将其中所有的json文件移动到mod中的Data文件夹下。

#### 5.美术资源文件

降妖散记的美术资源主要分为两种。一种是普通的png图片资源，用于卡图等。另一种则是spine资产用于角色与敌人的模型。

##### 5.1 Png图片资产

请直接放置于Mod的Assets文件夹中，需要注意的是游戏中的卡图资源大小请使用 351 x 253 像素

##### 5.2 Spine资产

Spine是一款用于制作骨骼动画的软件，其导出的骨骼动画资源格式（按照json格式输出）一般如下:

```
.json 骨骼数据
.png 图集纹理
.atlas.txt 图集数据
```

请放置于如下路径

```
Assets\spine\名称\名称.json
Assets\spine\名称\名称.png
Assets\spine\名称\名称.atlas.txt
```
