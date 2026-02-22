# Complete Architecture Analysis

## Understanding the Cheat System

### How Cheats Work

#### 1. ICheat Interface
Every cheat implements `ICheat` which has these key methods:

- **`GetFields()`** - Returns UI elements (checkboxes, buttons, etc.) for the in-game menu
- **`OnConfig()`** - Called when config changes; returns flags indicating what the cheat needs:
  - `needsInput` - If true, cheat will receive button press events
  - `needsUpdate` - If true, cheat will be called every game tick
  - `needsRendering` - If true, cheat will be called on render
- **`OnUpdated()`** - Called every tick if `needsUpdate` is true - **THIS IS WHERE CHEATS EXECUTE**
- **`OnRendered()`** - Called on render if `needsRendering` is true
- **`OnButtonsChanged()`** - Called on input if `needsInput` is true

#### 2. Example: InfiniteHealthCheat

```csharp
public class InfiniteHealthCheat : BaseCheat
{
    // Returns checkbox for in-game menu
    public override IEnumerable<OptionsElement> GetFields(CheatContext context)
    {
        yield return new CheatsOptionsCheckbox(
            "Infinite Health",
            context.Config.InfiniteHealth,  // Current value from config
            value => context.Config.InfiniteHealth = value  // Setter
        );
    }
    
    // Determines if cheat should be active
    public override void OnConfig(CheatContext context, out bool needsInput, 
                                  out bool needsUpdate, out bool needsRendering)
    {
        needsInput = false;
        needsUpdate = context.Config.InfiniteHealth;  // ← Only active if config enabled!
        needsRendering = false;
    }
    
    // Executes every tick when enabled
    public override void OnUpdated(CheatContext context, UpdateTickedEventArgs e)
    {
        if (Context.IsWorldReady)
        {
            Game1.player.health = Game1.player.maxHealth;  // ← THE CHEAT
        }
    }
}
```

