﻿using HamstarHelpers.Components.UI;
using HamstarHelpers.Components.UI.Elements;
using HamstarHelpers.TmlHelpers;
using HamstarHelpers.TmlHelpers.ModHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;


namespace HamstarHelpers.Internals.ControlPanel {
	partial class UIControlPanel : UIState {
		private static object ModDataListLock = new object();
		
		////////////////


		public static void UpdateModList( HamstarHelpersMod mymod ) {
			var ctrl_panel = mymod.ControlPanel;

			if( ctrl_panel == null || !ctrl_panel.ModListUpdateRequired || !ctrl_panel.IsOpen ) {
				return;
			}

			ctrl_panel.ModListUpdateRequired = false;
			
			lock( UIControlPanel.ModDataListLock ) {
				try {
					ctrl_panel.ModListElem.Clear();
					ctrl_panel.ModListElem.AddRange( ctrl_panel.ModDataList.ToArray() );
				} catch( Exception ) { }
			}
		}



		////////////////

		public bool IsOpen { get; private set; }

		private UITheme Theme = new UITheme();
		private ControlPanelLogic Logic = new ControlPanelLogic();
		private UserInterface Backend = null;

		private IList<UIModData> ModDataList = new List<UIModData>();
		private UIModData CurrentModListItem = null;

		private UIElement OuterContainer = null;
		private UIPanel InnerContainer = null;
		private UIList ModListElem = null;
		private UITextPanelButton DialogClose = null;

		private UITextArea IssueTitleInput = null;
		private UITextArea IssueBodyInput = null;

		private UITextPanelButton IssueSubmitButton = null;
		private UITextPanelButton ApplyConfigButton = null;
		private UITextPanelButton ModLockButton = null;

		private bool HasClicked = false;
		private bool ModListUpdateRequired = false;
		public bool AwaitingReport { get; private set; }

		private bool ResetIssueInput = false;
		private bool SetDialogToClose = false;
		private bool IsPopulatingList = false;


		////////////////

		public UIControlPanel() {
			this.IsOpen = false;
			this.AwaitingReport = false;
			this.InitializeToggler();
		}

		////////////////

		public override void OnInitialize() {
			this.InitializeComponents();
		}

		public override void OnActivate() {
			base.OnActivate();

			this.RefreshApplyConfigButton();

			int count;
			lock( UIControlPanel.ModDataListLock ) {
				count = this.ModDataList.Count;
			}

			if( count == 0 && !this.IsPopulatingList ) {
				this.LoadModListAsync();
			}
		}


		////////////////

		public void LoadModListAsync() {
			//Task.Run( () => {
			ThreadPool.QueueUserWorkItem( _ => {
				this.IsPopulatingList = true;

				lock( UIControlPanel.ModDataListLock ) {
					this.ModDataList.Clear();
				}

				int i = 1;

				foreach( var mod in ModHelpers.GetAllMods() ) {
					UIModData moditem = this.CreateModListItem( i++, mod );

					lock( UIControlPanel.ModDataListLock ) {
						this.ModDataList.Add( moditem );
					}

					//if( ModMetaDataManager.HasGithub( moditem.Mod ) ) {
					moditem.CheckForNewVersionAsync();
				}

				this.ModListUpdateRequired = true;
				this.IsPopulatingList = false;
			} );
		}


		////////////////

		public override void Update( GameTime gameTime ) {
			base.Update( gameTime );

			if( !this.IsOpen ) { return; }

			if( Main.playerInventory || Main.npcChatText != "" ) {
				this.Close();
				return;
			}

			if( this.OuterContainer.IsMouseHovering ) {
				Main.LocalPlayer.mouseInterface = true;
			}

			if( this.AwaitingReport || this.CurrentModListItem == null || !ModMetaDataManager.HasGithub( this.CurrentModListItem.Mod ) ) {
				this.DisableIssueInput();
			} else {
				this.EnableIssueInput();
			}

			if( this.ResetIssueInput ) {
				this.ResetIssueInput = false;
				this.IssueTitleInput.SetText( "" );
				this.IssueBodyInput.SetText( "" );
			}

			if( this.SetDialogToClose ) {
				this.SetDialogToClose = false;
				this.Close();
				return;
			}

			this.UpdateElements( HamstarHelpersMod.Instance );
		}


		////////////////

		public void RecalculateMe() {
			if( this.Backend != null ) {
				this.Backend.Recalculate();
			} else {
				this.Recalculate();
			}
		}

		public override void Recalculate() {
			base.Recalculate();

			if( this.OuterContainer != null ) {
				this.RecalculateContainer();
			}
		}


		////////////////

		public override void Draw( SpriteBatch sb ) {
			if( !this.IsOpen ) { return; }

			base.Draw( sb );
			this.DrawHoverElements( sb );
		}

		public void DrawHoverElements( SpriteBatch sb ) {
			if( !this.ModListElem.IsMouseHovering ) { return; }
			
			foreach( UIElement elem in this.ModListElem._items ) {
				if( elem.IsMouseHovering ) {
					( (UIModData)elem ).DrawHoverEffects( sb );
					break;
				}
			}
		}


		////////////////

		public bool CanOpen() {
			return !this.IsOpen && !Main.inFancyUI;
		}


		public void Open() {
			this.IsOpen = true;

			Main.playerInventory = false;
			Main.editChest = false;
			Main.npcChatText = "";

			Main.inFancyUI = true;
			Main.InGameUI.SetState( (UIState)this );

			this.Backend = Main.InGameUI;

			this.RecalculateMe();
		}


		public void Close() {
			this.IsOpen = false;

			Main.inFancyUI = false;
			Main.InGameUI.SetState( (UIState)null );

			this.Backend = null;
		}

		////////////////

		private void SelectModFromList( UIModData list_item ) {
			Mod mod = list_item.Mod;

			if( this.CurrentModListItem != null ) {
				this.Theme.ApplyListItem( this.CurrentModListItem );
			}
			this.Theme.ApplyListItemSelected( list_item );
			this.CurrentModListItem = list_item;

			this.Logic.SetCurrentMod( mod );

			if( !ModMetaDataManager.HasGithub( mod ) ) {
				this.DisableIssueInput();
			} else {
				this.EnableIssueInput();
			}
		}

		////////////////

		private void ApplyConfigChanges( HamstarHelpersMod mymod ) {
			this.Logic.ApplyConfigChanges( mymod );

			this.SetDialogToClose = true;
		}

		private void ToggleModLock( HamstarHelpersMod mymod ) {
			if( !ModLockHelpers.IsWorldLocked() ) {
				ModLockHelpers.LockWorld();
			} else {
				ModLockHelpers.UnlockWorld();
			}

			this.RefreshModLockButton( mymod );
		}
	}
}
