﻿using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic;
using System.Drawing;
using Bussiness;

namespace GameServerScript.AI.NPC
{
    public class ThirdHardKingThird : ABrain
    {
        private int m_attackTurn = 0;

        private int m_attackCount = 3;

        private int totemType = 0;

        private int npcID = 3203;

        private List<SimpleNpc> TotemBlood = new List<SimpleNpc>();

        private int blowTotemID = 3212;

        private int bloodTotemID = 3213;

        private int isSay = 0;

        #region NPC 说话内容
        private static string[] KillPlayerChat = new string[] { 
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg19"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg20"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg21"),
        };
        private static string[] ShootChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg4"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg5"),
        };
        private static string[] SeriesShootChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg2"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg3"),
        };
        private static string[] KillAttackChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg22"),
        };
        private static string[] DieChat = new string[] { 
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg18"),
        };
        private static string[] ShooteChat = new string[] { 
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg23"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg231"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg232"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg233"),
        };
        private static string[] AllAttackChat = new string[] { 
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg15"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg16"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg17"),
        };
        private static string[] CallChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg12"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg13"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg14"),
        };
        private static string[] BlowTotemChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg6"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg7"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg8"),
        };
        private static string[] BloodTotemChat = new string[]{
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg9"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg10"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdHardKingThird.msg11"),
        };

        #endregion

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            isSay = 0;
            m_body.CurrentDamagePlus = 1;
            m_body.CurrentShootMinus = 1;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            Body.Direction = Game.FindlivingbyDir(Body);

            if (IsKillAttack())
                return;

            if (m_attackTurn == 0)
            {
                AllAttack();
                m_attackTurn++;
                return;
            }
            if (m_attackTurn == 1)
            {
                ClearTotem();
                m_attackTurn++;
                return;
            }
            if (m_attackTurn == 2)
            {
                TotemAttack();
                m_attackTurn++;
                return;
            }
            if (m_attackTurn == 3)
            {
                int npcCount = ((SimpleBoss)Body).CurrentLivingNpcNum;
                if (npcCount < 8)
                    Summon();
                else
                    BeginAttack();
                m_attackTurn = 0;
                return;
            }
        }

        private bool IsKillAttack()
        {
            bool result = false;
            int maxdis = 0;
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > Body.X - 100 && player.X < Body.X + 100)
                {
                    int dis = (int)Body.Distance(player.X, player.Y);
                    if (dis > maxdis)
                    {
                        maxdis = dis;
                    }
                    result = true;
                }
            }

            if (result)
            {
                KillAttack(Body.X - 100, Body.X + 100);
                return true;
            }
            return false;
        }

        private void ClearTotem()
        {
            if (TotemBlood.Count != 0)
            {
                foreach (SimpleNpc totem in TotemBlood)
                {
                    totem.PlayMovie("die", 0, 1000);
                    totem.Die(1500);
                }
                Body.CallFuction(new LivingCallBack(BeginAttack), 2000);
                return;
            }
            Body.CallFuction(new LivingCallBack(BeginAttack), 0);
        }

        private void TotemAttack()
        {
            totemType = Game.Random.Next(0, 2);

            if (totemType == 0)
            {
                int index = Game.Random.Next(0, BlowTotemChat.Length);
                Body.Say(BlowTotemChat[index], 1, 1500);
            }
            else
            {
                int index = Game.Random.Next(0, BloodTotemChat.Length);
                Body.Say(BloodTotemChat[index], 1, 1500);
            }
            Body.PlayMovie("callA", 2500, 3000, new LivingCallBack(CreateTotem));
        }

        private void CreateTotem()
        {
            int delay = GetMaxDelay();
            if (totemType == 0)
            {
                SimpleNpc npc1 = null;
                SimpleNpc npc2 = null;
                SimpleNpc npc3 = null;
                int x = Game.FindBombPlayerX(100);
                npc1 = ((PVEGame)Game).CreateNpc(blowTotemID, x, 500, 1, 1, 1);
                if (x < 250)
                {
                    npc2 = ((PVEGame)Game).CreateNpc(blowTotemID, x + 200, 500, 1, 1, 1);
                    npc3 = ((PVEGame)Game).CreateNpc(blowTotemID, x + 400, 500, 1, 1, 1);
                }
                else if (x > 250 && x < 1250)
                {
                    npc2 = ((PVEGame)Game).CreateNpc(blowTotemID, x + 200, 500, 1, 1, 1);
                    npc3 = ((PVEGame)Game).CreateNpc(blowTotemID, x - 200, 500, 1, 1, 1);
                }
                else if (x > 1250)
                {
                    npc2 = ((PVEGame)Game).CreateNpc(blowTotemID, x - 200, 500, 1, 1, 1);
                    npc3 = ((PVEGame)Game).CreateNpc(blowTotemID, x - 400, 500, 1, 1, 1);
                }
                ((PVEGame)Game).ChangeMissionDelay(1, delay + 100);
            }
            else
            {
                int rand = Game.Random.Next(0, 2);
                int x = Game.Random.Next(Body.X - 200, Body.X - 100);
                TotemBlood.Add(((PVEGame)Game).CreateNpc(bloodTotemID, x, 500, 1, 1, 2));
                x = Game.Random.Next(Body.X + 100, Body.X + 200);
                TotemBlood.Add(((PVEGame)Game).CreateNpc(bloodTotemID, x, 500, 1, -1, 2));
                foreach (SimpleNpc totem in TotemBlood)
                {
                    totem.SetRelateDemagemRect(-15, -120, 30, 120);
                }
                ((PVEGame)Game).ChangeMissionDelay(2, delay + 100);
            }
        }

        public int GetMaxDelay()
        {
            List<Player> players = Game.GetAllFightPlayers();

            int maxDelay = 0;

            foreach (Player player in players)
            {
                if (player.Delay > maxDelay)
                {
                    maxDelay = player.Delay;
                }
            }
            return maxDelay;
        }

        private void BeginAttack()
        {
            //if (TotemBlood.Count != 0)
            //{
            //    foreach (SimpleNpc totem in TotemBlood)
            //    {
            //        totem.PlayMovie("die", 0, 1000);
            //        totem.Die(1500);
            //    }
            //}
            //TotemBlood.Clear();
            int index = Game.Random.Next(645, 845);
            int idx = Game.Random.Next(0, 2);
            if (idx == 0)
                Body.MoveTo(index, Body.Y, "walk", 2000, new LivingCallBack(PersonalAttack));
            else
                Body.MoveTo(index, Body.Y, "walk", 2000, new LivingCallBack(SeriesAttack));
        }

        private void AllAttack()
        {
            Body.CurrentDamagePlus = 0.8f;
            int index = Game.Random.Next(0, AllAttackChat.Length);
            Body.Say(AllAttackChat[index], 1, 1500);
            Body.PlayMovie("beatC", 3500, 0);
            Body.RangeAttacking(-1, Game.Map.Info.ForegroundWidth + 1, "cry", 5500, null);
        }

        private void KillAttack(int fx, int tx)
        {
            Body.CurrentDamagePlus = 20;
            int index = Game.Random.Next(0, KillAttackChat.Length);
            Body.Say(KillAttackChat[index], 1, 1500);
            Body.PlayMovie("beatC", 3500, 0);
            Body.RangeAttacking(fx, tx, "cry", 5500, null);
        }

        private void AttackPlayer(Player player, int ShootDelay, int MovieDelay, int value)
        {
            if (player.X > Body.X)
            {
                Body.ChangeDirection(1, MovieDelay - 100);
            }
            else
            {
                Body.ChangeDirection(-1, MovieDelay - 100);
            }

            int mtX = Game.Random.Next(player.X - 10, player.X + 10);

            float time = 1.0f;
            if (Math.Abs(Body.X - player.X) > 350)
            {
                time = 1.8f;
            }

            if (value == 0)
            {
                Body.CurrentDamagePlus = 1.3f;
                if (Body.ShootPoint(mtX, player.Y, 55, 1000, 10000, 1, time, ShootDelay))
                {
                    Body.PlayMovie("beatB", MovieDelay, 0);
                }
            }
            else
            {
                Body.CurrentDamagePlus = 0.7f;
                if (Body.ShootPoint(mtX, player.Y, 54, 1000, 10000, 1, time, ShootDelay))
                {
                    Body.PlayMovie("beatA", MovieDelay, 0);
                }
            }

        }

        private void PersonalAttack()
        {
            if (IsKillAttack())
                return;
            Player target = Game.FindRandomPlayer();
            if (target != null)
            {
                int index = Game.Random.Next(0, ShootChat.Length);
                Body.Say(ShootChat[index], 1, 1500);
                AttackPlayer(target, 4000, 3000,0);
            }
        }

        private void SeriesAttack()
        {
            if (IsKillAttack())
                return;
            List<Player> target = Game.GetAllFightPlayers();
            int attackCount = 0;
            int shootDelay = 4000;
            int movieDelay = 3000;
            foreach (Player player in target)
            {
                if (player.IsLiving)
                {
                    if (attackCount < m_attackCount)
                    {
                        attackCount++;
                        int index = Game.Random.Next(0, SeriesShootChat.Length);
                        Body.Say(SeriesShootChat[index], 1, 1500);
                        AttackPlayer(player, shootDelay, movieDelay, 1);
                        shootDelay += 1800;
                        movieDelay += 1800;
                    }
                }
            }
            if (attackCount < m_attackCount)
            {
                for (int i = attackCount; i < m_attackCount; i++)
                {
                    Player player = Game.FindRandomPlayer();
                    if (player != null)
                    {
                        AttackPlayer(player, shootDelay, movieDelay, 1);
                        shootDelay += 1800;
                        movieDelay += 1800;
                    }
                }
            }
        }
        private void Summon()
        {
            int index = Game.Random.Next(0, CallChat.Length);
            Body.Say(CallChat[index], 1, 1500);
            Body.PlayMovie("callB", 1500, 3000, new LivingCallBack(CreateChild));
        }

        private void CreateChild()
        {
            Point[] Point = { new Point(Body.X + 200, 500), new Point(Body.X - 200, 500), new Point(Body.X + 100, 500), new Point(Body.X - 100, 500) };
            ((SimpleBoss)Body).CreateChild(npcID, Point, 8, 4, 0);
        }


        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public override void OnKillPlayerSay()
        {
            base.OnKillPlayerSay();
            int index = Game.Random.Next(0, KillPlayerChat.Length);
            Body.Say(KillPlayerChat[index], 1, 1000);
        }

        public override void OnDiedSay()
        {

        }

        public override void OnDiedEvent()
        {
            if (TotemBlood.Count != 0)
            {
                foreach (SimpleNpc totem in TotemBlood)
                {
                    totem.PlayMovie("die", 0, 1000);
                    totem.Die(1500);
                }
            }
        }

        public override void OnShootedSay(int delay)
        {
            if (isSay == 0 && Body.IsLiving == true)
            {
                int index = Game.Random.Next(0, ShooteChat.Length);
                Body.Say(ShooteChat[index], 1, delay);
                isSay = 1;
            }

            if (!Body.IsLiving && isSay == 0)
            {
                int index = Game.Random.Next(0, DieChat.Length);
                Body.Say(DieChat[index], 1, delay - 800, 2000);
                Game.AddAction(new FocusAction(Body.X, Body.Y, 0, delay - 800, 1000));
            }
        }
    }
}
