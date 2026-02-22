# GMCM (Generic Mod Configuration Menu) Integration Research

## What is GMCM?
Generic Mod Configuration Menu is a popular Stardew Valley mod that provides a standardized UI for mod configuration. It allows mods to register configuration options that appear in an in-game menu.

## Why Use GMCM?
- ✅ Standard, polished UI that players are familiar with
- ✅ No need to build custom UI
- ✅ Easy to add nested categories and complex options
- ✅ Supports various input types (toggles, dropdowns, sliders, etc.)
- ✅ Optional dependency - mod still works without it

## Integration Pattern

### 1. Add Soft Dependency
In `manifest.json`, add GMCM as an optional dependency:
```json
"Dependencies": [
  {
    "UniqueID": "spacechase0.GenericModConfigMenu",
    "IsRequired": false
  }
]
```

### 2. Check for GMCM at Runtime
```csharp
var gmcmApi = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
if (gmcmApi != null)
{
    // GMCM is installed, register config
    RegisterGMCMConfig(gmcmApi);
}
```

### 3. Register Configuration Options
```csharp
private void RegisterGMCMConfig(IGenericModConfigMenuApi api)
{
    // Register mod
    api.Register(
        mod: this.ModManifest,
        reset: () => this.Config = new ModConfig(),
        save: () => this.Helper.WriteConfig(this.Config)
    );

    // Add options
    api.AddBoolOption(
        mod: this.ModManifest,
        name: () => "Enable Warps",
        tooltip: () => "Enable or disable all warp cheats",
        getValue: () => this.Config.EnableWarps,
        setValue: value => this.Config.EnableWarps = value
    );

    // Add section headers
    api.AddSectionTitle(
        mod: this.ModManifest,
        text: () => "Warp Options"
    );

    // Nested options with conditional display
    api.AddBoolOption(
        mod: this.ModManifest,
        name: () => "  Ginger Island Warp",
        tooltip: () => "Allow warping to Ginger Island (requires boat repair)",
        getValue: () => this.Config.EnableGingerIslandWarp,
        setValue: value => this.Config.EnableGingerIslandWarp = value
    );
}
```

## GMCM API Interface
```csharp
public interface IGenericModConfigMenuApi
{
    void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
    void Unregister(IManifest mod);
    
    void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
    void AddParagraph(IManifest mod, Func<string> text);
    void AddImage(IManifest mod, Func<Texture2D> texture, Rectangle? texturePixelArea = null, int scale = 4);
    
    void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    void AddIntOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
    void AddFloatOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);
    void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
    void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    
    void AddPage(IManifest mod, string pageId, Func<string> pageTitle = null);
    void AddPageLink(IManifest mod, string pageId, Func<string> text, Func<string> tooltip = null);
    
    // ... other methods
}
```

## Our Implementation Plan

### Configuration Model Structure
```csharp
public class ModConfig
{
    // Category master toggles
    public bool EnableAdvanced { get; set; } = true;
    public bool EnableFarmAndFishing { get; set; } = true;
    public bool EnablePlayerAndTools { get; set; } = true;
    public bool EnableRelationships { get; set; } = true;
    public bool EnableSkills { get; set; } = true;
    public bool EnableTime { get; set; } = true;
    public bool EnableWarps { get; set; } = true;
    public bool EnableWeather { get; set; } = true;
    
    // Individual feature toggles (warps example)
    public WarpConfig Warps { get; set; } = new WarpConfig();
    
    // ... other categories
}

public class WarpConfig
{
    public bool EnableFarmWarp { get; set; } = true;
    public bool EnableTownWarp { get; set; } = true;
    public bool EnableGingerIslandWarp { get; set; } = true;
    public bool RespectProgressionRestrictions { get; set; } = true;
    // ... other warps
}
```

### Menu Structure
```
CJBCheatsMenu Configuration
├── [General Settings]
│   └── Open Cheats Menu Keybind
├── ─── Advanced ───
│   ├── ☑ Enable Advanced Category
│   └── [Individual toggles...]
├── ─── Farm & Fishing ───
│   ├── ☑ Enable Farm & Fishing Category
│   └── [Individual toggles...]
├── ─── Player & Tools ───
│   ├── ☑ Enable Player & Tools Category
│   └── [Individual toggles...]
├── ─── Relationships ───
│   ├── ☑ Enable Relationships Category
│   └── [Individual toggles...]
├── ─── Skills ───
│   ├── ☑ Enable Skills Category
│   └── [Individual toggles...]
├── ─── Time ───
│   ├── ☑ Enable Time Category
│   └── [Individual toggles...]
├── ─── Warps ───
│   ├── ☑ Enable Warps Category
│   ├── ☑ Respect Progression Restrictions
│   └── [Individual warp toggles...]
└── ─── Weather ───
    ├── ☑ Enable Weather Category
    └── [Individual toggles...]
```

## Testing Checklist
- [ ] Test with GMCM installed - config menu appears
- [ ] Test without GMCM - mod still functions, uses config.json
- [ ] Test toggle combinations
- [ ] Test save/load of config
- [ ] Test defaults for new installs
- [ ] Test progression restrictions work correctly

## Resources
- GMCM Mod Page: https://www.nexusmods.com/stardewvalley/mods/5098
- GMCM on GitHub: https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu
- SMAPI Mod Integration: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations
