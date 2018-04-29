﻿using HamstarHelpers.Utilities.Network;
using Terraria;


namespace HamstarHelpers.NetProtocols {
	class HHPlayerPermaDeathProtocol : PacketProtocol {
		public int PlayerWho;
		public string Msg;


		////////////////

		internal HHPlayerPermaDeathProtocol( int player_who, string msg ) {
			this.PlayerWho = player_who;
			this.Msg = msg;

			if( Main.netMode == 1 ) {
				this.SendToServer( true );
			} else if( Main.netMode == 2 ) {
				this.SendToClient( -1, -1 );
			}
		}

		////////////////

		protected override void ReceiveWithClient() {
			Player player = Main.player[ this.PlayerWho ];

			PlayerHelpers.PlayerHelpers.ApplyPermaDeath( player, this.Msg );
		}
	}
}