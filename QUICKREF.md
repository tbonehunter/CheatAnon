# Quick Reference Guide

## Project Overview
This workspace is for adding GMCM (Generic Mod Configuration Menu) integration to CJBCheatsMenu, allowing players to selectively enable/disable cheats and set progression-based restrictions.

## Folder Structure

```
Separate Mods from Cheats/
├── .github/
│   └── copilot-instructions.md    # Workspace-specific AI instructions
├── Analysis/
│   ├── Dependencies/               # Document code dependencies
│   └── Features/                   # Detailed feature analysis (per-category)
├── Implementation/                 # GMCM integration code (when ready)
├── Documentation/
│   ├── Feature-List.md            # Complete catalog of all features
│   └── Progression-Restrictions.md # Progression-based restriction logic
├── Research/
│   └── GMCM-Integration.md        # GMCM API research and patterns
├── ANALYSIS.md                     # Overall implementation strategy
├── README.md                       # Project overview
├── STATUS.md                       # Current progress tracker
└── QUICKREF.md                     # This file
```

## Parent Folder (Source Code)
The parent CJBCheatsMenu folder contains the original mod source:
- `../CJBCheatsMenu.Framework.Cheats.*/` - Cheat category folders
- `../CJB.Common*/` - Common utilities and UI
- `../assets/` - Sprites and resources
- `../i18n/` - Translations
- `../manifest.json` - SMAPI manifest

## Workflow

### 1. Analysis Phase (Current)
- Read source files from parent folder
- Document all features in Documentation/Feature-List.md
- Analyze each category in Analysis/Features/
- Identify progression-restricted features
- Research GMCM integration patterns

### 2. Design Phase
- Design enhanced config model with toggles
- Plan GMCM menu structure
- Define restriction logic
- Create API integration plan

### 3. Implementation Phase
- Extend ModConfig class with feature toggles
- Add GMCM registration code
- Add conditional checks to each cheat
- Implement progression restriction logic
- Test with/without GMCM

### 4. Testing Phase
- Test each toggle combination
- Test progression restrictions
- Verify backward compatibility
- Document configuration options
, add GMCM here
2. `../CJBCheatsMenu.Framework/ModConfig.cs` - Config model to extend
3. `../CJBCheatsMenu.Framework.Cheats.*/` - Individual cheat implementations
4. `../manifest.json` - Add GMCM as optional dependency
5. `Research/GMCM-Integration.md` - API documentation and patterns
6. `Documentation/Feature-List.md` - Complete feature catalog
7. `Documentation/Progression-Restrictions.md` - Restriction logics
4. `../manifest.json` - Mod metadata
5. `../CJBCheatsMenu.csproj` - Build configuration

## Commands

### Build Original Mod
```powershell
cd "../"
dotnet build CJBCheatsMenu.csproj
```Test in Game
```powershell
# Build and deploy to Stardew Valley Mods folder
# (Exact path depends on your SMAPI installation)
mkdir "[ModName]"
# Copy template files
```

##GMCM is an optional dependency - mod must work without it
- Each cheat needs a config toggle and runtime check
- Progression restrictions should be opt-in (disabled by default)
- Test with Stan! 😄 Make sure self-control features work
- Keep user-facing text clear and friendly
- Maintain backward compatibility with existing config.json

## Implementation Checklist
- [ ] Document all 8 categories and their features
- [ ] Research GMCM API thoroughly
- [ ] Design config model with all toggles
- [ ] Add GMCM dependency to manifest
- [ ] Implement GMCM registration code
- [ ] Add conditional checks to each cheat
- [ ] Implement progression restriction logic
- [ ] Test all toggle combinations
- [ ] Test progression restrictions
- [ ] Create user documentationods
- Document any breaking changes

## Resources
- SMAPI Documentation: https://stardewvalleywiki.com/Modding:Index
- SMAPI Mod Build Config: https://smapi.io/package/mod-build-config
- Original CJBCheatsMenu: Source in parent folder
