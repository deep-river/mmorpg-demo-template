# 项目简介

Unity引擎下开发的个人MMORPG游戏Demo，包括项目前后端代码及资源文件  

视频演示地址: (待更新)

# 文件目录结构

├─Src  
│&nbsp;&nbsp;├─Client - Unity客户端项目文件夹  
│&nbsp;&nbsp;├─Data - 策划配置表及转表工具  
│&nbsp;&nbsp;├─Lib - GameServer引用的Common等项目  
│&nbsp;&nbsp;└─Server  
│&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;└─GameServer - C#服务端项目文件夹  
└─Tools - ProtoBuf协议生成脚本等工具  

# 运行环境

Unity 2018.2.3f1  
SQL Server 2022  
Visual Studio 2022  

# 主要功能模块

### Common

- 数据包处理 & 消息分发模块  

### Server

- 网络模块  
- 数据库管理服务  
- 用户管理服务  
- 场景管理服务  
- 道具管理服务  
- 任务管理服务  
- 好友管理服务  
- 组队管理服务  

### Client

- 协议通信模块  
- DataManager - 本地数据加载模块  
- UserService - 用户管理模块(处理登录、注册等事件)  
- SceneManager & MapService - 场景管理模块(处理角色进入、离开场景等事件)  
- BagManager - 背包管理模块  
- ShopManager - 商店管理模块  
- ItemService & ItemManager - 道具系统  
- NpcManager - Npc管理模块  
- QuestService & QuestManager - 任务系统  
- FriendService & FriendManager - 好友系统  
- TeamService & TeamManager - 组队系统  
- UIManager & UIWindow & UIMessageBo & UIInputBox - UGUI框架及扩展组件  
