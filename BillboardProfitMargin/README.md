﻿# Billboard Profit Margin

Adjust billboard quest rewards to the profit margin, or by a custom amount.

Compatible with:
- [Profit Margins](https://www.nexusmods.com/stardewvalley/mods/4663)
- [Help Wanted Quest Fixes](https://www.nexusmods.com/stardewvalley/mods/2644)

## Installation

- Download the mod zip ->
	[GitHub](https://github.com/desto-git/sdv-mods/releases),
	[Nexus](https://www.nexusmods.com/stardewvalley/mods/6948)
- Download and install [SMAPI](https://smapi.io/)
- Unzip the mod into Stardew Valley's Mods folder

## Configuration

- `UseProfitMargin: bool` = (Pierre's) Use the profit margin (`true`), or use the custom amount (`false`)
- `CustomProfitMargin: float` = (Pierre's) Multiplier for quest rewards. `1.0` = no change, `0.5` = half, `2.0` = double
- `UseProfitMarginForSpecialOrders: bool` = (Lewis') Use the profit margin (`true`), or use the custom amount (`false`)
- `CustomProfitMarginForSpecialOrders: float` = (Lewis') Multiplier for quest rewards. `1.0` = no change, `0.5` = half, `2.0` = double

## Existing save

This mod can be used on an existing save.

For special orders (the board in front of Lewis' house),
please note however that only newly accepted quests will have their reward updated,
and that those quests rewards remain after you remove the mod again.
New quests after mod removal will have their original reward value again.

Reason being that special order rewards are already written to the save file by the game once the quest has been accepted.