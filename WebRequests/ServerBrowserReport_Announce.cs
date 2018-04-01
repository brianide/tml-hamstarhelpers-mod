﻿using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.DotNetHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.NPCHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;


namespace HamstarHelpers.WebRequests {
	partial class ServerBrowserReporter {
		public static void AnnounceServer() {
			int pvp = 0;
			bool[] team_checks = new bool[10];
			ServerBrowserReporter server_browser = HamstarHelpersMod.Instance.ServerBrowser;

			for( int i=0; i<Main.player.Length; i++ ) {
				Player player = Main.player[i];
				if( player == null || !player.active ) { continue; }

				if( player.hostile ) {
					pvp++;
				}
				team_checks[ player.team ] = true;
			}

			int team_count = 0;
			for( int i=1; i<team_checks.Length; i++ ) {
				if( team_checks[i] ) { team_count++; }
			}

			try {
				var server_data = new ServerBrowserEntry();
				server_data.ServerIP = NetHelpers.NetHelpers.GetPublicIP();
				server_data.Port = Netplay.ListenPort;
				server_data.Motd = Main.motd;
				server_data.WorldName = Main.worldName;
				server_data.WorldProgress = InfoHelpers.GetVanillaProgress();
				server_data.WorldEvent = NPCInvasionHelpers.GetCurrentInvasionType().ToString();
				server_data.MaxPlayerCount = Main.maxNetPlayers;
				server_data.PlayerCount = Main.ActivePlayersCount;
				server_data.PlayerPvpCount = pvp;
				server_data.TeamsCount = team_count;
				server_data.AveragePing = HamstarHelpersMod.Instance.ServerBrowser.AveragePing;
				server_data.Mods = new Dictionary<string, string>();

				foreach( Mod mod in ModLoader.LoadedMods ) {
					if( mod.File == null ) { continue; }
					server_data.Mods[ mod.DisplayName ] = mod.Version.ToString();
				}
			
				string json_str = JsonConvert.SerializeObject( server_data, Formatting.None );
				byte[] json_bytes = Encoding.UTF8.GetBytes( json_str );
			
				NetHelpers.NetHelpers.MakePostRequestAsync( ServerBrowserReporter.URL, json_bytes, delegate ( string output ) {
					ServerBrowserReporter.HandleServerAnnounceOutputAsync( server_data, output );
				}, delegate( Exception e, string output ) {
					LogHelpers.Log( "Server browser returned error: " + e.ToString() );
				} );

				ServerBrowserReporter.LastSendTimestamp = SystemHelpers.TimeStampInSeconds();
			} catch( Exception e ) {
				LogHelpers.Log( "AnnounceServer - " + e.ToString() );
				return;
			}
		}

		private static void HandleServerAnnounceOutputAsync( ServerBrowserEntry server_data, string output ) {
			try {
				var reply = JsonConvert.DeserializeObject<ServerBrowserWorkAssignment>( output );

				if( reply.ProofOfWorkNeeded ) {
					ServerBrowserReporter.DoWorkToValidateServer( server_data, reply.Hash );

					LogHelpers.Log( "Server data added to browser? " + reply.Success + " - Beginning validation..." );
				} else {
					LogHelpers.Log( "Server updated on browser? " + reply.Success );
				}
			} catch( Exception e ) {
				LogHelpers.Log( "HandleServerAnnounceOutput - " + e.ToString() + " - " + (output.Length>256?output.Substring(0,256):output) );
			}
		}


		////////////////

		public static void AnnounceServerConnect() {
			throw new NotImplementedException();
			/*try {
				var client_data = new ServerBrowserClientData();
				client_data.SteamID = SteamHelpers.GetSteamID();
				client_data.ClientIP = NetHelpers.NetHelpers.GetPublicIP();	// Netplay.GetLocalIPAddress();
				client_data.ServerIP = Netplay.ServerIP.ToString(); //Main.recentIP[0];
				client_data.WorldName = Main.worldName;
				client_data.Port = Netplay.ListenPort;
				client_data.Ping = NetHelpers.NetHelpers.GetServerPing();
				client_data.IsPassworded = !string.IsNullOrEmpty( Netplay.ServerPassword );
				client_data.HelpersVersion = HamstarHelpersMod.Instance.Version.ToString();
			
				string json_str = JsonConvert.SerializeObject( client_data, Formatting.None );
				byte[] json_bytes = Encoding.UTF8.GetBytes( json_str );
			
				NetHelpers.NetHelpers.MakePostRequestAsync( ServerBrowserReport.URL, json_bytes, delegate ( string output ) {
					LogHelpers.Log( "Server connection data added to browser. " + output );
				}, delegate ( Exception e, string output ) {
					LogHelpers.Log( "Server browser returned error for client: " + e.ToString() );
				} );

				ServerBrowserReport.LastSendTimestamp = SystemHelpers.TimeStampInSeconds();
			} catch( Exception e ) {
				LogHelpers.Log( "AnnounceServerConnect - " + e.ToString() );
				return;
			}*/
		}
	}
}