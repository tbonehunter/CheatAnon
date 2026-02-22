# CJBCheatsMenu - Feature Configuration Enhancement

## Overview
This project enhances the CJBCheatsMenu mod for Stardew Valley by adding a GMCM (Generic Mod Configuration Menu) interface that allows players to selectively enable/disable specific cheats and set intelligent restrictions.

## Purpose
The original CJBCheatsMenu is a comprehensive mod with many features. By adding configurable toggles, we provide:
- **Self-Control**: Players can disable cheats they want to avoid (looking at you, Stan!)
- **Customization**: Enable only the cheats you actually want to use
- **Smart Restrictions**: Progression-based limitations (e.g., no Ginger Island warp until boat is repaired)
- **Maintainability**: Single mod, easier to update and maintain

## Project Status
✅ **IMPLEMENTATION COMPLETE** - February 19, 2026

All 5 phases implemented and committed:
- **Phase 1**: Extended ModConfig.cs with 9 new properties
- **Phase 2**: Added category checks to 28 cheat files + CheatContext helpers
- **Phase 3**: GMCM integration with category toggles and progression restrictions
- **Phase 4**: Dynamic tab filtering based on enabled categories
- **Phase 5**: Warp progression restrictions with 10 location checks

**Commits**:
- e058f56: Phases 1-3 (ModConfig + category checks + GMCM)
- 852abab: Phase 4 (dynamic tab filtering)
- 2e71466: Phase 5 (warp progression restrictions)

**Ready for testing in Stardew Valley with SMAPI.**

## Features Implemented

### Category Master Toggles (GMCM - Title Screen Only)
- Enable/Disable Player & Tools cheats
- Enable/Disable Farm & Fishing cheats
- Enable/Disable Relationships cheats
- Enable/Disable Skills cheats
- Enable/Disable Time cheats
- Enable/Disable Warps cheats
- Enable/Disable Weather cheats
- Enable/Disable Advanced cheats

All category toggles default to **enabled** for backward compatibility.

### Progression Restrictions (GMCM - Can Toggle Anytime)
- **Enforce Warp Progression Restrictions**: Master toggle (default OFF)
  - When enabled, warps require meeting in-game progression requirements
  - Desert: Vault bundles or Joja membership
  - Ginger Island: Boat repair complete
  - Secret Woods: Steel Axe or better
  - Sewer: Rusty Key obtained
  - Mutant Bug Lair: Dark Talisman quest + Rusty Key
  - Wizard Tower: Entered Community Center
  - Railroad/Bathhouse/Witch's Swamp: Summer 3 Year 1+
  - Quarry: Crafts Room bundles or Joja membership
  - Mastery Cave: Reached Mastery level

### In-Game Menu Behavior
- Disabled category tabs are **hidden** (not grayed out)
- Individual feature toggles remain in the in-game menu (Press P)
- Controls tab always visible
- Tab navigation automatically adjusts to visible tabs

## Structure
This workspace contains:
- `/Analysis` - Documentation of existing features and dependencies
- `/CJBCheatsMenu` - Modified source code with GMCM integration
- `/Documentation` - Feature toggles and restrictions documentation
- `/Implementation` - GMCM integration code and configuration system
- `STATUS.md` - Detailed implementation progress tracker
- `GMCM-Menu-Structure.md` - Complete GMCM design specification

## Testing Checklist
1. **Build & Deploy**: Compile in Visual Studio, copy to Mods folder
2. **GMCM Integration**: Open GMCM from title screen, verify 8 category toggles + 1 progression restriction appear
3. **Category Toggles**: Disable a category, restart game, verify that tab is hidden in-game menu
4. **Progression Restrictions**: Enable warp restrictions, verify locked warps don't appear
5. **Backward Compatibility**: Existing config.json loads with all categories enabled by default

## Requirements
- Visual Studio 2022 or VS Code with C# extensions
- SMAPI (Stardew Modding API)
- Generic Mod Configuration Menu (GMCM) mod
- Stardew Valley 1.6+

## Technical Approach
- Integrated GMCM API for configuration UI (titleScreenOnly for category toggles)
- Added toggle flags for each cheat category (8 categories)
- Implemented progression checks for 10 warp locations
- Maintained backward compatibility with existing config.json (all defaults enabled)
- Added user-friendly English descriptions (i18n support deferred)
- Dynamic UI filtering hides disabled category tabs

## Implementation Details
- **34 files modified** across Framework, Cheats, and Models
- **1 new file created**: WarpRestrictions.cs (10 progression check methods)
- **28 cheat files updated** with category enable checks
- **Zero compilation errors** - ready for deployment
## Contributing
This is a modular separation project. Each mod will maintain the original functionality while being completely standalone.
