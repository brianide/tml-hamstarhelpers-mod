﻿using HamstarHelpers.DebugHelpers;
using System;


namespace HamstarHelpers.TmlHelpers {
	public partial class TmlLoadHelpers {
		[System.Obsolete( "use TmlLoadHelpers.IsModLoaded", true )]
		public static bool IsLoaded() {
			return TmlLoadHelpers.IsModLoaded();
		}

		[System.Obsolete( "use TmlLoadHelpers.AddPostModLoadPromise", true )]
		public static void AddPostLoadPromise( Action action ) {
			TmlLoadHelpers.AddPostModLoadPromise( action );
		}

		[System.Obsolete( "use TmlLoadHelpers.AddPostModLoadPromise", true )]
		public static void AddPostGameLoadPromise( Action action ) {
			var mymod = HamstarHelpersMod.Instance;

			if( TmlLoadHelpers.PostGameLoadPromiseConditionsMet ) {
				action();
			} else {
				mymod.TmlLoadHelpers.PostGameLoadPromises.Add( action );
			}
		}
	}
}
