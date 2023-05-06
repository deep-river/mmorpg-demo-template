using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using GameServer.Network;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Services;
using GameServer.Managers;
namespace GameServer
{
    class GameServer
    {
        NetService network;
        Thread thread;
        bool running = false;
        public bool Init()
        {
            // 初始化各项服务
            int Port = Properties.Settings.Default.ServerPort;
            network = new NetService();
            network.Init(Port);
            DBService.Instance.Init();
            DataManager.Instance.Load();
            MapService.Instance.Init();
            UserService.Instance.Init();
            ItemService.Instance.Init();
            QuestService.Instance.Init();
            FriendService.Instance.Init();
            thread = new Thread(new ThreadStart(this.Update)); // 单独开一个线程实现Update

            return true;
        }

        public void Start()
        {
            network.Start();
            running = true;
            thread.Start();
        }


        public void Stop()
        {
            running = false;
            thread.Join();
            network.Stop();
        }

        public void Update()
        {
            while (running)
            {
                var mapManager = MapManager.Instance;
                Time.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
                mapManager.Update(); // 怪物刷新管理
            }
        }
    }
}