**Key Insight**: The config property (`InfiniteHealth`) controls:
1. Whether the checkbox appears checked in the menu
2. Whether the cheat is registered for updates (in `OnConfig`)
3. The actual execution happens in `OnUpdated` regardless of config check (but it won't be called if not registered)

---

### CheatManager - The Hub

`CheatManager.cs` is the central coordinator:

1. **Instantiates all cheats** as properties
2. **Collects them into arrays** based on what they need:
   - `CheatsWhichNeedUpdate` - Called every tick
   - `CheatsWhichNeedInput` - Called on button press
   - `CheatsWhichNeedRendering` - Called on render
3. **Calls `OnConfig()`** when settings change to rebuild these arrays
4. **Routes events** to the appropriate cheat lists

```csharp
// When config changes
public void OnOptionsChanged()
{
    CheatsWhichNeedUpdate.Clear();
    // ...
    
    foreach (ICheat cheat in Cheats)
    {
        cheat.OnConfig(Context, out var needsInput, out var needsUpdate, out var needsRendering);
        
        if (needsUpdate)
            CheatsWhichNeedUpdate.Add(cheat);  // ← Only active cheats added!
        // ...
    }
}

// Every game tick
public void OnUpdateTicked(UpdateTickedEventArgs e)
{
    foreach (ICheat cheat in CheatsWhichNeedUpdate)  // ← Only calls active cheats
    {
        cheat.OnUpdated(Context, e);
    }
}
```

---

### CheatsMenu - The In-Game UI

`CheatsMenu.cs` displays the cheat configuration interface:

```csharp
private void SetOptions()
{
    switch (CurrentTab)
    {
        case MenuTab.PlayerAndTools:
            AddOptions("Player:", 
                cheats.InfiniteHealth,   // ← Gets UI from GetFields()
                cheats.InfiniteStamina,
                cheats.OneHitKill
                // ...
            );
            break;
        
        case MenuTab.FarmAndFishing:
            AddOptions("Farm:", 
                cheats.AutoWater,
                cheats.DurableFences
                // ...
            );
            break;
        // ... other tabs
    }
}
```

Each cheat's `GetFields()` method returns the UI elements that appear in its section.

---

## The Three Missing Categories

### 1. Skills
**Files**: `SkillsCheat.cs`, `ProfessionsCheat.cs`

**Functionality**:
- **Skills**: Buttons to level up Farming, Mining, Foraging, Fishing, Combat
- **Professions**: Checkboxes to add/remove profession perks

**UI Pattern**: Action buttons (not toggles) - click to increase skill

**No Config Properties**: These are one-time actions, not persistent toggles

---

### 2. Weather
**File**: `SetWeatherForTomorrowCheat.cs`

**Functionality**:
- Set tomorrow's weather: Sunny, Rain, Storm, Snow, Green Rain

**UI Pattern**: Action buttons - click to change weather

**No Config Properties**: One-time action

---

### 3. Advanced
**Files**: 
- `QuestsCheat.cs` - Complete active quests
- `WalletItemsCheat.cs` - Toggle special items (Dwarf translation, rusty key, skull key, etc.)
- `UnlockDoorCheat.cs` - Unlock specific doors
- `UnlockContentCheat.cs` - Unlock community center, greenhouse, wizard, etc.
- `BundlesCheat.cs` - Complete community center bundles

**Functionality**: Mix of toggles (wallet items) and actions (complete quests/bundles)

**No Config Properties**: These were likely considered too advanced/game-breaking for persistent config

---

## How to Add Category Toggles

### Strategy: Category Master Switches

Add boolean flags to `ModConfig.cs` that control entire categories:

```csharp
// In ModConfig.cs
public class ModConfig
{
    // NEW: Category master toggles
    public bool EnablePlayerAndTools { get; set; } = true;
    public bool EnableFarmAndFishing { get; set; } = true;
    public bool EnableRelationships { get; set; } = true;
    public bool EnableSkills { get; set; } = true;
    public bool EnableTime { get; set; } = true;
    public bool EnableWarps { get; set; } = true;
    public bool EnableWeather { get; set; } = true;
    public bool EnableAdvanced { get; set; } = true;
    
    // Existing individual cheat toggles remain unchanged
    public bool InfiniteHealth { get; set; }
    // ...
}
```

### Implementation Points

#### 1. Update Each Cheat's `OnConfig()` Method

Add a category check before returning `needsUpdate`:

```csharp
// Example: InfiniteHealthCheat.cs
public override void OnConfig(CheatContext context, out bool needsInput, 
                              out bool needsUpdate, out bool needsRendering)
{
    needsInput = false;
    needsUpdate = context.Config.EnablePlayerAndTools  // ← NEW: Category check
                  && context.Config.InfiniteHealth;     // ← Existing: Individual toggle
    needsRendering = false;
}
```

**Impact**: If category is disabled, cheat won't be registered for updates even if individual toggle is on.

#### 2. Update CheatsMenu to Hide Disabled Categories (Optional)

Could filter out cheats from disabled categories in `SetOptions()`.

**OR Better**: Leave the in-game menu alone, only use GMCM for category toggles. The in-game menu becomes a "what's available" view, while GMCM is the "configuration" interface.

---

## Progression Restrictions

### Example: Ginger Island Warp

Add property to ModConfig:
```csharp
public bool RequireBoatForGingerIsland { get; set; } = false;
```

Add check in `WarpCheat.cs` before executing warp:
```csharp
public void WarpTo(string locationName)
{
    // Check if this is a Ginger Island location
    if (IsGingerIslandLocation(locationName))
    {
        // Check progression restriction
        if (Context.Config.RequireBoatForGingerIsland && !IsBoatRepaired())
        {
            Game1.showRedMessage("You must repair the boat first!");
            return;
        }
    }
    
    // Execute warp
    // ...
}

private bool IsBoatRepaired()
{
    // Check if player has seen the boat repair event
    return Game1.MasterPlayer.mailReceived.Contains("willyBoatFixed");
    // OR check if Ginger Island is accessible
}
```

---

## GMCM Integration Plan

### Requirement: Title Screen Only

**User's Requirement**: Configuration must be done from the title screen, not in-game, to prevent easy mid-game cheating.

**Implementation**:
```csharp
// In GenericModConfigMenuIntegration.cs Register() method
menu.Register(
    mod: Manifest,
    reset: Reset,
    save: Save,
    titleScreenOnly: true  // ← THIS IS THE KEY!
);
```

This means:
- ✅ Can configure before loading save
- ❌ Cannot configure during active game
- 🎯 Prevents impulse cheating (Stan-proof!)

---

### Menu Structure Design

```
CJBCheatsMenu Configuration
│
├── ─── CATEGORY TOGGLES ───
│   ├── ☑ Enable Player & Tools
│   ├── ☑ Enable Farm & Fishing
│   ├── ☑ Enable Relationships
│   ├── ☑ Enable Skills
│   ├── ☑ Enable Time
│   ├── ☑ Enable Warps
│   ├── ☑ Enable Weather
│   └── ☑ Enable Advanced
│
├── ─── PLAYER & TOOLS ───
│   ├── ☑ Infinite Health
│   ├── ☑ Infinite Stamina
│   ├── ☑ Instant Cooldowns
│   ├── ☑ One Hit Kill
│   ├── ☑ Max Daily Luck
│   ├── ☑ One Hit Break
│   ├── ☑ Infinite Watering Can
│   └── ☑ Harvest with Scythe
Tool Enchantments
Add Money/Casino Coins/Qi Gems/Golden Walnuts
│
├── ─── FARM & FISHING ───
│   ├── ☑ Auto Water Crops
│   ├── ☑ Durable Fences
│   ├── ☑ Instant Build
│   ├── ☑ Auto Feed Animals
│   ├── ☑ Auto Pet Animals
│   ├── ☑ Auto Pet Pets
│   ├── ☑ Infinite Hay
│   ├── ☑ Instant Fish Bite
│   ├── ☑ Instant Fish Catch
│   ├── ☑ Throw Bobber Max Distance
│   ├── ☑ Durable Tackles
│   └── ☑ Always Treasure Chest
Fast Machine Processing
│
├── ─── RELATIONSHIPS ───
│   ├── ☑ Always Give Gifts
│   └── ☑ No Friendship Decay
Allow setting heart levels?
== For NPC's not yet met?

├── ─── TIME ───
│   ├── ☑ Freeze Time (Outdoor)
│   ├── ☑ Freeze Time (Inside)
│   ├── ☑ Freeze Time (Caves)
│   └── ☑ Hide Time Frozen Message
Allow reset time, day, season, year?
│
├── ─── WARPS ───
│   ├── ☑ Enable Warps (Master)
│   └── ─── Progression Restrictions ───
│       └── ☐ Require boat repair for Ginger Island
Warp Locations restricted:
Sewers
Mastery Cave
Secret Woods
Adventurer's Guild
Bathhouse
Mutant Bug Lair
Quarry
Witch's Swamp
Desert
Ginger Island

│
├── ─── SKILLS & ADVANCED ───
│   └── (Note: Skills/Weather/Advanced are action-based, not toggles)
│       Configure via in-game menu
Skills: 
Allow multiple professions?
│
└── ─── CONTROLS ───
    ├── Open Menu Key: [P]
    ├── Freeze Time Key: [None]
    ├── Grow Tree Key: [NumPad1]
    └── Grow Crops Key: [NumPad2]
```

---

## Implementation Checklist

### Phase 1: Extend ModConfig
- [ ] Add category master toggle properties
- [ ] Add progression restriction properties
- [ ] Test config serialization/deserialization

### Phase 2: Update Individual Cheats
- [ ] Update all PlayerAndTools cheats' `OnConfig()` methods
- [ ] Update all FarmAndFishing cheats' `OnConfig()` methods
- [ ] Update all Relationships cheats' `OnConfig()` methods
- [ ] Update all Time cheats' `OnConfig()` methods
- [ ] Update Warps cheat's `OnConfig()` method
- [ ] Add progression checks to Warps cheat

### Phase 3: Expand GMCM Integration
- [ ] Set `titleScreenOnly: true`
- [ ] Add category toggle section
- [ ] Add individual cheat toggles organized by category
- [ ] Add progression restriction options
- [ ] Test save/load/reset functionality

### Phase 4: Testing
- [ ] Test category toggles disable cheats correctly
- [ ] Test individual toggles work within enabled categories
- [ ] Test progression restrictions work correctly
- [ ] Test title-screen-only enforcement
- [ ] Test backward compatibility with existing config.json
- [ ] Test that in-game menu still works

---

## Key Design Decisions

### ✅ Keep In-Game Menu
The existing in-game menu (press P) should remain functional:
- Shows what cheats are "available" (based on category toggles)
- Allows toggling individual cheats on/off
- Provides UI for action-based cheats (skills, weather, quests, etc.)

### ✅ GMCM for Configuration
GMCM handles:
- Category-level master toggles
- Progression restrictions
- Keybind configuration
- **Title screen only** to prevent mid-game changes

### ✅ Two Layers of Control
1. **GMCM (configured before loading save)**:
   - Category enables/disables
   - Progression restrictions
   
2. **In-Game Menu (during gameplay)**:
   - Individual cheat toggles (within enabled categories)
   - Action buttons (skills, weather, etc.)

This gives maximum flexibility while maintaining the anti-temptation barrier!

---

## Next Steps

1. **Design GMCM menu layout** - Finalize the exact menu structure
2. **Implement Phase 1** - Extend ModConfig with new properties
3. **Implement Phase 2** - Update all cheat OnConfig methods
4. **Implement Phase 3** - Expand GenericModConfigMenuIntegration
5. **Test thoroughly** - Ensure everything works correctly
