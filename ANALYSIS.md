# CJBCheatsMenu Feature Configuration Analysis

## Project Approach: GMCM Integration (Not Separation)

**Decision**: Instead of separating the mod into multiple standalone mods, we will add GMCM (Generic Mod Configuration Menu) integration to allow players to selectively enable/disable features within the unified mod.

**Benefits**:
- ✅ Maintains single mod - easier maintenance
- ✅ Dynamic feature toggling without reinstalling
- ✅ Can implement smart restrictions (progression-based)
- ✅ Standard Stardew Valley modding pattern
- ✅ Prevents "temptation" by disabling unwanted cheats
- ✅ Better user experience with GMCM's polished UI

## Original Mod Structure

### Cheat Categories (Already Separated in Source)
The CJBCheatsMenu source code is already organized into logical categories:

1. **Advanced** - `CJBCheatsMenu.Framework.Cheats.Advanced/`
2. **Farm & Fishing** - `CJBCheatsMenu.Framework.Cheats.FarmAndFishing/`
3. **Player & Tools** - `CJBCheatsMenu.Framework.Cheats.PlayerAndTools/`
4. **Relationships** - `CJBCheatsMenu.Framework.Cheats.Relationships/`
5. **Skills** - `CJBCheatsMenu.Framework.Cheats.Skills/`
6. **Time** - `CJBCheatsMenu.Framework.Cheats.Time/`
7. **Warps** - `CJBCheatsMenu.Framework.Cheats.Warps/`
8. **Weather** - `CJBCheatsMenu.Framework.Cheats.Weather/`

### Common/Shared Components
- `CJB.Common/` - Common utilities
- `CJB.Common.Integrations/` - Integration with other mods
- `CJB.Common.UI/` - UI components
- `CJBCheatsMenu.Framework/` - Core framework
- `CJBCheatsMenu.Framework.Components/` - Shared components
- `CJBCheatsMenu.Framework.ContentModels/` - Content models
- `CJBCheatsMenu.Framework.Models/` - Data models

### Assets & Configuration
- `assets/` - Sprites, icons, and other assets
- `i18n/` - Internationalization/translations
- `config.json` - Configuration file
- `manifest.json` - SMAPI mod manifest

## Implementation Strategy

### Phase 1: Analysis (Current)
- [ ] Examine each cheat category folder
- [ ] Document all individual cheats/features that can be toggled
- [ ] Identify features that should have progression checks
- [ ] Review existing config.json structure
- [ ] Research GMCM API integration

### Phase 2: Design
- [ ] Design configuration data model
  - [ ] Category-level toggles (toggle entire category on/off)
  - [ ] Individual feature toggles within each category
  - [ ] Progression restriction flags
- [ ] Plan GMCM menu structure
- [ ] Define restriction logic (e.g., warp restrictions based on game milestones)

### Phase 3: Implementation
- [ ] Add GMCM integration to ModEntry
- [ ] Extend config.json with toggle flags
- [ ] Add conditional logic to each cheat (check if enabled before executing)
- [ ] Implement progression checks
  - [ ] Check community center completion
  - [ ] Check boat repair status
  - [ ] Check other game milestones as needed
- [ ] Add GMCM menu generation code
- [ ] Ensure graceful fallback if GMCM not installed

### Phase 4: Testing & Documentatdocument individual features
3. Research GMCM API patterns and integration methods
4. Document features that need progression restrictions (e.g., Ginger Island warp)
5. Design the enhanced configuration model

## Example: Warp Category with Restrictions

```
Warps Category
├── Master Toggle: Enable Warps
├── Individual Warps:
│   ├── Warp to Farm ✓ Always available
│   ├── Warp to Town ✓ Always available
│   ├── Warp to Beach ✓ Always available
│   ├── Warp to Ginger Island Farm 🔒 Requires boat repair
│   ├── Warp to Ginger Island Beach 🔒 Requires boat repair
│   └── ... other warps
└── Progression Checks:
    └── Boat Repaired: Game1.MasterPlayer.hasCompletedCommunityCenter() && [boat repair check]
```
- [ ] Test without GMCM (should still work with config.json)
- [ ] Test each toggle combination
- [ ] Test progression restrictions
- [ ] Document configuration options for users
- [ ] Create installation guide

## Feature Categories & Toggles

Each category should have:
1. **Master Toggle** - Enable/disable entire category
2. **Individual Toggles** - Enable/disable specific features within category
3. **Progression Restrictions** (where applicable) - Lock features until milestones reached

## Next Actions
1. Examine the main ModEntry.cs to understand initialization
2. Review each cheat category to understand functionality
3. Document dependencies and shared code usage
