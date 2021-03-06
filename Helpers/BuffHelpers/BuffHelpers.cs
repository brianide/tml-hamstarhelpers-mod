﻿using Terraria;


namespace HamstarHelpers.BuffHelpers {
	public partial class BuffHelpers {
		public static void AddPermaBuff( Player player, int buff_id ) {
			var myplayer = player.GetModPlayer<HamstarHelpersPlayer>();
			myplayer.Logic.AddPermaBuff( buff_id );
		}

		public static void RemovePermaBuff( Player player, int buff_id ) {
			var myplayer = player.GetModPlayer<HamstarHelpersPlayer>();
			myplayer.Logic.RemovePermaBuff( buff_id );
		}
	}
}
