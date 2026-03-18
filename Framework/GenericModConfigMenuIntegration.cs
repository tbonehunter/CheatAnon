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
        menu.AddParagraph(this.Manifest, () => "All changes take effect at the title screen. Turning a section OFF hides it from the in-game cheats menu.");

        // ── Player & Tools ────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, () => "Player & Tools");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnablePlayerAndTools, v => this.Config.EnablePlayerAndTools = v,
            () => "Enable Player & Tools", () => "Master toggle — turns off the entire Player & Tools tab.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnablePlayerStats,        v => this.Config.EnablePlayerStats        = v, () => "  Player Stats",        () => "Health, stamina, speed, luck, one-hit kill, inventory size, cooldowns.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableTools,              v => this.Config.EnableTools              = v, () => "  Tools",              () => "One-hit break, infinite water, harvest with scythe.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableToolEnchantments,   v => this.Config.EnableToolEnchantments   = v, () => "  Tool Enchantments", () => "Apply enchantments to tools.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAddMoney,           v => this.Config.EnableAddMoney           = v, () => "  Add Money",          () => "Add gold to your wallet.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAddCasinoCoins,     v => this.Config.EnableAddCasinoCoins     = v, () => "  Add Casino Coins",   () => "Add Qi coins.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAddGoldenWalnuts,   v => this.Config.EnableAddGoldenWalnuts   = v, () => "  Add Golden Walnuts", () => "Add Golden Walnuts.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAddQiGems,          v => this.Config.EnableAddQiGems          = v, () => "  Add Qi Gems",        () => "Add Qi Gems.");

        // ── Farm & Fishing ────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, () => "Farm & Fishing");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableFarmAndFishing, v => this.Config.EnableFarmAndFishing = v,
            () => "Enable Farm & Fishing", () => "Master toggle — turns off the entire Farm & Fishing tab.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableFarm,           v => this.Config.EnableFarm           = v, () => "  Farm",                    () => "Auto-water, fences, instant build, animals, hay.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableFishing,        v => this.Config.EnableFishing        = v, () => "  Fishing",                  () => "Instant catch/bite, distance, tackle, treasure.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableFastMachines,   v => this.Config.EnableFastMachines   = v, () => "  Fast Machine Processing",   () => "All machines finish instantly.");

        // ── Skills ────────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, () => "Skills");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableSkills, v => this.Config.EnableSkills = v,
            () => "Enable Skills", () => "Master toggle — turns off the entire Skills tab.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableSkillLevels,  v => this.Config.EnableSkillLevels  = v, () => "  Skill Levels",  () => "Raise individual skill levels.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableProfessions,  v => this.Config.EnableProfessions  = v, () => "  Professions",   () => "Change profession choices.");

        // ── Relationships ─────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, () => "Relationships");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableRelationships, v => this.Config.EnableRelationships = v,
            () => "Enable Relationships", () => "Master toggle — turns off the entire Relationships tab.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableGiveGiftsAnytime,       v => this.Config.EnableGiveGiftsAnytime       = v, () => "  Give Gifts Anytime",        () => "Remove the weekly gift limit.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableNoFriendshipDecay,      v => this.Config.EnableNoFriendshipDecay      = v, () => "  No Friendship Decay",       () => "Friendship points don't drop over time.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAdjustFriendshipLevels, v => this.Config.EnableAdjustFriendshipLevels = v, () => "  Adjust Friendship Levels",  () => "Per-NPC heart level sliders.");

        // ── Time ──────────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, () => "Time");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableTime, v => this.Config.EnableTime = v,
            () => "Enable Time", () => "Freeze time, set time of day, day, season, and year.");

        // ── Warps ─────────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, () => "Warps");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarps, v => this.Config.EnableWarps = v,
            () => "Enable Warps", () => "Master toggle — turns off the entire Warps tab.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnforceWarpProgressionRestrictions, v => this.Config.EnforceWarpProgressionRestrictions = v,
            () => "  Enforce Progression Restrictions", () => "Warp sections only appear after you've unlocked them in-game (e.g., boat repair for Ginger Island). Default: OFF.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionMain,     v => this.Config.EnableWarpSectionMain     = v, () => "  Main Locations",  () => "Farm, Mines, and other main-area warps.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionTown,     v => this.Config.EnableWarpSectionTown     = v, () => "  Town",            () => "Town-area warps.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionForest,   v => this.Config.EnableWarpSectionForest   = v, () => "  Forest",          () => "Forest-area warps.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionMountain, v => this.Config.EnableWarpSectionMountain = v, () => "  Mountain",        () => "Mountain-area warps.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionBeach,    v => this.Config.EnableWarpSectionBeach    = v, () => "  Beach",           () => "Beach-area warps.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionDesert,   v => this.Config.EnableWarpSectionDesert   = v, () => "  Desert",          () => "Desert warps.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWarpSectionIsland,   v => this.Config.EnableWarpSectionIsland   = v, () => "  Ginger Island",   () => "Ginger Island warps.");

        // ── Weather ───────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, () => "Weather");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWeather, v => this.Config.EnableWeather = v,
            () => "Enable Weather", () => "Set the weather for tomorrow.");

        // ── Advanced ──────────────────────────────────────────────────────────
        menu.AddSectionTitle(this.Manifest, () => "Advanced");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableAdvanced, v => this.Config.EnableAdvanced = v,
            () => "Enable Advanced", () => "Master toggle — turns off the entire Advanced tab.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableCompleteQuests,   v => this.Config.EnableCompleteQuests   = v, () => "  Complete Quests",   () => "Instantly complete or cancel quests/special orders.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableWalletItems,      v => this.Config.EnableWalletItems      = v, () => "  Wallet Items",      () => "Add special items to your wallet.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableUnlockedAreas,    v => this.Config.EnableUnlockedAreas    = v, () => "  Unlocked Areas",    () => "Unlock doors and NPC areas.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableUnlockedContent,  v => this.Config.EnableUnlockedContent  = v, () => "  Unlocked Content",  () => "Unlock tailoring, Junimo text, perfection tracker.");
        menu.AddBoolOption(this.Manifest, () => this.Config.EnableCommunityCenter,  v => this.Config.EnableCommunityCenter  = v, () => "  Community Center",  () => "Complete or reset Community Center bundles / Joja purchases.");

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
