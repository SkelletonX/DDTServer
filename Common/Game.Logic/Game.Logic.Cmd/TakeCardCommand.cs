using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(98, "翻牌")]
	public class TakeCardCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (!player.FinishTakeCard && player.CanTakeOut > 0)
			{
				int index = (int)packet.ReadByte();
				if (index < 0 || index > game.Cards.Length)
				{
					game.TakeCard(player, false);
				}
				else
				{
					game.TakeCard(player, index, false);
				}
			}
		}
	}
}
