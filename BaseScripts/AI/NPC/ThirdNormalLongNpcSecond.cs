﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;
using Bussiness;

namespace GameServerScript.AI.NPC
{
    public class ThirdNormalLongNpcSecond : ABrain
    {
        protected Player m_targer;
        protected SimpleNpc m_bloom;
        protected Living attack;
        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            m_body.CurrentDamagePlus = 1;
            m_body.CurrentShootMinus = 1;
            if (m_body.CanSay)
            {
                string msg;
                int index = Game.Random.Next(0, AntChat.Length);
                msg = AntChat[index];
                m_body.Say(msg, 0, 2000);
            }
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }


        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            m_targer = Game.FindRandomPlayer();
            SimpleNpc npc = (SimpleNpc)Body;
            m_bloom = Game.FindNearestAdverseNpc(Body.X, Body.Y, npc.NpcInfo.Camp);

            if (m_bloom != null)
            {
                attack = m_bloom;
            }
            else
            {
                attack = m_targer;
            }
            if (Math.Abs(attack.X - Body.X) < 250)
            {
                MoveToPlayer();
                return;
            }
            int dis = Game.Random.Next(Body.X - 100, Body.X + 100);
            Body.MoveTo(dis, Body.Y, "walk", 1000, new LivingCallBack(NextAttack));
        }

        public void MoveToPlayer()
        {
            m_body.CurrentDamagePlus = 1.5f;
            int dis = (int)attack.Distance(Body.X, Body.Y);
            int ramdis = Game.Random.Next(((SimpleNpc)Body).NpcInfo.MoveMin, ((SimpleNpc)Body).NpcInfo.MoveMax);
            if (dis > ((SimpleNpc)Body).NpcInfo.MoveMax)
            {
                dis = ramdis;
            }
            else
            {
                dis = dis - 60;
            }

            if (dis == 0)
            {
                MoveBeat();
                return;
            }

            if (Body.X > m_targer.X)
            {
                Body.MoveTo(Body.X - dis, attack.Y, "walk", 500, new LivingCallBack(MoveBeat));
            }
            else
            {
                Body.MoveTo(Body.X + dis, attack.Y, "walk", 500, new LivingCallBack(MoveBeat));
            }
        }

        public void MoveBeat()
        {
            Body.Beat(attack, "beatB", 0);
        }

        private void NextAttack()
        {
            if (attack != null)
            {
                if (attack.X > Body.X)
                {
                    Body.ChangeDirection(1, 800);
                }
                else
                {
                    Body.ChangeDirection(-1, 800);
                }

                int mtX = Game.Random.Next(attack.X - 10, attack.X + 10);

                if (Body.ShootPoint(mtX, attack.Y, 58, 1000, 10000, 1, 2.0f, 2300))
                {
                    //Body.PlayMovie("beatA", 1500, 0);
                }
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        #region NPC 小怪说话

        private static Random random = new Random();

        private static string[] AntChat = new string[] {
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalLongNpcSecond.msg1"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalLongNpcSecond.msg2"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalLongNpcSecond.msg3"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalLongNpcSecond.msg4"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalLongNpcSecond.msg5"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalLongNpcSecond.msg6"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalLongNpcSecond.msg7"),
            LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalLongNpcSecond.msg8"),
        };
        #endregion
    }
}
