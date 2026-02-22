# GMCM Integration - Reimplementation Guide

## Overview

This guide provides step-by-step instructions to reimplement the GMCM integration on a clean CJBCheatsMenu codebase from the official GitHub repository.

**Original Work**: 5 phases completed with 5 git commits  
**Patches Available**: `Separate Mods from Cheats/Patches/` (0001-0005)  
**Documentation**: `Separate Mods from Cheats/` folder  

---

## Prerequisites

1. **Clone official CJBCheatsMenu source**:
   ```powershell
   cd "C:\Users\HP\Documents"
   git clone https://github.com/CJBok/SDV-Mods.git
   cd SDV-Mods
   ```

2. **Copy documentation folder**:
   ```powershell
   Copy-Item -Recurse "C:\Users\HP\Documents\CJBCheatsMenu\Separate Mods from Cheats" -Destination ".\CJBCheatsMenu\"
   ```

3. **Install development tools**:
   - Visual Studio 2022 or VS Code
   - .NET 6.0 SDK
   - Stardew Valley with SMAPI installed

---

## Reimplementation Checklist

### Phase 1: Extend ModConfig.cs (Patch 0001)
**File**: `CJBCheatsMenu.Framework.Models/ModConfig.cs`

- [ ] **Add 8 category toggle properties** (all default `true`):
  ```csharp
  public bool EnablePlayerAndTools { get; set; } = true;
  public bool EnableFarmAndFishing { get; set; } = true;
  public bool EnableRelationships { get; set; } = true;
  public bool EnableSkills { get; set; } = true;
  public bool EnableTime { get; set; } = true;
  public bool EnableWarps { get; set; } = true;
  public bool EnableWeather { get; set; } = true;
  public bool EnableAdvanced { get; set; } = true;
  ```

- [ ] **Add progression restriction toggle** (default `false`):
  ```csharp
  public bool EnforceWarpProgressionRestrictions { get; set; } = false;
  ```

- [ ] **Location**: Insert after keybind properties, before individual feature toggles
- [ ] **Add XML documentation comments** for each property
- [ ] **Verify**: Build compiles successfully

**Patch File**: `0001-Phase-1-Add-category-toggles-and-progression-restric.patch`

---

### Phase 2: Add Category Checks (Patch 0002)
**Files**: `CheatContext.cs` + 26 cheat files

#### Part 2A: Add Helper Methods to CheatContext
**File**: `CJBCheatsMenu.Framework/CheatContext.cs`

- [ ] **Add 8 category check helper methods**:
  ```csharp
  public bool IsPlayerAndToolsEnabled() => Config.EnablePlayerAndTools;
  public bool IsFarmAndFishingEnabled() => Config.EnableFarmAndFishing;
  public bool IsRelationshipsEnabled() => Config.EnableRelationships;
  public bool IsSkillsEnabled() => Config.EnableSkills;
  public bool IsTimeEnabled() => Config.EnableTime;
  public bool IsWarpsEnabled() => Config.EnableWarps;
  public bool IsWeatherEnabled() => Config.EnableWeather;
  public bool IsAdvancedEnabled() => Config.EnableAdvanced;
  ```

#### Part 2B: Update Individual Cheats
Update `OnConfig()` method in each cheat to check category before individual setting:

**Player & Tools Category** (8 files):
- [ ] `InfiniteHealthCheat.cs` - Add `context.IsPlayerAndToolsEnabled() &&` check
- [ ] `InfiniteStaminaCheat.cs` - Add `context.IsPlayerAndToolsEnabled() &&` check
- [ ] `InstantCooldownCheat.cs` - Add `context.IsPlayerAndToolsEnabled() &&` check
- [ ] `OneHitKillCheat.cs` - Add `context.IsPlayerAndToolsEnabled() &&` check
- [ ] `MaxDailyLuckCheat.cs` - Add `context.IsPlayerAndToolsEnabled() &&` check
- [ ] `MoveSpeedCheat.cs` - Add `context.IsPlayerAndToolsEnabled() &&` check
- [ ] `OneHitBreakCheat.cs` - Add `context.IsPlayerAndToolsEnabled() &&` check
- [ ] `InfiniteWaterCheat.cs` - Add `context.IsPlayerAndToolsEnabled() &&` check

