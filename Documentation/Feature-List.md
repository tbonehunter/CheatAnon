# CheatAnon - Complete Feature List

All features are toggleable via GMCM (Generic Mod Config Menu). Each category has a master
toggle; subcategories and individual feature bools are listed below.

---

## 1. Advanced
**Config subcategories**: Complete Quests, Unlocked Content, Unlocked Areas, Wallet Items

| Feature | What it does | Config bool |
|---|---|---|
| BundlesCheat | Complete community center / JojaMart bundles (7 bundle types) | `EnableCompleteQuests` |
| QuestsCheat | Complete all open quests in the quest log | `EnableCompleteQuests` |
| UnlockContentCheat | Unlock dyes/tailoring, Junimo text, perfection status | `EnableUnlockedContent` |
| UnlockDoorCheat | Unlock Adventurer's Guild and all NPC rooms | `EnableUnlockedAreas` |
| WalletItemsCheat | Toggle 11 wallet items (Dwarvish translation, Rusty Key, Club Card, Special Charm, Skull Key, Town Key, Magnifying Glass, Dark Talisman, Magic Ink, Bear Paw, Spring Onion Mastery) | `EnableWalletItems` |

---

## 2. Farm & Fishing
**Config subcategories**: Farm, Fishing, Fast Machines

| Feature | What it does | Config bool |
|---|---|---|
| AutoFeedAnimalsCheat | Fills animal feed troughs in barns/coops automatically | `EnableFarm` |
| AutoPetAnimalsCheat | Automatically pets farm animals daily | `EnableFarm` |
| AutoPetPetsCheat | Automatically pets dogs/cats daily | `EnableFarm` |
| AutoWaterCropsCheat | Crops are watered automatically each day | `EnableFarm` |
| AutoWaterPetBowlsCheat | Pet water bowls are filled automatically | `EnableFarm` |
| DurableFencesCheat | Fences never decay or break | `EnableFarm` |
| InfiniteHayCheat | Hay silos are always full | `EnableFarm` |
| InstantBuildCheat | Building construction completes instantly | `EnableFarm` |
| AlwaysCastMaxDistanceCheat | Fishing rod always casts at maximum distance | `EnableFishing` |
| AlwaysFishTreasureCheat | Every fishing encounter includes treasure | `EnableFishing` |
| DurableFishTacklesCheat | Fishing tackles never break | `EnableFishing` |
| InstantFishBiteCheat | Fishing minigame appears immediately after casting | `EnableFishing` |
| InstantFishCatchCheat | Fish is caught instantly when minigame appears | `EnableFishing` |
| FastMachinesCheat | Machines (Crab Pot, Fruit Trees, all Data/Machines items) complete instantly | `EnableFastMachines` |

---

## 3. Player & Tools
**Config subcategories**: Player Stats, Tools, Tool Enchantments, Add Money, Add Casino Coins, Add Golden Walnuts, Add Qi Gems

| Feature | What it does | Config bool |
|---|---|---|
| InfiniteHealthCheat | Player health stays at maximum | `EnablePlayerStats` / `InfiniteHealth` |
| InfiniteStaminaCheat | Player stamina stays at maximum | `EnablePlayerStats` / `InfiniteStamina` |
| InstantCooldownCheat | Weapon special attack cooldowns are instant | `EnablePlayerStats` / `InstantCooldowns` |
| MaxDailyLuckCheat | Daily luck is always at maximum (0.115) | `EnablePlayerStats` / `MaxDailyLuck` |
| MoveSpeedCheat | Increases movement speed (0–10 levels via slider) | `EnablePlayerStats` / `MoveSpeed` |
| OneHitKillCheat | Player attacks kill any monster in one hit | `EnablePlayerStats` / `OneHitKill` |
| InventorySizeCheat | Sets inventory size via slider (1–12 upgrades) | `EnablePlayerStats` |
| HarvestWithScytheCheat | All crops can be harvested with the scythe | `EnableTools` / `HarvestScythe` |
| InfiniteWaterCheat | Watering can never runs dry | `EnableTools` / `InfiniteWateringCan` |
| OneHitBreakCheat | Tools break rocks/logs/etc. in one hit | `EnableTools` / `OneHitBreak` |
| ToolEnchantmentsCheat | Add or remove enchantments on the selected tool | `EnableToolEnchantments` |
| AddMoneyCheat | Buttons to add 100 / 1k / 10k / 100k / 1M gold | `EnableAddMoney` |
| AddCasinoCoinsCheat | Buttons to add 100 / 1,000 / 10,000 casino coins | `EnableAddCasinoCoins` |
| AddGoldenWalnutsCheat | Buttons to add 1 / 10 / 100 golden walnuts | `EnableAddGoldenWalnuts` |
| AddQiGemsCheat | Buttons to add 1 / 10 / 100 Qi Gems | `EnableAddQiGems` |

