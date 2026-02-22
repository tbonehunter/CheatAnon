# Progression-Based Restrictions

This document tracks features that should be locked until certain game milestones are reached.

## Purpose
Allow players to set restrictions that prevent cheating before achieving certain milestones. This helps maintain game progression while still having cheats available for convenience later.

**Important**: All progression restrictions are **OPTIONAL** and controlled by a master toggle in GMCM. Players can choose:
- **Restrictions OFF** (default): All warps/features available immediately (classic cheat mod behavior)
- **Restrictions ON**: Warps/features locked until vanilla unlock conditions are met

## Configuration Properties

### Master Toggle
```csharp
public bool EnforceWarpProgressionRestrictions { get; set; } = false;
```

When `false` (default), all warp restrictions are bypassed. When `true`, individual restrictions below are checked.

## Restriction Categories

### 1. Boat Repair Required
**Game Check**: Check if the boat to Ginger Island has been repaired

**Affected Features**:
- [ ] Warp to Ginger Island Farm
- [ ] Warp to Ginger Island Beach  
- [ ] Warp to Ginger Island Jungle
- [ ] Warp to Ginger Island Dig Site
- [ ] Any other Ginger Island locations

**Implementation**:
```csharp
bool IsBoatRepaired()
{
    // Need to find the appropriate game state check
    // Likely: Game1.MasterPlayer.mailReceived.Contains("willyBoatFixed")
    // Or check for specific quest completion
}
```

---

### 2. Community Center Completion
**Game Check**: Check if Community Center is complete

**Affected Features**:
- [ ] TBD - Identify features that should be locked until CC complete

**Implementation**:
```csharp
bool IsCommunitycenterComplete()
{
    return Game1.MasterPlayer.hasCompletedCommunityCenter();
}
```

---

### 3. Desert Access Unlocked (Vault Bundles Complete)
**Game Check**: Check if Vault bundles are complete (bus repaired) OR Joja membership active