**Farm & Fishing Category** (14 files):
- [ ] `AutoWaterCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `DurableFencesCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `InstantBuildCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `AutoFeedAnimalsCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `AutoPetAnimalsCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `AutoPetPetsCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `InfiniteHayCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `FastMachinesCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `InstantFishBiteCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `InstantFishCatchCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `AlwaysCastMaxDistanceCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `AlwaysFishTreasureCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `DurableFishTacklesCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check
- [ ] `GrowCheat.cs` - Add `context.IsFarmAndFishingEnabled() &&` check (hotkey check)

**Relationships Category** (2 files):
- [ ] `AlwaysGiveGiftsCheat.cs` - Add `context.IsRelationshipsEnabled() &&` check
- [ ] `NoFriendshipDecayCheat.cs` - Add `context.IsRelationshipsEnabled() &&` check

**Time Category** (1 file):
- [ ] `FreezeTimeCheat.cs` - Add `context.IsTimeEnabled() &&` check

**Pattern for all**:
```csharp
// BEFORE:
needsUpdate = context.Config.InfiniteHealth;

// AFTER:
needsUpdate = context.IsPlayerAndToolsEnabled() && context.Config.InfiniteHealth;
```

- [ ] **Verify**: All 26 files updated correctly
- [ ] **Verify**: Build compiles successfully

**Patch File**: `0002-Phase-2-Add-category-enable-checks-to-all-active-che.patch`

---

### Phase 3: GMCM Integration & Localization (Patch 0003)

#### Part 3A: Expand GenericModConfigMenuIntegration
**File**: `CJBCheatsMenu.Framework/GenericModConfigMenuIntegration.cs`

- [ ] **Update Register() method** - Add `titleScreenOnly: true` parameter:
  ```csharp
  menu.Register(Manifest, Reset, Save, titleScreenOnly: true);
  ```

- [ ] **Add Category Toggles section** (after Register, before Controls):
  ```csharp
  menu.AddSectionTitle(Manifest, I18n.Config_Title_Categories);
  
  menu.AddBoolOption(Manifest, () => Config.EnablePlayerAndTools, 
      value => Config.EnablePlayerAndTools = value,
      I18n.Config_EnablePlayerAndTools_Name, 
      I18n.Config_EnablePlayerAndTools_Desc);
  
  // ... repeat for all 8 categories
  ```

- [ ] **Add Progression Restrictions section** (after categories, before Controls):
  ```csharp
  menu.AddSectionTitle(Manifest, I18n.Config_Title_ProgressionRestrictions);
  
  menu.AddBoolOption(Manifest, () => Config.EnforceWarpProgressionRestrictions,
      value => Config.EnforceWarpProgressionRestrictions = value,
      I18n.Config_EnforceWarpProgressionRestrictions_Name,
      I18n.Config_EnforceWarpProgressionRestrictions_Desc);
  ```

- [ ] **Update Reset() method** - Add defaults for 9 new properties:
  ```csharp
  Config.EnablePlayerAndTools = true;
  Config.EnableFarmAndFishing = true;
  // ... all 8 categories to true
  Config.EnforceWarpProgressionRestrictions = false;
  ```

#### Part 3B: Add Translation Keys
**File**: `i18n/default.json`

- [ ] **Add section titles**:
  ```json
  "config.title.categories": "Category Master Toggles",
  "config.title.progression-restrictions": "Progression Restrictions",
  ```

- [ ] **Add 8 category name/description pairs**:
  ```json
  "config.enable-player-and-tools.name": "Enable Player & Tools",
  "config.enable-player-and-tools.desc": "Enables all cheats in the Player & Tools category...",
  ```

- [ ] **Add progression restriction pair**:
  ```json
  "config.enforce-warp-progression-restrictions.name": "Enforce Warp Progression Restrictions",
  "config.enforce-warp-progression-restrictions.desc": "When enabled, warps require vanilla unlock conditions...",
  ```

- [ ] **Total**: 21 new translation keys

#### Part 3C: Add Translation Helper Methods
**File**: `CJBCheatsMenu/I18n.cs`

- [ ] **Add 21 constant keys** (after `Controls_ReloadConfig_Desc`):
  ```csharp
  private const string Config_Title_Categories = "config.title.categories";
  private const string Config_EnablePlayerAndTools_Name = "config.enable-player-and-tools.name";
  // ... etc for all 21 keys
  ```

- [ ] **Add 21 translation getter methods** (after `Controls_ReloadConfig_Desc()` method):
  ```csharp
  public static string Config_Title_Categories()
  {
      return Translation.op_Implicit(GetByKey("config.title.categories"));
  }
  // ... repeat pattern for all 21 methods
  ```

- [ ] **Verify**: All methods follow existing pattern
- [ ] **Verify**: Build compiles successfully

**Patch File**: `0003-Phase-3-Add-GMCM-integration-with-category-toggles-a.patch`

---

### Phase 4: Dynamic Tab Filtering (Patch 0004)
**File**: `CJBCheatsMenu.Framework/CheatsMenu.cs`

- [ ] **Locate ResetComponents() method** (around line 333)

- [ ] **Replace fixed tab array** with dynamic list building:
  ```csharp
  // BEFORE: Fixed array of 9 tabs
  Tabs.AddRange((IEnumerable<ClickableComponent>)(object)new ClickableComponent[9] { ... });
  
  // AFTER: Dynamic list based on config
  ModConfig config = Cheats.Context.Config;
  List<ClickableComponent> tabList = new List<ClickableComponent>();
  
  if (config.EnablePlayerAndTools)
      tabList.Add(new ClickableComponent(..., MenuTab.PlayerAndTools, ...));
  if (config.EnableFarmAndFishing)
      tabList.Add(new ClickableComponent(..., MenuTab.FarmAndFishing, ...));
  // ... repeat for all 8 categories
  
  // Always add Controls tab (not a cheat category)
  tabList.Add(new ClickableComponent(..., MenuTab.Controls, ...));
  
  Tabs.AddRange(tabList);
  ```

- [ ] **Verify**: Counter variable `i` increments only when tab is added
- [ ] **Verify**: Controls tab is always included
- [ ] **Verify**: Build compiles successfully
- [ ] **Test**: Disabled categories don't appear in menu

**Patch File**: `0004-Phase-4-Add-dynamic-tab-filtering-based-on-enabled-c.patch`

---

### Phase 5: Warp Progression Restrictions (Patch 0005)

#### Part 5A: Create WarpRestrictions Helper Class
**New File**: `CJBCheatsMenu.Framework.Cheats.Warps/WarpRestrictions.cs`

- [ ] **Create new file** in Warps folder
- [ ] **Add using statements**:
  ```csharp
  using StardewValley;
  using StardewValley.Tools;
  ```

- [ ] **Implement 10 static check methods**:
  1. `IsDesertUnlocked()` - Vault bundles OR Joja
  2. `IsQuarryUnlocked()` - Crafts Room OR Joja
  3. `IsRailroadUnlocked()` - Date check (Summer 3 Y1+)
  4. `IsMutantBugLairUnlocked()` - Rusty Key AND Magic Ink
  5. `IsSecretWoodsUnlocked()` - Steel Axe (level 2+)
  6. `IsWizardTowerUnlocked()` - CC discovery
  7. `IsSewerUnlocked()` - Rusty Key
  8. `IsGingerIslandUnlocked()` - Boat repair
  9. `IsMasteryCaveUnlocked()` - Mastery exp > 0

- [ ] **Add XML documentation** for each method
- [ ] **Reference**: See `Documentation/Progression-Restrictions.md` for logic details

#### Part 5B: Integrate Restrictions into WarpCheat
**File**: `CJBCheatsMenu.Framework.Cheats.Warps/WarpCheat.cs`

- [ ] **Add new method** `IsWarpRestricted(WarpContentModel warp, CheatContext context)`:
  - Return `false` if `!context.Config.EnforceWarpProgressionRestrictions`
  - Check `warp.Location` against restriction mappings
  - Call appropriate `WarpRestrictions.Is*Unlocked()` method

- [ ] **Update GetFields() method**:
  - Add `bool isRestricted = IsWarpRestricted(warp, context);` check
  - Pass `disabled: isRestricted` to all warp button constructors
  - Update Farm warp, Mine/Skull Cave buttons, and regular warps

- [ ] **Update CreateMinesButton() method signature**:
  - Add `bool isRestricted` parameter
  - Pass `disabled: isRestricted` to CheatsOptionsNumberWheel constructor

#### Part 5C: Update CheatsOptionsNumberWheel
**File**: `CJBCheatsMenu.Framework.Components/CheatsOptionsNumberWheel.cs`

- [ ] **Update constructor signature**:
  ```csharp
  // BEFORE:
  public CheatsOptionsNumberWheel(..., bool disabled = false)
      : base(..., disabled: false)
  
  // AFTER:
  public CheatsOptionsNumberWheel(..., bool disabled = false)
      : base(..., disabled: disabled)
  ```

- [ ] **Verify**: Mines/Skull Cave buttons can be disabled
- [ ] **Verify**: Build compiles successfully

**Patch File**: `0005-Phase-5-Implement-warp-progression-restrictions.patch`

**Restriction Mappings Reference**:
- `island*` → IsGingerIslandUnlocked()
- `desert`, `skullcave` → IsDesertUnlocked()
- Quarry warps → IsQuarryUnlocked()
- `railroad`, `bathhouse` → IsRailroadUnlocked()
- `sewer` → IsSewerUnlocked()
- `bugland` → IsMutantBugLairUnlocked()
- `woods` → IsSecretWoodsUnlocked()
- `wizardhouse` → IsWizardTowerUnlocked()
- Mastery warps → IsMasteryCaveUnlocked()

---

## Alternative: Apply Patches Automatically

If the official codebase structure is similar enough, you can try applying patches directly:

```powershell
cd "C:\Users\HP\Documents\SDV-Mods\CJBCheatsMenu"

