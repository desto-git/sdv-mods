﻿namespace GiftDecline
{
	using System;
	using System.Collections.Generic;
	using Common;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;
	using StardewValley.Menus;

	/// <summary>Listeners for SMAPI events.</summary>
	internal static class EventHandler
	{
		/// <summary>Switching GUI windows.</summary>
		/// <param name="isInDialog">Reference to a boolean to flip.</param>
		/// <param name="e">Event data.</param>
		public static void OnMenuChanged(ref bool isInDialog, MenuChangedEventArgs e)
		{
			isInDialog = e.NewMenu is DialogueBox newDialog && newDialog.isPortraitBox();
		}

		/// <summary>Host notices a peer.</summary>
		/// <param name="e">Event data.</param>
		public static void OnModMessageReceived(ModMessageReceivedEventArgs e)
		{
			if (e.FromModID != MultiplayerHelper.ModId) return;
			if (e.Type != SaveGameHelper.Key) return;

			Logger.Trace("Receiving message from peer. Type = " + e.Type);
			SaveGameHelper.SaveState = e.ReadAs<ModData>();
			SaveGameHelper.Apply();
		}

		/// <summary>Host notices a peer.</summary>
		/// <param name="e">Event data.</param>
		public static void OnPeerConnected(PeerConnectedEventArgs e)
		{
			if (!e.Peer.HasSmapi) return;

			MultiplayerHelper.SendMessage(SaveGameHelper.SaveState, SaveGameHelper.Key);
		}

		/// <summary>Player changes location.</summary>
		/// <param name="onItemRemoved">Callback.</param>
		/// <param name="e">Event data.</param>
		public static void OnInventoryChanged(Action<Item> onItemRemoved, InventoryChangedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			IEnumerator<Item> removed = e.Removed.GetEnumerator();
			while (removed.MoveNext())
			{
				onItemRemoved(removed.Current);
			}

			IEnumerator<ItemStackSizeChange> quantityChanged = e.QuantityChanged.GetEnumerator();
			while (quantityChanged.MoveNext())
			{
				if (quantityChanged.Current.NewSize < quantityChanged.Current.OldSize)
				{
					onItemRemoved(quantityChanged.Current.Item);
				}
			}
		}

		/// <summary>Player changes location.</summary>
		public static void OnWarped()
		{
			NpcHelper.StoreAmountOfGiftsReceived(Game1.player.currentLocation.characters);
		}

		/// <summary>Day ends (before save).</summary>
		/// <param name="config">Mod configuration object.</param>
		public static void OnDayEnding(ModConfig config)
		{
			// apply gift taste changes at the end of day (and not immediately after gifting)
			// this way the social tab will show the reaction you actually got for that day
			SaveGameHelper.Apply();

			if (config.ResetEveryXDays == 0) return;

			int nextDay = Game1.Date.TotalDays + 1;
			if (nextDay % config.ResetEveryXDays == 0)
			{
				Logger.Trace("Resetting gift tastes");
				NpcHelper.ResetGiftTastes();
				SaveGameHelper.ResetGiftTastes();
			}
		}

		/// <summary>Just before a game is being saved.</summary>
		public static void OnSaving()
		{
			if (!Context.IsMainPlayer) return;
			SaveGameHelper.WriteToFile();
		}

		/// <summary>After save game got loaded (or new one is created).</summary>
		public static void OnSaveLoaded()
		{
			NpcHelper.StoreDefaultGiftTastes();

			if (!Context.IsMainPlayer) return;

			SaveGameHelper.LoadFromFileOrInitialize();
			SaveGameHelper.Apply();
		}

		/// <summary>NPCs got loaded.</summary>
		/// <param name="e">Event data.</param>
		public static void OnNpcListChanged(NpcListChangedEventArgs e)
		{
			if (!e.IsCurrentLocation) return;
			NpcHelper.StoreAmountOfGiftsReceived(e.Added);
		}
	}
}