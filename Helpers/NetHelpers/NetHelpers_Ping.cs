﻿using System;
using Terraria;


namespace HamstarHelpers.NetHelpers {
	public partial class NetHelpers {
		public static int GetServerPing() {
			if( Main.netMode != 1 ) {
				throw new Exception("Only clients can gauge ping.");
			}
			
			return HamstarHelpersMod.Instance.NetHelpers.CurrentPing;
		}


		////////////////

		private int CurrentPing = -1;


		////////////////

		internal void UpdatePing( int ping ) {
			this.CurrentPing = ((this.CurrentPing * 2) + ping) / 3;
		}
	}
}
