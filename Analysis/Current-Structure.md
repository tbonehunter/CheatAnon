# Current CJBCheatsMenu Structure Analysis

## Key Findings

### 1. Configuration Already Exists! ✓
**Good News**: The `ModConfig.cs` already has individual boolean properties for most cheats. They're just not exposed via GMCM.

**Current Config Properties** (from ModConfig.cs):

#### Player & Tools
- `InfiniteHealth` - Infinite health
- `InfiniteStamina` - Infinite stamina/energy
- `InstantCooldowns` - No cooldowns on special attacks
- `MaxDailyLuck` - Maximum daily luck
- `OneHitKill` - Kill monsters in one hit
- `OneHitBreak` - Break items in one hit
- `MoveSpeed` - Player movement speed modifier

#### Farm & Fishing
- `InfiniteWateringCan` - Auto-refill watering can
- `HarvestScythe` - Harvest crops with scythe
- `InstantBite` - Instant fishing bite
- `InstantCatch` - Instant fishing catch
- `ThrowBobberMax` - Throw bobber max distance
- `DurableTackles` - Tackle doesn't break
- `AlwaysTreasure` - Always get treasure chests when fishing
- `DurableFences` - Fences don't decay
- `InstantBuild` - Buildings construct instantly
- `AutoFeed` - Auto-feed animals
- `AutoPetAnimals` - Auto-pet farm animals
- `AutoPetPets` - Auto-pet pets
- `AutoWater` - Auto-water crops
- `InfiniteHay` - Infinite hay in silos
- `FastFruitTree` - Fast fruit tree growth
- `FastBuildings` - HashSet of buildings to speed up
- `FastMachines` - HashSet of machines to speed up

#### Relationships
- `AlwaysGiveGift` - Always able to give gifts (no limit)
- `NoFriendshipDecay` - Friendship doesn't decay

#### Time
- `FreezeTime` - Freeze time (outdoor)
- `FreezeTimeInside` - Freeze time inside buildings
- `FreezeTimeCaves` - Freeze time in caves/mines
- `FadeTimeFrozenMessage` - Hide the "time frozen" message

#### Warps
- `HideWarpSections` - HashSet of warp sections to hide
- `HideWarps` - HashSet of individual warps to hide
- `AddWarpSections` - Custom warp sections to add
- `AddWarps` - Custom warps to add

#### Keybinds
- `OpenMenuKey` - Keybind to open cheats menu
- `FreezeTimeKey` - Keybind to toggle time freezing
- `GrowTreeKey` - Keybind to grow trees
- `GrowCropsKey` - Keybind to grow crops
- `ReloadConfigKey` - Keybind to reload config
- `GrowRadius` - Radius for grow tree/crops keys

#### Other
- `DefaultTab` - Which tab opens by default in the menu

---

## 2. GMCM Integration Already Exists (But Minimal)

**File**: `CJBCheatsMenu.Framework/GenericModConfigMenuIntegration.cs`

**Currently Exposed in GMCM**:
- ✅ Keybind configurations (OpenMenuKey, FreezeTimeKey, GrowTreeKey, etc.)
- ✅ Default tab selection
- ❌ **NOT exposed**: Individual cheat toggles
- ❌ **NOT exposed**: Category-level toggles
- ❌ **NOT exposed**: Progression restrictions

**The Integration Pattern**:
```csharp
// In ModEntry.cs OnGameLaunched():
GenericModConfigMenuIntegration configMenu = new GenericModConfigMenuIntegration(
    this.ModManifest, 
    this.Helper.ModRegistry, 
    Config, 
    () => this.Helper.WriteConfig<ModConfig>(Config)
);
configMenu.Register();
```

The integration class uses GMCM API calls like:
- `menu.AddSectionTitle()` - Section headers
- `menu.AddKeybindList()` - Keybind options
- `menu.AddTextOption()` - Dropdown selections
- `menu.AddParagraph()` - Informational text

---

## 3. Current User Experience

