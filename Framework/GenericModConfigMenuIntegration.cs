using System;
using CJB.Common.Integrations;
using CJBCheatsMenu.Framework.Models;
using StardewModdingAPI;

namespace CJBCheatsMenu.Framework;

/// <summary>Registers the mod configuration with Generic Mod Config Menu.</summary>
internal class GenericModConfigMenuIntegration
{
    /*********
    ** Fields
    *********/
    /// <summary>The CJB Cheats Menu manifest.</summary>
    private readonly IManifest Manifest;

    /// <summary>The Generic Mod Config Menu integration.</summary>
    private readonly IGenericModConfigMenuApi? ConfigMenu;

    /// <summary>The current mod settings.</summary>
    private readonly ModConfig Config;

    /// <summary>Save the mod's current config to the <c>config.json</c> file.</summary>
    private readonly Action Save;


    /*********
    ** Public methods
    *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="manifest">The CJB Cheats Menu manifest.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="config">Get the current mod config.</param>
    /// <param name="save">Save the mod's current config to the <c>config.json</c> file.</param>
    public GenericModConfigMenuIntegration(IManifest manifest, IModRegistry modRegistry, ModConfig config, Action save)
    {
        this.Manifest = manifest;
        this.ConfigMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        this.Config = config;
        this.Save = save;
    }

    /// <summary>Register the config menu if available.</summary>
    public void Register()
    {
        var menu = this.ConfigMenu;
        if (menu is null)
            return;

        // Register with titleScreenOnly=true so category toggles only work from title screen
        menu.Register(this.Manifest, this.Reset, this.Save, titleScreenOnly: true);

        // controls
        menu.AddSectionTitle(this.Manifest, I18n.Controls_Title);
        menu.AddKeybindList(
            mod: this.Manifest,
            name: I18n.Controls_OpenMenu,
            tooltip: I18n.Config_OpenMenu_Desc,
            getValue: () => this.Config.OpenMenuKey,
            setValue: value => this.Config.OpenMenuKey = value
        );
        menu.AddKeybindList(
            mod: this.Manifest,
            name: I18n.Controls_FreezeTime,
            tooltip: I18n.Config_FreezeTime_Desc,
            getValue: () => this.Config.FreezeTimeKey,
            setValue: value => this.Config.FreezeTimeKey = value
        );
        menu.AddKeybindList(
            mod: this.Manifest,
            name: I18n.Controls_GrowTree,
            tooltip: I18n.Config_GrowTree_Desc,
            getValue: () => this.Config.GrowTreeKey,
            setValue: value => this.Config.GrowTreeKey = value
        );
        menu.AddKeybindList(
            mod: this.Manifest,
            name: I18n.Controls_GrowCrops,
            tooltip: I18n.Config_GrowCrops_Desc,
            getValue: () => this.Config.GrowCropsKey,
            setValue: value => this.Config.GrowCropsKey = value
        );
        menu.AddKeybindList(
            mod: this.Manifest,
            name: I18n.Controls_ReloadConfig,
            tooltip: I18n.Controls_ReloadConfig_Desc,
            getValue: () => this.Config.ReloadConfigKey,
            setValue: value => this.Config.ReloadConfigKey = value
        );

        // category toggles (title screen only)
        menu.AddSectionTitle(this.Manifest, () => "Enable/Disable Cheat Categories");
        menu.AddParagraph(this.Manifest, () => "Toggle entire cheat categories on/off. Changes only take effect from the title screen.");
        
        menu.AddBoolOption(
            mod: this.Manifest,
            name: () => "Enable Player & Tools",
            tooltip: () => "Enable or disable all Player & Tools cheats (health, stamina, speed, etc.)",
            getValue: () => this.Config.EnablePlayerAndTools,
            setValue: value => this.Config.EnablePlayerAndTools = value
        );
        menu.AddBoolOption(
            mod: this.Manifest,
            name: () => "Enable Farm & Fishing",
            tooltip: () => "Enable or disable all Farm & Fishing cheats (auto-water, instant fish, fast machines, etc.)",
            getValue: () => this.Config.EnableFarmAndFishing,
            setValue: value => this.Config.EnableFarmAndFishing = value
        );
        menu.AddBoolOption(
            mod: this.Manifest,
            name: () => "Enable Relationships",
            tooltip: () => "Enable or disable all Relationships cheats (hearts, friendship decay, gifts, etc.)",
            getValue: () => this.Config.EnableRelationships,
            setValue: value => this.Config.EnableRelationships = value
        );
        menu.AddBoolOption(
            mod: this.Manifest,
            name: () => "Enable Skills",
            tooltip: () => "Enable or disable all Skills cheats (skill levels, professions, etc.)",
            getValue: () => this.Config.EnableSkills,
            setValue: value => this.Config.EnableSkills = value
        );
        menu.AddBoolOption(
            mod: this.Manifest,
            name: () => "Enable Time",
            tooltip: () => "Enable or disable all Time cheats (freeze time, set time/day/season/year, etc.)",
            getValue: () => this.Config.EnableTime,
            setValue: value => this.Config.EnableTime = value
        );
        menu.AddBoolOption(
            mod: this.Manifest,
            name: () => "Enable Warps",
            tooltip: () => "Enable or disable all Warp cheats (teleport to locations)",
            getValue: () => this.Config.EnableWarps,
            setValue: value => this.Config.EnableWarps = value
        );
        menu.AddBoolOption(
            mod: this.Manifest,
            name: () => "Enable Weather",
            tooltip: () => "Enable or disable all Weather cheats (set weather for tomorrow)",
            getValue: () => this.Config.EnableWeather,
            setValue: value => this.Config.EnableWeather = value
        );
        menu.AddBoolOption(
            mod: this.Manifest,
            name: () => "Enable Advanced",
            tooltip: () => "Enable or disable all Advanced cheats (quests, bundles, unlocks, wallet items, etc.)",
            getValue: () => this.Config.EnableAdvanced,
            setValue: value => this.Config.EnableAdvanced = value
        );

        // progression restrictions
        menu.AddSectionTitle(this.Manifest, () => "Progression Restrictions");
        menu.AddBoolOption(
            mod: this.Manifest,
            name: () => "Enforce Warp Restrictions",
            tooltip: () => "When enabled, warp locations require meeting in-game progression requirements (e.g., boat repair for Ginger Island, desert access for Desert, etc.)",
            getValue: () => this.Config.EnforceWarpProgressionRestrictions,
            setValue: value => this.Config.EnforceWarpProgressionRestrictions = value
        );

        // other options
        menu.AddSectionTitle(this.Manifest, I18n.Config_Title_OtherOptions);
        menu.AddTextOption(
            mod: this.Manifest,
            name: I18n.Config_DefaultTab_Name,
            tooltip: I18n.Config_DefaultTab_Desc,
            getValue: () => this.Config.DefaultTab.ToString(),
            setValue: value =>
            {
                if (Enum.TryParse(value, out MenuTab tab))
                    this.Config.DefaultTab = tab;
            },
            allowedValues: Enum.GetNames<MenuTab>(),
            formatAllowedValue: value =>
            {
                if (Enum.TryParse(value, out MenuTab tab))
                {
                    return tab switch
                    {
                        MenuTab.PlayerAndTools => I18n.Tabs_PlayerAndTools(),
                        MenuTab.FarmAndFishing => I18n.Tabs_FarmAndFishing(),
                        MenuTab.Skills => I18n.Tabs_Skills(),
                        MenuTab.Weather => I18n.Tabs_Weather(),
                        MenuTab.Relationships => I18n.Tabs_Relationships(),
                        MenuTab.WarpLocations => I18n.Tabs_Warp(),
                        MenuTab.Time => I18n.Tabs_Time(),
                        MenuTab.Advanced => I18n.Tabs_Advanced(),
                        MenuTab.Controls => I18n.Tabs_Controls(),
                        _ => value
                    };
                }

                return value;
            });
        menu.AddParagraph(this.Manifest, I18n.Config_OtherOptions);
    }


    /*********
    ** Private methods
    *********/
    /// <summary>Reset the mod's config to its default values.</summary>
    private void Reset()
    {
        ModConfig config = this.Config;
        ModConfig defaults = new();

        config.OpenMenuKey = defaults.OpenMenuKey;
        config.FreezeTimeKey = defaults.FreezeTimeKey;
        config.GrowTreeKey = defaults.GrowTreeKey;
        config.GrowCropsKey = defaults.GrowCropsKey;

        config.DefaultTab = defaults.DefaultTab;

        // category toggles
        config.EnablePlayerAndTools = defaults.EnablePlayerAndTools;
        config.EnableFarmAndFishing = defaults.EnableFarmAndFishing;
        config.EnableRelationships = defaults.EnableRelationships;
        config.EnableSkills = defaults.EnableSkills;
        config.EnableTime = defaults.EnableTime;
        config.EnableWarps = defaults.EnableWarps;
        config.EnableWeather = defaults.EnableWeather;
        config.EnableAdvanced = defaults.EnableAdvanced;

        // progression restrictions
        config.EnforceWarpProgressionRestrictions = defaults.EnforceWarpProgressionRestrictions;
    }
}
