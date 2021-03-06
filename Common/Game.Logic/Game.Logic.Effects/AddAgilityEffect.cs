using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
	public class AddAgilityEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		private int m_added = 0;
		public AddAgilityEffect(int count, int probability) : base(eEffectType.AddAgilityEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddAgilityEffect effect = living.EffectList.GetOfType(eEffectType.AddAgilityEffect) as AddAgilityEffect;
			bool result;
			if (effect != null)
			{
				this.m_probability = ((this.m_probability > effect.m_probability) ? this.m_probability : effect.m_probability);
				result = true;
			}
			else
			{
				result = base.Start(living);
			}
			return result;
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeforePlayerShoot += new PlayerShootEventHandle(this.ChangeProperty);
			player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_AfterPlayerShooted(Player player, int delay)
		{
			player.FlyingPartical = 0;
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.ChangeProperty);
			player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void ChangeProperty(Player player, int ball)
		{
			player.Agility -= (double)this.m_added;
			this.m_added = 0;
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				player.FlyingPartical = 65;
				player.AttackEffectTrigger = true;
				this.IsTrigger = true;
				player.Agility += (double)this.m_count;
				this.m_added = this.m_count;
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddAgilityEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
