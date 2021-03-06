using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	internal class AddRichesRobCondition : BaseUserRecord
	{
		public AddRichesRobCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.PlayerAddItem += new GamePlayer.PlayerAddItemEventHandel(this.player_PlayerAddItem);
		}
		private void player_PlayerAddItem(string type, int value)
		{
			if (type == "RichesRob")
			{
				this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, value);
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.PlayerAddItem -= new GamePlayer.PlayerAddItemEventHandel(this.player_PlayerAddItem);
		}
	}
}
