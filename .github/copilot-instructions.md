# CJBCheatsMenu Feature Configuration Enhancement

## Project Overview
This workspace is dedicated to enhancing the CJBCheatsMenu mod for Stardew Valley by adding GMCM (Generic Mod Configuration Menu) integration. This allows players to selectively enable/disable specific cheats and set progression-based restrictions.

## Project Goals
- **Add GMCM Integration**: Create a user-friendly configuration interface
- **Feature Toggles**: Allow players to enable/disable individual cheats or entire categories
- **Smart Restrictions**: Implement progression-based limitations (e.g., locked warps until certain milestones)
- **Self-Control Features**: Help players avoid temptation by disabling unwanted cheats
- **Maintain Compatibility**: Preserve existing functionality and config.json structure

## Development Guidelines
- Use SMAPI (Stardew Modding API) standards for all development
- Integrate with Generic Mod Configuration Menu (GMCM) API
- Follow C# and .NET best practices
- Maintain backward compatibility with existing installations
- Add clear, user-friendly descriptions for all configuration options
- Implement graceful degradation if GMCM is not installed

## Workspace Tasks
- [ ] Analyze CJBCheatsMenu source code structure
- [ ] Document all features/cheats in each category (8 categories)
- [ ] Research GMCM API integration
- [ ] Design configuration structure
  - [ ] Category-level toggles
  - [ ] Individual feature toggles
  - [ ] Progression-based restrictions
- [ ] Implement GMCM integration
- [ ] Add feature toggle logic throughout codebase
- [ ] Implement smart restrictions (e.g., warp restrictions based on game progress)
- [ ] Test with GMCM installed and uninstalled
- [ ] Create documentation for users

## Technical Requirements
- **Language**: C#
- **Framework**: .NET (compatible with SMAPI)
- **Target**: Stardew Valley with SMAPI
- **Dependencies**: 
  - SMAPI
  - nalyze the existing CJBCheatsMenu codebase
2. Document all toggleable features
3. Research GMCM API integration patterns
4. Design the configuration data model
5. Implement GMCM integration
6. Add feature toggle logic
7. Implement progression-based restrictions (e.g., boat repair check for Ginger Island warp)
1. **Advanced** - Advanced cheats and features
2. **Farm & Fishing** - Farm and fishing related cheats
3. **Player & Tools** - Player stats and tool modifications
4. **Relationships** - Friendship and relationship cheats
5. **Skills** - Skill level modifications
6. **Time** - Time manipulation features
7. **Warps** - Location warping (with progression checks)
8. **Weather** - Weather control features

## Next Steps
1. Access the parent CJBCheatsMenu folder to analyze the original mod
2. Inventory all features and their dependencies
3. Create a separation plan
4. Begin implementing individual mods
