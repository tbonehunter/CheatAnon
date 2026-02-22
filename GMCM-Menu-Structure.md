# CJBCheatsMenu - GMCM Configuration Structure

**Configuration Note**: All settings are configurable from the title screen only (not during active gameplay)

**Design Philosophy**: 
- GMCM provides ***category-level*** control (title screen only)
- In-game menu (Press P) provides ***feature-level*** control (during gameplay)
- Players choose which categories they want access to, then fine-tune features in-game

---

## CATEGORY MASTER TOGGLES

**What these control**: Enable/disable entire cheat categories

- [ ] Enable Player & Tools
- [ ] Enable Farm & Fishing
- [ ] Enable Relationships
- [ ] Enable Skills
- [ ] Enable Time
- [ ] Enable Warps
- [ ] Enable Weather
- [ ] Enable Advanced

**Default**: All enabled (backward compatible with existing behavior)

**Behavior when disabled**:
- Category's cheats will not execute
- In-game menu section is hidden (not grayed out)
- Config values preserved (re-enabling restores previous settings)

---

## PROGRESSION RESTRICTIONS

### Master Toggle (All or Nothing)
- [ ] **Enforce Warp Progression Restrictions** (default: OFF)
  - When disabled: All warps available immediately (classic cheat behavior)
  - When enabled: All warp restrictions below apply automatically

**Rationale**: If players want to self-restrict one warp, they likely want all progression-appropriate restrictions.

### Automatic Warp Restrictions (When Master Toggle Enabled)
**Philosophy**: Warps only become available after the player has legitimately accessed the location at least once.

**Note**: These apply automatically when master toggle is ON. No individual restriction toggles.

#### Bundle-Based Restrictions
- Vault bundles complete (bus repair) → Desert warps unlocked
- Crafts Room bundles complete → Quarry warp unlocked
- **Joja Route**: Joja membership bypasses ALL bundle restrictions

#### Date-Based Restrictions
- Summer 3+ Year 1 (rubble cleared) → Railroad/Bathhouse/Witch's Swamp warps unlocked

#### Quest-Based Restrictions
- Community Center entered → Wizard Tower warp unlocked
- Rusty Key obtained → Sewer warp unlocked
- Dark Talisman quest complete + Rusty Key → Mutant Bug Lair warp unlocked
- Boat repair complete → Ginger Island warps unlocked

#### Tool-Based Restrictions
- Steel Axe or better → Secret Woods warp unlocked

#### Other Restrictions
- Mastery level reached → Mastery Cave warp unlocked
- Met NPC → Can change their heart level
- Vanilla profession rules enforced

---

## INDIVIDUAL FEATURE TOGGLES (In-Game Menu Only)

**Important**: Individual feature toggles are NOT exposed in GMCM. They remain in the in-game menu (Press P) only.

**Why**: 
- Simplifies GMCM configuration (category-level discipline at title screen)
- Allows flexibility during gameplay (fine-tune without restarting)
- Reduces GMCM menu length (less overwhelming)
- Preserves existing in-game menu functionality

### Player & Tools Features (In-Game Menu)
- Infinite Health
- Infinite Stamina
- Instant Cooldowns
- One Hit Kill
- Max Daily Luck
- One Hit Break
- Infinite Watering Can
- Harvest with Scythe
- Tool Enchantments (action buttons)
- Add Money/Casino Coins/Qi Gems/Golden Walnuts (action buttons)

### Farm & Fishing Features (In-Game Menu)
- Auto Water Crops
- Durable Fences
- Instant Build
- Auto Feed Animals
- Auto Pet Animals
- Auto Pet Pets
- Infinite Hay
- Fast Machine Processing
- Instant Fish Bite
- Instant Fish Catch
- Throw Bobber Max Distance
- Durable Tackles
- Always Treasure Chest

### Relationships Features (In-Game Menu)
- Always Give Gifts
- No Friendship Decay
- Heart Level Changes (action buttons)

### Time Features (In-Game Menu)
- Freeze Time (Outdoor)
- Freeze Time (Inside)
- Freeze Time (Caves)
- Hide Time Frozen Message
- Time/Date adjustment (action buttons)

### Skills Features (In-Game Menu)
- Skill level adjustments (action buttons)
- Profession changes (action buttons)

### Warps Features (In-Game Menu)
- Individual warp locations (menu of locations)
- Progression restrictions apply automatically if enabled

### Weather Features (In-Game Menu)
- Set tomorrow's weather (action dropdown)

### Advanced Features (In-Game Menu)
- Complete quests (action buttons)
- Toggle wallet items (action buttons)
- Unlock doors/content (action buttons)
- Complete bundles (action buttons)

---

## CONTROLS (Hotkeys)

- [ ] Open Menu Key: (default: **None** - player should set to avoid conflicts)
- [ ] Freeze Time Key: (default: **None**)
- [ ] Grow Tree Key: (default: **None** - was NumPad1, but avoid defaults)
- [ ] Grow Crops Key: (default: **None** - was NumPad2, but avoid defaults)

**Philosophy**: No hotkey defaults to prevent mod conflicts. Players choose their own bindings.

---

## MULTIPLAYER BEHAVIOR

- **Configuration**: Host's GMCM configuration applies to entire farm
- **Viewing**: Farmhands see host's category enables/disables in their in-game menus
- **Category Toggles**: Host controls which categories are available
- **Individual Features**: Farmhands can toggle features within enabled categories for themselves
- **Progression Restrictions**: Evaluated based on host's game progress (host is master player)

---

## Implementation Notes

### Default Values
- **Category Toggles**: All default to `true` (enabled) - backward compatible
- **Progression Restrictions Master Toggle**: Defaults to `false` (unrestricted) - classic behavior
- **Individual Feature Toggles**: Match existing mod defaults (not in GMCM)
- **Hotkeys**: All default to **None** (unbound) - players set their own

### Two-Layer Control System
1. **GMCM (Title Screen Only)**:
   - 8 category master toggles
   - 1 progression restrictions master toggle
   - Hotkey bindings
   
2. **In-Game Menu (Press P During Gameplay)**:
   - Individual feature toggles (checkboxes)
   - Action buttons (instant effects)
   - Warp location selector
   - Only shows enabled categories (disabled = hidden sections)

### Behavior When Category Disabled
- **Code Execution**: Category's cheats completely skip execution (OnConfig returns needsUpdate=false)
- **In-Game Menu**: Entire section is hidden (not grayed out, completely removed from UI)
- **Config Preservation**: Individual feature toggle values are preserved in config.json
- **Re-enabling**: When category re-enabled, previous individual settings are restored

### Joja Route Handling
- **Bundle Restrictions**: Check for both Community Center completion OR Joja membership
- **Implementation**: `HasCompletedVault() => ccVault mail OR Joja membership active`
- **Philosophy**: Joja membership bypasses ALL bundle-based warp restrictions
- **Why**: Players who chose Joja have already decided to bypass progression

### Backward Compatibility
- Existing config.json files load correctly (new properties have defaults)
- If GMCM not installed, in-game menu continues to work normally
- All categories enabled by default = identical to pre-GMCM behavior
- Old hotkey defaults (P, NumPad1, NumPad2) preserved for existing users