---

## 4. Relationships
**Config subcategories**: Give Gifts Anytime, Adjust Friendship Levels, No Friendship Decay

| Feature | What it does | Config bool |
|---|---|---|
| AlwaysGiveGiftsCheat | Bypass daily/weekly gift limits for all NPCs | `EnableGiveGiftsAnytime` / `AlwaysGiveGift` |
| HeartsCheat | Adjust friendship heart levels for all social NPCs | `EnableAdjustFriendshipLevels` |
| NoFriendshipDecayCheat | Prevents NPC friendships from slowly decaying | `EnableNoFriendshipDecay` / `NoFriendshipDecay` |

---

## 5. Skills
**Config subcategories**: Skill Levels, Professions

| Feature | What it does | Config bool |
|---|---|---|
| SkillsCheat | Increase Farming / Mining / Foraging / Fishing / Combat skill levels | `EnableSkillLevels` |
| ProfessionsCheat | Toggle individual professions across all skills (levels 5 & 10) | `EnableProfessions` |

---

## 6. Time
**Config subcategory**: Time

| Feature | What it does | Config bool |
|---|---|---|
| FreezeTimeCheat | Freeze time everywhere, inside only, or in caves only | `EnableTime` / `FreezeTime`, `FreezeTimeInside`, `FreezeTimeCaves` |
| SetTimeCheat | Set time of day (6:00 AM – 2:50 AM) via slider | `EnableTime` |
| SetDayCheat | Set day of month (1–28) via slider | `EnableTime` |
| SetSeasonCheat | Set season (Spring / Summer / Fall / Winter) via slider | `EnableTime` |
| SetYearCheat | Set year (1+) via slider | `EnableTime` |

---

## 7. Warps
**Config subcategories**: Main, Town, Forest, Mountain, Beach, Desert, Island  
**Config option**: Enforce Progression Restrictions

| Feature | What it does | Config bool |
|---|---|---|
| WarpCheat (all locations) | Warp button (Go) + hotkey binder (Set) per location | Section toggle |
| Mine / Skull Cavern | Level selector (−/+, Shift×10, Ctrl×50, Shift+Ctrl×500) + Go to selected level + Set hotkey to entrance | `EnableWarpSectionMountain` |

### Progression Restrictions (when enabled)
| Location | Requirement |
|---|---|
| Desert | Vault bundles complete or Joja membership |
| Ginger Island (all) | Boat repair complete |
| Secret Woods | Steel Axe or better |
| Sewer | Rusty Key in wallet |
| Mutant Bug Lair | Rusty Key + Dark Talisman quest complete |
| Various others | Location-specific game state checks |

### Warp Hotkeys
- Each warp location has a bindable hotkey stored in `config.json` under `WarpHotkeys`
- Hotkeys work outside the menu; pressing the key warps the player directly
- Mine/Skull Cavern hotkeys always warp to the entrance (level 0)

---

## 8. Weather
**Config subcategory**: Weather

| Feature | What it does | Config bool |
|---|---|---|
| SetWeatherForTomorrowCheat | Set tomorrow's weather: Sunny / Raining / Lightning / Snowing / Green Rain | `EnableWeather` |

---

## Custom UI Components

| Component | Purpose |
|---|---|
| `WarpOptionsButton` | Warp row with Go button (custom `button-go.png`) + Set hotkey button |
| `CheatsOptionsNumberWheel` | Mine/Skull Cavern row — level selector + Go + Set hotkey |
| Tooltip system | Per-button hover tooltips on Go and Set buttons throughout Warps tab |
- **Description**: What does this cheat do?
- **Config Key**: Proposed config property name
- **Default**: Enabled/Disabled by default?
- **Restrictions**: Any progression requirements?
- **UI Label**: Friendly name for GMCM
- **Tooltip**: Help text for users
- **Dependencies**: Other features this depends on
```

## Next Steps
1. Read through each cheat category source folder
2. Fill in feature lists using the template above
3. Identify all features that should have progression restrictions
4. Propose user-friendly naming for GMCM menu
