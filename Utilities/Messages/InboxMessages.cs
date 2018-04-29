﻿using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.TmlHelpers;
using System;
using System.Collections.Generic;
using Terraria;


namespace HamstarHelpers.Utilities.Messages {
	public class InboxMessages {
		public static void SetMessage( string which, string msg, bool force_unread, Action<bool> on_run=null ) {
			InboxMessages inbox = HamstarHelpersMod.Instance.Inbox.Messages;

			if( inbox.Messages.ContainsKey( which ) ) {
				if( force_unread ) {
					inbox.Order.Remove( which );
				} else {
					return;
				}
			}

			inbox.Messages[which] = msg;
			inbox.MessageActions[which] = on_run;
			inbox.Order.Add( which );
		}


		public static int CountUnreadMessages() {
			InboxMessages inbox = HamstarHelpersMod.Instance.Inbox.Messages;

			return inbox.Messages.Count - inbox.Current;
		}


		public static string DequeueMessage() {
			InboxMessages inbox = HamstarHelpersMod.Instance.Inbox.Messages;
			
			if( inbox.Current >= inbox.Order.Count ) {
				return null;
			}

			string which = inbox.Order[ inbox.Current++ ];
			string msg = inbox.Messages[ which ];

			return msg;
		}


		public static string GetMessageAt( int i, out bool is_unread ) {
			InboxMessages inbox = HamstarHelpersMod.Instance.Inbox.Messages;
			is_unread = false;

			if( i < 0 || i >= inbox.Order.Count ) {
				return null;
			}

			string which = inbox.Order[ i ];
			string msg = inbox.Messages[ which ];

			is_unread = i >= inbox.Current;

			return msg;
		}



		////////////////

		private IDictionary<string, string> Messages = new Dictionary<string, string>();
		private IDictionary<string, Action<bool>> MessageActions = new Dictionary<string, Action<bool>>();
		private IList<string> Order = new List<string>();
		public int Current { get; private set; }

		private bool IsLoaded = false;


		////////////////

		internal InboxMessages() {
			this.Current = 0;

			TmlLoadHelpers.AddWorldLoadPromise( () => {
				if( this.IsLoaded ) { return; }
				InboxMessages inbox_copy = this.LoadFromFile( out this.IsLoaded );

				if( this.IsLoaded ) {
					this.Messages = inbox_copy.Messages;
					this.MessageActions = inbox_copy.MessageActions;
					this.Order = inbox_copy.Order;
					this.Current = inbox_copy.Current;
				}
			} );
		}
		
		~InboxMessages() {
			if( this.IsLoaded ) {
				this.SaveToFile();
			}
		}


		////////////////

		internal InboxMessages LoadFromFile( out bool success ) {
			string pid = PlayerIdentityHelpers.GetUniqueId( Main.LocalPlayer, out success );
			if( !success ) {
				return null;
			}

			return DataFileHelpers.LoadBinary<InboxMessages>( HamstarHelpersMod.Instance, "Inbox_" + pid, out success );
		}

		internal void SaveToFile() {
			bool success;
			string pid = PlayerIdentityHelpers.GetUniqueId( Main.LocalPlayer, out success );
			if( !success ) {
				return;
			}

			DataFileHelpers.SaveAsBinary<InboxMessages>( HamstarHelpersMod.Instance, "Inbox_" + pid, this );
		}
	}
}