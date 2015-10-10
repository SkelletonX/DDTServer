﻿using System;
using Game.Base.Packets;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic.Actions;
using Game.Logic;
using System.Drawing;

namespace GameServerScript.AI.Messions
{
    public class FLNS115 : AMissionControl
    {
        private SimpleNpc someNpc = null;

        private bool isBeginHit = false;

        private bool isCreateNpcFirst = true;

        private int createNpcCount = 0;

        private int hitCount = 0;

        private int needHitCount = 6;

        private int currTurnID = 0;

        private int redNpcID = 4;

        private int maxTurnSize = 10;

        private Point[] createNpcX = { new Point(77, 573), new Point(109, 573), new Point(141, 573), new Point(173, 573), new Point(205, 573), new Point(237, 573),
                                       new Point(442,1271), new Point(479,1271), new Point(516,1271), new Point(553,1271), new Point(590,1271), new Point(627,1271),
                                       new Point(866,769), new Point(899,769), new Point(932,769), new Point(965,769), new Point(998,769), new Point(1031,769) };


        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 1870)
            {
                return 3;
            }
            else if (score > 1825)
            {
                return 2;
            }
            else if (score > 1780)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override void OnPrepareNewGame()
        {
            base.OnPrepareNewGame();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            if (!isBeginHit)
                return;
            if (isBeginHit && someNpc != null && !someNpc.IsLiving)
                hitCount++;
            KillNpc();
            CreateNpc();
        }

        public override void OnMissionEvent(GSPacketIn packet)
        {
            int type = packet.ReadInt();
            switch (type)
            {
                case 0:
                    CreateNpc();
                    break;
                case 1:
                    isBeginHit = false;
                    Reset();
                    KillNpc();
                    break;
                case 2:
                    isBeginHit = true;
                    someNpc = null;
                    Game.AddAction(new LivingCallFunctionAction(null, Skip, 4000));
                    break;
            }
        }

        public void Skip()
        {
            Game.CurrentPlayer.Skip(0);
        }

        public void CreateNpc()
        {
            if (createNpcCount <= maxTurnSize)
            {
                if (isCreateNpcFirst)
                {
                    someNpc = Game.CreateNpc(redNpcID, 932, 769, 2);
                    someNpc.SetRelateDemagemRect(-25, -55, 45, 55);
                    isCreateNpcFirst = false;
                    Game.WaitTime(0);
                    return;
                }
                int x = Game.Random.Next(0, createNpcX.Length);
                someNpc = Game.CreateNpc(redNpcID, createNpcX[x].X, createNpcX[x].Y, 2);
                someNpc.SetRelateDemagemRect(-25, -55, 45, 55);
                createNpcCount++;
                currTurnID++;
                Game.WaitTime(0);
            }
        }

        public void KillNpc()
        {
            if (someNpc != null && someNpc.IsLiving)
            {
                someNpc.Die(0);
                someNpc = null;
            }
        }

        public void Reset()
        {
            currTurnID = 0;
            hitCount = 0;
            isCreateNpcFirst = true;
            isBeginHit = false;
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            int[] resources = { redNpcID };
            int[] gameOverResources = { redNpcID, redNpcID, redNpcID };
            Game.LoadResources(resources);
            Game.LoadNpcGameOverResources(gameOverResources);
            Game.SetMap(1138);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
            if (Game.CurrentLiving != null)
            {
                ((Player)Game.CurrentLiving).Seal((Player)Game.CurrentLiving, 0, 0);
            }
        }

        public override bool CanGameOver()
        {
            int currHitCount = 0;
            if (isBeginHit && someNpc != null && !someNpc.IsLiving)
                currHitCount = 1;
            if (hitCount + currHitCount >= needHitCount)
            {
                Game.IsWin = true;
                return true;
            }

            if (createNpcCount == maxTurnSize)
            {
                if (hitCount + currHitCount >= needHitCount)
                    Game.IsWin = true;
                else
                    Game.IsWin = false;
                return true;
            }
            return false;
        }

        public override int UpdateUIData()
        {
            return Game.TotalKillCount;
        }

        public override void OnPrepareGameOver()
        {

        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            List<LoadingFileInfo> loadingFileInfos = new List<LoadingFileInfo>();
        }
    }
}

