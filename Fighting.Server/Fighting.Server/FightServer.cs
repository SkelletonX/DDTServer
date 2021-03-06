﻿using Bussiness;
using Bussiness.Managers;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Events;
using Game.Base.Packets;
using Game.Logic;
using Game.Base.Managers;
using Lsj.Util.Logs;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using SqlDataProvider.BaseClass;

namespace Fighting.Server
{
	public class FightServer : BaseServer
    {
        public static readonly string Edition = CONFIG.EDITION;
        //private static bool KeepRunning = false;
        private FightServerConfig m_config;
		private bool m_running;
		private static FightServer m_instance;
        private int IsRunning = -1;
        public FightServerConfig Configuration
		{
			get
			{
				return this.m_config;
			}
		}
		public static FightServer Instance
		{
			get
			{
				return FightServer.m_instance;
			}
		}
		protected override BaseClient GetNewClient()
		{
			return new ServerClient(this);
		}
		public override bool Start()
		{
			bool result;
            IsRunning = 0;
				try
				{
                this.m_running = true;
					Thread.CurrentThread.Priority = ThreadPriority.Normal;
					AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);

                   

                    if (!this.InitSocket(IPAddress.Parse(this.m_config.FightIP), this.m_config.FightPort))
                    {
                        result = false;
                        FightServer.log.Error("初始化监听端口失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化监听端口成功!");



                    





                    if (!base.Start())
                    {
                        result = false;
                        FightServer.log.Error("基础服务启动失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("基础服务启动成功!");


                    if (!ProxyRoomMgr.Setup())
                    {
                        result = false;
                        FightServer.log.Error("启动房间管理服务失败，请检查!");
                        return result;
                    }
                    ProxyRoomMgr.Start();
                    FightServer.log.Info("启动房间管理服务成功!");

                    if (!GameMgr.Setup(0, 4))
                    {
                        result = false;
                        FightServer.log.Error("启动游戏管理服务失败，请检查!");
                        return result;
                    }
                    GameMgr.Start();
                    StartScriptComponents();
                    GameEventMgr.Notify(ScriptEvent.Loaded);
                    FightServer.log.Info("启动游戏管理服务成功!");
                    GC.Collect(GC.MaxGeneration);
                    FightServer.log.Warn("战斗服务器已启动!");
                    result = true;
                IsRunning = 1;

                }

                catch (Exception e)
                {
                    FightServer.log.Error("Failed to start the server", e);
                    result = false;
                }

                return result;
            }
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			FightServer.log.Error("Unhandled exception!\n" + e.ExceptionObject.ToString());
		}
		protected bool InitComponent(bool componentInitState, string text)
		{
			FightServer.log.Debug(text + ": " + componentInitState);
			if (!componentInitState)
			{
				this.Stop();
			}
			return componentInitState;
		}
		protected bool StartScriptComponents()
		{
			bool result;
			try
			{
				ScriptMgr.InsertAssembly(typeof(FightServer).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
				Assembly[] scripts = ScriptMgr.Scripts;
				Assembly[] array = scripts;
				for (int i = 0; i < array.Length; i++)
				{
					Assembly asm = array[i];
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
				}
				FightServer.log.Debug("Registering global event handlers: true");
				result = true;
			}
			catch (Exception e)
			{
				FightServer.log.Error("StartScriptComponents", e);
				result = false;
			}
			return result;
		}
		public override void Stop()
		{
			if (this.m_running)
			{
				try
				{
					this.m_running = false;
                    this.IsRunning = -1;
					GameMgr.Stop();
					ProxyRoomMgr.Stop();
				}
				catch (Exception ex)
				{
					FightServer.log.Error("Server stopp error:", ex);
				}
				finally
				{
					base.Stop();
				}
				FightServer.log.Warn("Fight server is stopped!");
			}
		}
		public new ServerClient[] GetAllClients()
		{
			ServerClient[] list = null;
			object syncRoot;
			Monitor.Enter(syncRoot = this._clients.SyncRoot);
			try
			{
				list = new ServerClient[this._clients.Count];
				this._clients.Keys.CopyTo(list, 0);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			return list;
		}
		public void SendToALL(GSPacketIn pkg)
		{
			this.SendToALL(pkg, null);
		}
		public void SendToALL(GSPacketIn pkg, ServerClient except)
		{
			ServerClient[] list = this.GetAllClients();
			if (list != null)
			{
				ServerClient[] array = list;
				for (int i = 0; i < array.Length; i++)
				{
					ServerClient client = array[i];
					if (client != except)
					{
						client.SendTCP(pkg);
					}
				}
			}
		}
		private FightServer(FightServerConfig config)
		{
			this.m_config = config;
		}
        public static bool IsRun => Instance?.IsRunning == 1;
        public static void StartServer()
        {
            if (FightServer.Instance?.IsRunning >= 0)
            {
                return;
            }
            FightServer.m_instance = new FightServer(new FightServerConfig());
            if (Instance.Start() == false)
            {
                Instance.IsRunning = -1;
            }
        }
        public static void StopServer() => FightServer.Instance?.Stop();

    }
}