# Copy patches
Copy-Item "C:\Users\HP\Documents\CJBCheatsMenu\Separate Mods from Cheats\Patches\*" -Destination ".\"

# Apply patches in order
git am 0001-Phase-1-Add-category-toggles-and-progression-restric.patch
git am 0002-Phase-2-Add-category-enable-checks-to-all-active-che.patch
git am 0003-Phase-3-Add-GMCM-integration-with-category-toggles-a.patch
git am 0004-Phase-4-Add-dynamic-tab-filtering-based-on-enabled-c.patch
git am 0005-Phase-5-Implement-warp-progression-restrictions.patch
```

**Note**: Patches may fail if file structure differs. Use manual reimplementation if needed.

---

## Verification & Testing

### Build Testing
- [ ] Project compiles without errors
- [ ] No missing references
- [ ] All new files included in project

### Configuration Testing
- [ ] GMCM menu appears correctly
- [ ] All 8 category toggles present
- [ ] Progression restriction toggle works
- [ ] titleScreenOnly: true enforced (game closes on save)
- [ ] Defaults correct (categories enabled, restrictions disabled)

### Category Toggle Testing
- [ ] Disable each category → tab hidden, cheats stop
- [ ] Enable/disable cycle works
- [ ] Controls tab always visible

### Progression Restriction Testing
- [ ] Restricted warps appear grayed out
- [ ] Clicking disabled warp does nothing
- [ ] Unlocking progression enables warp
- [ ] Joja route bypasses bundle restrictions

### Edge Cases
- [ ] All categories disabled (except Controls)
- [ ] Multiplayer: host config applies to farmhands
- [ ] Custom warps don't cause errors
- [ ] Without GMCM installed: no errors

---

## Documentation Reference

All documentation is in `Separate Mods from Cheats/` folder:

- **Project_Development.md** - Complete development diary (10 entries)
- **GMCM-Menu-Structure.md** - Planned menu structure and design decisions
- **Documentation/Progression-Restrictions.md** - Detailed restriction logic
- **Patches/** - Git patches for all 5 phases
- **Analysis/** - Architecture and feature analysis
- **This file** - Reimplementation guide

---

## Design Principles

**Two-Layer Control**:
- Title screen (GMCM): Category discipline
- In-game menu: Feature flexibility

**Anti-Temptation**:
- Title screen only changes
- Hidden tabs (not grayed out)
- Must close game to change categories

**Progression Philosophy**:
- "Warps for convenience, not bypassing progression"
- Optional restrictions (default off)
- Joja route bypasses bundle restrictions

**Backward Compatibility**:
- All defaults maintain existing behavior
- Existing configs load seamlessly
- Graceful degradation without GMCM

---

## Support

For questions or issues during reimplementation, refer to:
- **Project_Development.md** - Detailed explanations of all decisions
- **Git patches** - Exact code changes for each phase
- **Progression-Restrictions.md** - Logic details for warp restrictions

---

**Total Estimated Time**: 4-6 hours (manual reimplementation)  
**Files Modified**: ~35 files  
**Lines Added**: ~800 lines  
**Commits**: 5 phases  

Good luck with the reimplementation! 🎮