**How Users Configure Cheats Now**:
1. Press `P` (default) to open in-game CheatsMenu UI
2. Navigate tabs (Player & Tools, Farm & Fishing, etc.)
3. Toggle cheats on/off in the menu
4. Changes are saved to `config.json`

**GMCM Currently Only Used For**:
- Setting keybinds
- Choosing default tab
- Everything else points to the in-game menu

---

## Our Enhancement Plan

### What We Need to Do

#### 1. Extend ModConfig.cs
Add category-level master toggles and progression flags:

```csharp
// Category master toggles (NEW)
public bool EnablePlayerAndTools { get; set; } = true;
public bool EnableFarmAndFishing { get; set; } = true;
public bool EnableRelationships { get; set; } = true;
public bool EnableSkills { get; set; } = true;
public bool EnableTime { get; set; } = true;
public bool EnableWarps { get; set; } = true;
public bool EnableWeather { get; set; } = true;
public bool EnableAdvanced { get; set; } = true;

// Progression restrictions (NEW)
public bool RequireBoatForGingerIsland { get; set; } = false;
// ... other restrictions
```

#### 2. Expand GenericModConfigMenuIntegration.cs
Add all the existing config properties to GMCM, organized by category:

```csharp
// Add category sections
menu.AddSectionTitle(Manifest, "Player & Tools");
menu.AddBoolOption(Manifest, 
    () => Config.EnablePlayerAndTools,
    value => Config.EnablePlayerAndTools = value,
    "Enable Player & Tools Category",
    "Master toggle for all player & tools cheats"
);

menu.AddBoolOption(Manifest,
    () => Config.InfiniteHealth,
    value => Config.InfiniteHealth = value,
    "  Infinite Health",
    "Player health never decreases"
);
// ... etc for all properties
```

#### 3. Update Cheat Execution Code
Add checks for category toggles and individual toggles. This likely happens in:
- `CJBCheatsMenu.Framework/CheatManager.cs`
- Individual cheat classes in `CJBCheatsMenu.Framework.Cheats.*/`

```csharp
// Before executing a cheat, check if enabled
if (!Config.EnablePlayerAndTools || !Config.InfiniteHealth)
    return; // Don't apply the cheat

// Also check progression restrictions
if (Config.RequireBoatForGingerIsland && !IsBoatRepaired())
{
    Game1.showRedMessage("Boat must be repaired first!");
    return;
}
```

#### 4. Maintain Backward Compatibility
- Existing config.json files should still work
- New properties default to `true` (enabled) to match current behavior
- In-game CheatsMenu should still work alongside GMCM

---

## Next Steps

1. ✅ **Understand structure** (DONE - this document)
2. ⏭️ **Examine cheat categories** - Look at each `CJBCheatsMenu.Framework.Cheats.*` folder to understand:
   - How cheats are currently executed
   - Where to add toggle checks
   - What Skills/Advanced categories contain (we haven't documented those yet)
3. ⏭️ **Design GMCM menu structure** - Plan the full menu layout
4. ⏭️ **Implement changes** - Code the enhancements
5. ⏭️ **Test** - Verify everything works

---

## Files to Modify

### Will Need Changes:
- ✏️ `CJBCheatsMenu.Framework.Models/ModConfig.cs` - Add category toggles and restrictions
- ✏️ `CJBCheatsMenu.Framework/GenericModConfigMenuIntegration.cs` - Add all cheat options
- ✏️ `CJBCheatsMenu.Framework/CheatManager.cs` - Add toggle checks (probably)
- ✏️ Individual cheat classes - Add toggle checks where cheats execute

### Should NOT Change:
- ✅ `CJBCheatsMenu/ModEntry.cs` - Already calls GMCM integration correctly
- ✅ `config.json` - User's config file, will auto-update with new properties

---

## Questions to Investigate

- [ ] What's in the **Skills** category? (Not in current config properties)
- [ ] What's in the **Weather** category? (Not in current config properties)  
- [ ] What's in the **Advanced** category? (Not in current config properties)
- [ ] Where exactly are cheats executed? (CheatManager? Individual cheat classes?)
- [ ] How does the in-game CheatsMenu interact with ModConfig?
