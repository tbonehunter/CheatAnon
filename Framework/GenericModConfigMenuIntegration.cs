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

        // ── Category toggles ──────────────────────────────────────────────────
        // Each category has a master ON/OFF toggle.
        // Categories that have named sections also list those section toggles directly below.
        // Turning off a section hides it from the in-game menu without disabling its category.
        menu.AddParagraph(this.Manifest, I18n.Config_CategoryToggles_Note);

        // ── Player & Tools ────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, I18n.Tabs_PlayerAndTools);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnablePlayerAndTools, v => this.Config.EnablePlayerAndTools = v,
            I18n.Config_EnablePlayerAndTools_Name, I18n.Config_EnablePlayerAndTools_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnablePlayerStats,        v => this.Config.EnablePlayerStats        = v, I18n.Config_EnablePlayerStats_Name,        I18n.Config_EnablePlayerStats_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableTools,              v => this.Config.EnableTools              = v, I18n.Config_EnableTools_Name,              I18n.Config_EnableTools_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableToolEnchantments,   v => this.Config.EnableToolEnchantments   = v, I18n.Config_EnableToolEnchantments_Name,   I18n.Config_EnableToolEnchantments_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAddMoney,           v => this.Config.EnableAddMoney           = v, I18n.Config_EnableAddMoney_Name,           I18n.Config_EnableAddMoney_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAddCasinoCoins,     v => this.Config.EnableAddCasinoCoins     = v, I18n.Config_EnableAddCasinoCoins_Name,     I18n.Config_EnableAddCasinoCoins_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAddGoldenWalnuts,   v => this.Config.EnableAddGoldenWalnuts   = v, I18n.Config_EnableAddGoldenWalnuts_Name,   I18n.Config_EnableAddGoldenWalnuts_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAddQiGems,          v => this.Config.EnableAddQiGems          = v, I18n.Config_EnableAddQiGems_Name,          I18n.Config_EnableAddQiGems_Desc);

        // ── Farm & Fishing ────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, I18n.Tabs_FarmAndFishing);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableFarmAndFishing, v => this.Config.EnableFarmAndFishing = v,
            I18n.Config_EnableFarmAndFishing_Name, I18n.Config_EnableFarmAndFishing_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableFarm,           v => this.Config.EnableFarm           = v, I18n.Config_EnableFarm_Name,           I18n.Config_EnableFarm_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableFishing,        v => this.Config.EnableFishing        = v, I18n.Config_EnableFishing_Name,        I18n.Config_EnableFishing_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableFastMachines,   v => this.Config.EnableFastMachines   = v, I18n.Config_EnableFastMachines_Name,   I18n.Config_EnableFastMachines_Desc);

        // ── Skills ────────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, I18n.Tabs_Skills);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableSkills, v => this.Config.EnableSkills = v,
            I18n.Config_EnableSkills_Name, I18n.Config_EnableSkills_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableSkillLevels,  v => this.Config.EnableSkillLevels  = v, I18n.Config_EnableSkillLevels_Name,  I18n.Config_EnableSkillLevels_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableProfessions,  v => this.Config.EnableProfessions  = v, I18n.Config_EnableProfessions_Name,  I18n.Config_EnableProfessions_Desc);

        // ── Relationships ─────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, I18n.Tabs_Relationships);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableRelationships, v => this.Config.EnableRelationships = v,
            I18n.Config_EnableRelationships_Name, I18n.Config_EnableRelationships_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableGiveGiftsAnytime,       v => this.Config.EnableGiveGiftsAnytime       = v, I18n.Config_EnableGiveGiftsAnytime_Name,       I18n.Config_EnableGiveGiftsAnytime_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableNoFriendshipDecay,      v => this.Config.EnableNoFriendshipDecay      = v, I18n.Config_EnableNoFriendshipDecay_Name,      I18n.Config_EnableNoFriendshipDecay_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAdjustFriendshipLevels, v => this.Config.EnableAdjustFriendshipLevels = v, I18n.Config_EnableAdjustFriendshipLevels_Name, I18n.Config_EnableAdjustFriendshipLevels_Desc);

        // ── Time ──────────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, I18n.Tabs_Time);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableTime, v => this.Config.EnableTime = v,
            I18n.Config_EnableTime_Name, I18n.Config_EnableTime_Desc);

        // ── Warps ─────────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, I18n.Tabs_Warp);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarps, v => this.Config.EnableWarps = v,
            I18n.Config_EnableWarps_Name, I18n.Config_EnableWarps_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnforceWarpProgressionRestrictions, v => this.Config.EnforceWarpProgressionRestrictions = v,
            I18n.Config_EnforceWarpProgression_Name, I18n.Config_EnforceWarpProgression_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionMain,     v => this.Config.EnableWarpSectionMain     = v, I18n.Config_EnableWarpSectionMain_Name,     I18n.Config_EnableWarpSectionMain_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionTown,     v => this.Config.EnableWarpSectionTown     = v, I18n.Config_EnableWarpSectionTown_Name,     I18n.Config_EnableWarpSectionTown_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionForest,   v => this.Config.EnableWarpSectionForest   = v, I18n.Config_EnableWarpSectionForest_Name,   I18n.Config_EnableWarpSectionForest_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionMountain, v => this.Config.EnableWarpSectionMountain = v, I18n.Config_EnableWarpSectionMountain_Name, I18n.Config_EnableWarpSectionMountain_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionBeach,    v => this.Config.EnableWarpSectionBeach    = v, I18n.Config_EnableWarpSectionBeach_Name,    I18n.Config_EnableWarpSectionBeach_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionDesert,   v => this.Config.EnableWarpSectionDesert   = v, I18n.Config_EnableWarpSectionDesert_Name,   I18n.Config_EnableWarpSectionDesert_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionIsland,   v => this.Config.EnableWarpSectionIsland   = v, I18n.Config_EnableWarpSectionIsland_Name,   I18n.Config_EnableWarpSectionIsland_Desc);

        // ── Weather ───────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, I18n.Tabs_Weather);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWeather, v => this.Config.EnableWeather = v,
            I18n.Config_EnableWeather_Name, I18n.Config_EnableWeather_Desc);

        // ── Advanced ──────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, I18n.Tabs_Advanced);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAdvanced, v => this.Config.EnableAdvanced = v,
            I18n.Config_EnableAdvanced_Name, I18n.Config_EnableAdvanced_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableCompleteQuests,   v => this.Config.EnableCompleteQuests   = v, I18n.Config_EnableCompleteQuests_Name,   I18n.Config_EnableCompleteQuests_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWalletItems,      v => this.Config.EnableWalletItems      = v, I18n.Config_EnableWalletItems_Name,      I18n.Config_EnableWalletItems_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableUnlockedAreas,    v => this.Config.EnableUnlockedAreas    = v, I18n.Config_EnableUnlockedAreas_Name,    I18n.Config_EnableUnlockedAreas_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableUnlockedContent,  v => this.Config.EnableUnlockedContent  = v, I18n.Config_EnableUnlockedContent_Name,  I18n.Config_EnableUnlockedContent_Desc);
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableCommunityCenter,  v => this.Config.EnableCommunityCenter  = v, I18n.Config_EnableCommunityCenter_Name,  I18n.Config_EnableCommunityCenter_Desc);

        // ── Other options ──────────────────────────────────────────────────────
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
        config.EnableRelationships  = defaults.EnableRelationships;
        config.EnableSkills         = defaults.EnableSkills;
        config.EnableTime           = defaults.EnableTime;
        config.EnableWarps          = defaults.EnableWarps;
        config.EnableWeather        = defaults.EnableWeather;
        config.EnableAdvanced       = defaults.EnableAdvanced;

        // progression restrictions
        config.EnforceWarpProgressionRestrictions = defaults.EnforceWarpProgressionRestrictions;

        // subcategory toggles - Player & Tools
        config.EnablePlayerStats       = defaults.EnablePlayerStats;
        config.EnableTools             = defaults.EnableTools;
        config.EnableToolEnchantments  = defaults.EnableToolEnchantments;
        config.EnableAddMoney          = defaults.EnableAddMoney;
        config.EnableAddCasinoCoins    = defaults.EnableAddCasinoCoins;
        config.EnableAddGoldenWalnuts  = defaults.EnableAddGoldenWalnuts;
        config.EnableAddQiGems         = defaults.EnableAddQiGems;

        // subcategory toggles - Farm & Fishing
        config.EnableFarm         = defaults.EnableFarm;
        config.EnableFishing      = defaults.EnableFishing;
        config.EnableFastMachines = defaults.EnableFastMachines;

        // subcategory toggles - Skills
        config.EnableSkillLevels = defaults.EnableSkillLevels;
        config.EnableProfessions = defaults.EnableProfessions;

        // subcategory toggles - Relationships
        config.EnableGiveGiftsAnytime       = defaults.EnableGiveGiftsAnytime;
        config.EnableNoFriendshipDecay      = defaults.EnableNoFriendshipDecay;
        config.EnableAdjustFriendshipLevels = defaults.EnableAdjustFriendshipLevels;

        // subcategory toggles - Warps
        config.EnableWarpSectionMain     = defaults.EnableWarpSectionMain;
        config.EnableWarpSectionTown     = defaults.EnableWarpSectionTown;
        config.EnableWarpSectionForest   = defaults.EnableWarpSectionForest;
        config.EnableWarpSectionMountain = defaults.EnableWarpSectionMountain;
        config.EnableWarpSectionBeach    = defaults.EnableWarpSectionBeach;
        config.EnableWarpSectionDesert   = defaults.EnableWarpSectionDesert;
        config.EnableWarpSectionIsland   = defaults.EnableWarpSectionIsland;

        // subcategory toggles - Advanced
        config.EnableCompleteQuests  = defaults.EnableCompleteQuests;
        config.EnableWalletItems     = defaults.EnableWalletItems;
        config.EnableUnlockedAreas   = defaults.EnableUnlockedAreas;
        config.EnableUnlockedContent = defaults.EnableUnlockedContent;
        config.EnableCommunityCenter = defaults.EnableCommunityCenter;

        // warp hotkeys are intentionally NOT reset (player-defined key bindings)
    }
}