**Affected Features**:
- [ ] Warp to Desert (Sandy's Shop)
- [ ] Warp to Skull Cavern
- [ ] Warp to Casino (also requires Club Card separately)

**Implementation**:
```csharp
bool IsDesertUnlocked()
{
    // Community Center route
    bool ccRoute = Game1.MasterPlayer.mailReceived.Contains("ccVault");
    
    // Joja route - membership bypasses bundle restrictions
    bool jojaRoute = Game1.MasterPlayer.mailReceived.Contains("JojaMember");
    
    return ccRoute || jojaRoute;
}
```

**Rationale**: Bus repair (Vault bundles) grants legitimate access to Desert area in vanilla game. Joja membership bypasses all bundle-based restrictions as players have chosen the "pay to skip" route.

**Important: Joja Route Philosophy**
- Players who purchase Joja membership have already decided to bypass traditional progression
- ALL bundle-based restrictions should check for Joja membership as an alternative
- This includes: Desert (Vault), Quarry (Crafts Room), and any other bundle-unlocked locations

---

### 4. Skull Cavern Unlocked
**Game Check**: Check if Skull Cavern is accessible

**Affected Features**:
- [ ] Warp to Skull Cavern (if we want to restrict it)
- [ ] TBD

---

### 5. Sewer Access Unlocked
**Game Check**: Check if player has Rusty Key

**Affected Features**:
- [ ] Warp to Sewers

**Implementation**:
```csharp
bool IsSewerUnlocked()
{
    return Game1.player.hasRustyKey;
}
```

**Rationale**: Rusty Key is required to access sewers in vanilla game.

---

### 6. Railroad/Bathhouse Area Unlocked
**Game Check**: Check if date is Summer 3 Year 1 or later (rubble cleared)

**Affected Features**:
- [ ] Warp to Railroad area
- [ ] Warp to Bathhouse
- [ ] Warp to Witch's Swamp

**Implementation**:
```csharp
bool IsRailroadUnlocked()
{
    // Rubble clears on night of Summer 2, Year 1
    // So it's accessible starting Summer 3, Year 1
    int currentDate = (Game1.year - 1) * 112 + Game1.dayOfMonth + (Game1.season.ToLower() switch
    {
        "spring" => 0,
        "summer" => 28,
        "fall" => 56,
        "winter" => 84,
        _ => 0
    });
    
    int unlockDate = 28 + 2; // Summer 2 (day 30 of year 1)
    return currentDate >= unlockDate;
}
```

**Rationale**: The rubble blocking the Railroad/Bathhouse area is cleared the night of Summer 2, Year 1. Player can access these areas starting Summer 3, Year 1.

---

### 7. Quarry Unlocked
**Game Check**: Check if Crafts Room bundles are complete OR Joja membership active

**Affected Features**:
- [ ] Warp to Quarry

**Implementation**:
```csharp
bool IsQuarryUnlocked()
{
    // Community Center route
    bool ccRoute = Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom");
    
    // Joja route - membership bypasses bundle restrictions
    bool jojaRoute = Game1.MasterPlayer.mailReceived.Contains("JojaMember");
    
    return ccRoute || jojaRoute;
}
```

**Rationale**: Completing Crafts Room bundles removes the boulder blocking Quarry access. Joja membership bypasses this restriction.

---

### 8. Mutant Bug Lair Unlocked
**Game Check**: Check if Dark Talisman quest is complete AND player has Rusty Key

**Affected Features**:
- [ ] Warp to Mutant Bug Lair

**Implementation**:
```csharp
bool IsMutantBugLairUnlocked()
{
    // Mutant Bug Lair is INSIDE the Sewer, so requires both:
    // 1. Rusty Key (to access Sewer)
    // 2. Dark Talisman quest completion (to unlock Bug Lair within Sewer)
    
    bool hasSewerAccess = Game1.player.hasRustyKey;
    bool hasCompletedDarkTalismanQuest = Game1.player.hasMagicInk; // Received after quest
    
    return hasSewerAccess && hasCompletedDarkTalismanQuest;
}
```

**Rationale**: Mutant Bug Lair is located inside the Sewer, so it has a dependency chain:
1. First need Rusty Key to enter Sewer
2. Then need to complete Dark Talisman quest (which unlocks the Bug Lair entrance within the Sewer)

---

### 9. Secret Woods Unlocked
**Game Check**: Check if player has Steel Axe or better

**Affected Features**:
- [ ] Warp to Secret Woods

**Implementation**:
```csharp
bool IsSecretWoodsUnlocked()
{
    // Check if player has at least Steel Axe
    Tool axe = Game1.player.getToolFromName("Axe");
    if (axe != null)
    {
        return axe.UpgradeLevel >= 2; // 0=Basic, 1=Copper, 2=Steel, 3=Gold, 4=Iridium
    }
    return false;
}
```

**Rationale**: Secret Woods requires Steel Axe to remove the blocking hardwood log.

---

### 10. Wizard Tower Unlocked
**Game Check**: Check if player has entered Community Center

**Affected Features**:
- [ ] Warp to Wizard Tower area

**Implementation**:
```csharp
bool IsWizardTowerUnlocked()
{
    // Check if player has triggered the Community Center unlock/cutscene
    return Game1.MasterPlayer.mailReceived.Contains("wizardJunimoNote") || 
           Game1.MasterPlayer.eventsSeen.Contains(611439); // CC unlock event
}
```

**Rationale**: Wizard Tower becomes relevant after Community Center is discovered and Wizard sends his first letter.

---

## Configuration Design

### Global Progression Setting
Add a master toggle:
```csharp
public bool EnableProgressionRestrictions { get; set; } = false;
```

If `EnableProgressionRestrictions` is `false`, all restrictions are ignored (default behavior - all cheats available).

If `true`, specific restrictions apply based on game state.

### Per-Category Restrictions
Each restricted feature can have its own toggle:
```csharp
public class WarpConfig
{
    // Master toggle for category
    public bool EnableWarps { get; set; } = true;
    
    // Individual warp toggles
    public bool EnableGingerIslandWarps { get; set; } = true;
    
    // Restriction toggle
    public bool RequireBoatForGingerIsland { get; set; } = false;
}
```

### Runtime Check Pattern
```csharp
public void WarpToGingerIsland()
{
    // Check if category enabled
    if (!Config.Warps.EnableWarps)
        return;
    
    // Check if specific feature enabled
    if (!Config.Warps.EnableGingerIslandWarps)
        return;
    
    // Check progression restriction
    if (Config.Warps.RequireBoatForGingerIsland && !IsBoatRepaired())
    {
        Game1.showRedMessage("You need to repair the boat first!");
        return;
    }
    
    // Execute warp
    DoWarp("IslandSouth");
}
```

## UI Design for GMCM

### Option 1: Global Toggle + Per-Feature Overrides
```
─── Progression Restrictions ───
☐ Enable Progression Restrictions (Master)

─── Warps ───
☑ Enable Warps
☐ Require boat repair for Ginger Island
```

### Option 2: Just Per-Feature (Stan's Preference?)
```
─── Warps ───
☑ Enable Warps
  ☑ Enable Town Warp
  ☑ Enable Beach Warp
  ☑ Enable Ginger Island Warp
    ☐ Lock until boat is repaired
```

This gives granular control without a confusing global setting.

## Questions to Resolve
- [ ] Which features should have progression restrictions?
- [ ] Should restrictions be opt-in (false by default) or opt-out (true by default)?
- [ ] Should there be a master "Enable Progression Restrictions" toggle, or just per-feature?
- [ ] What other game milestones should we check for?

## Next Steps
1. Analyze warp code to find all locations
2. Identify appropriate game state checks for each restriction
3. Document other categories that might benefit from restrictions
4. Decide on UI/config structure
