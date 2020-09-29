﻿namespace GiftDecline
{
	using System;
	using System.Collections.Generic;
	using Common;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;

	/// <summary>Main class.</summary>
	internal class ModEntry : Mod
	{
		private ModConfig config;
		private bool isInDialog = false;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Logger.Init(this.Monitor);
			MultiplayerHelper.Init(this.Helper.Multiplayer);
			SaveGameHelper.Init(this.Helper.Data);

			this.config = this.Helper.ReadConfig<ModConfig>();
			if (this.config.ResetEveryXDays < 0)
			{
				Logger.Error("Error in config.json: \"ResetEveryXDays\" must be at least 0.");
				Logger.Error("Deactivating mod");
				return;
			}

			SaveGameHelper.AddResetCommand(helper);

			helper.Events.Display.MenuChanged += (object sender, MenuChangedEventArgs e) =>
				EventHandler.OnMenuChanged(ref this.isInDialog, e);

			helper.Events.Multiplayer.ModMessageReceived += (object sender, ModMessageReceivedEventArgs e) =>
				EventHandler.OnModMessageReceived(e);

			helper.Events.Multiplayer.PeerConnected += (object sender, PeerConnectedEventArgs e) =>
				EventHandler.OnPeerConnected(e);

			helper.Events.Player.InventoryChanged += (object sender, InventoryChangedEventArgs e) =>
				EventHandler.OnInventoryChanged(this.OnItemRemoved, e);

			helper.Events.Player.Warped += (object sender, WarpedEventArgs e) =>
				EventHandler.OnWarped();

			helper.Events.GameLoop.DayEnding += (object sender, DayEndingEventArgs e) =>
				EventHandler.OnDayEnding(this.config);

			helper.Events.GameLoop.Saving += (object sender, SavingEventArgs e) =>
				EventHandler.OnSaving();

			helper.Events.GameLoop.SaveLoaded += (object sender, SaveLoadedEventArgs e) =>
				EventHandler.OnSaveLoaded();

			helper.Events.World.NpcListChanged += (object sender, NpcListChangedEventArgs e) =>
				EventHandler.OnNpcListChanged(e);
		}

		private void OnItemRemoved(Item plainItem)
		{
			if (!this.isInDialog) return;

			if (!(plainItem is StardewValley.Object item)) return;
			if (!item.canBeGivenAsGift()) return; // e.g. Tools or any placable object

			NPC recipient = null;
			IEnumerator<NPC> enumerator = Game1.player.currentLocation.characters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NPC npc = enumerator.Current;
				if (NpcHelper.AcceptsGifts(npc) && NpcHelper.HasReceivedGift(npc, item))
				{
					recipient = npc;
					break;
				}
			}

			if (recipient == null)
			{
				throw new Exception("It appears a gift has been given to someone, but I can't determine to whom :(");
			}

			int newGiftTaste = NpcHelper.GetReduceGiftTaste(recipient, item);
			string itemId = plainItem.ParentSheetIndex.ToString();
			SaveGameHelper.SetGiftTaste(recipient.Name, itemId, newGiftTaste);
		}
	}
}